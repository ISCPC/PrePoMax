using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Management;
using System.ComponentModel;
using CaeGlobals;

namespace CaeJob
{
    [Serializable]
    public enum JobStatus
    {
        None,
        InQueue,
        Running,
        OK,
        Killed,
        TimedOut,
        Failed
    }

    [Serializable]
    public class AnalysisJob : NamedClass
    {
        // Variables                                                                                                                        
        protected string _workDirectory;

        protected string _executable;
        protected string _argument;
        protected JobStatus _jobStatus;
        protected int _numCPUs;
        protected List<EnvironmentVariable> _environmentVariables;
        protected string _output;
        protected long _statusFileLength;
        protected string _statusFileContents;
        protected long _convergenceFileLength;
        protected string _convergenceFileContents;
        

        [NonSerializedAttribute]
        private System.Windows.Threading.DispatcherTimer _timer;

        [NonSerializedAttribute]
        protected System.Diagnostics.Stopwatch _watch;

        [NonSerializedAttribute]
        private Process _exe;


        // Properties                                                                                                               
        public override string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                _argument = Name;
            }
        }
        public string WorkDirectory
        {
            get { return _workDirectory; }
            set { _workDirectory = value; }
        }
        public string Executable
        {
            get { return _executable; }
            set { _executable = value; }
        }
        public string Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }
        public JobStatus JobStatus { get { return _jobStatus; } }
        public int NumCPUs
        {
            get { return _numCPUs; }
            set
            {
                _numCPUs = value;

                ProcessStartInfo psi = new ProcessStartInfo();
                SetNumberOfProcessors(psi);
            }
        }
        public List<EnvironmentVariable> EnvironmentVariables
        {
            get { return _environmentVariables; }
            set { _environmentVariables = value; }
        }
        public string OutputData { get { return _output; } }
        public string StatusFileData { get { return _statusFileContents; } }
        public string ConvergenceFileData { get { return _convergenceFileContents; } }

        

        // Events                                                                                                                   
        public event Action DataOutput;


        // Callback                                                                                                                 
        public Action<string, JobStatus> JobStatusChanged;


        // Constructor                                                                                                              
        public AnalysisJob(string name, string executable, string argument, string workDirectory)
            : base (name)
        {            
            _executable = executable;
            _argument = argument;
            _workDirectory = workDirectory;
            _numCPUs = 1;
            _environmentVariables = new List<EnvironmentVariable>();

            _exe = null;
            _jobStatus = JobStatus.None;
            _watch = null;
        }


        // Event handlers                                                                                                           
        void Timer_Tick(object sender, EventArgs e)
        {
            DataOutput?.Invoke();
        }


        // Methods                                                                                                                  
        public void Submit()
        {
            _output = "";
            AddDataToOutput(DateTime.Now + Environment.NewLine);
            AddDataToOutput("Running command: " + _executable + " " + _argument);

            _statusFileLength = -1;
            _statusFileContents = "";

            _convergenceFileLength = -1;
            _convergenceFileContents = "";

            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            _timer.Tick += Timer_Tick;
            
            _watch = new Stopwatch();
            
            _jobStatus = CaeJob.JobStatus.Running;

            JobStatusChanged?.Invoke(_name, _jobStatus);

            using (BackgroundWorker bwStart = new BackgroundWorker())
            {
                bwStart.DoWork += bwStart_DoWork;
                bwStart.RunWorkerCompleted += bwStart_RunWorkerCompleted;
                if (!bwStart.IsBusy)
                {
                    _timer.Start();
                    _watch.Start();
                    bwStart.RunWorkerAsync();
                }
            }
        }
        void bwStart_DoWork(object sender, DoWorkEventArgs e)
        {
            if (CaeGlobals.Tools.IsWindows10orNewer()) Run_Win10();
            else Run_OldWin();
        }
        void bwStart_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _timer.Stop();
            _watch.Stop();

            AddDataToOutput("");
            if (_jobStatus == JobStatus.OK && !File.Exists(Path.Combine(WorkDirectory, _name + ".frd")))
                AddDataToOutput(" Job failed.");
            else if (_jobStatus == JobStatus.Killed)
                AddDataToOutput(" Job killed.");
            else if (_jobStatus == JobStatus.Failed)
                AddDataToOutput(" Job failed.");

            AddDataToOutput("");
            AddDataToOutput(" Elapsed time [s]: " + _watch.Elapsed.TotalSeconds.ToString());

            Timer_Tick(null, null);

            JobStatusChanged?.Invoke(_name, _jobStatus);

            JobStatusChanged = null;   // dereference the link to otheh objects

            DataOutput = null;      
        }

        private void Run_OldWin()
        {
            string tmpName = Path.GetFileName(Name);
            string outputFileName = Path.Combine(_workDirectory, "_output_" + tmpName + ".txt");
            string errorFileName = Path.Combine(_workDirectory, "_error_" + tmpName + ".txt");

            if (File.Exists(outputFileName)) File.Delete(outputFileName);
            if (File.Exists(errorFileName)) File.Delete(errorFileName);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = _executable;
            psi.Arguments = _argument;
            psi.WorkingDirectory = _workDirectory;
            psi.UseShellExecute = false;

            SetEnvironmentVariables(psi);

            _exe = new Process();
            _exe.StartInfo = psi;
            _exe.Start();

            int ms = 1000 * 3600 * 24 * 7 * 3; // 3 weeks
            if (_exe.WaitForExit(ms))
            {
                // Process completed. Check process.ExitCode here.
                    
                // after Kill() _jobStatus is Killed
                _jobStatus = CaeJob.JobStatus.OK;
            }
            else
            {
                // Timed out.
                Kill("Time out.");
                Debug.WriteLine(DateTime.Now + "   Timeout proces: " + tmpName + " in: " + _workDirectory);
                _jobStatus = CaeJob.JobStatus.TimedOut;
            }
            _exe.Close();
        }
        private void Run_Win10()
        {
            string tmpName = Path.GetFileName(Name);
            string outputFileName = Path.Combine(_workDirectory, "_output_" + tmpName + ".txt");
            string errorFileName = Path.Combine(_workDirectory, "_error_" + tmpName + ".txt");

            if (File.Exists(outputFileName)) File.Delete(outputFileName);
            if (File.Exists(errorFileName)) File.Delete(errorFileName);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.FileName = _executable;
            psi.Arguments = _argument;
            psi.WorkingDirectory = _workDirectory;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            SetEnvironmentVariables(psi);

             _exe = new Process();
            _exe.StartInfo = psi;

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                _exe.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        // the safe wait handle closes on kill
                        if (!outputWaitHandle.SafeWaitHandle.IsClosed) outputWaitHandle.Set();
                    }
                    else
                    {
                        File.AppendAllText(outputFileName, e.Data + Environment.NewLine);
                        AddDataToOutput(e.Data);
                    }
                };

                _exe.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        // the safe wait handle closes on kill
                        if (!errorWaitHandle.SafeWaitHandle.IsClosed) errorWaitHandle.Set();
                    }
                    else
                    {
                        File.AppendAllText(errorFileName, e.Data + Environment.NewLine);
                        AddDataToOutput(e.Data);
                    }
                };

                _exe.Start();

                _exe.BeginOutputReadLine();
                _exe.BeginErrorReadLine();
                int ms = 1000 * 3600 * 24 * 7 * 3; // 3 weeks
                if (_exe.WaitForExit(ms) && outputWaitHandle.WaitOne(ms) && errorWaitHandle.WaitOne(ms))
                {
                    // Process completed. Check process.ExitCode here.
                    // after Kill() _jobStatus is Killed
                    if (_jobStatus != JobStatus.Killed) _jobStatus = CaeJob.JobStatus.OK;
                }
                else
                {
                    // Timed out.
                    Kill("Time out.");
                    Debug.WriteLine(DateTime.Now + "   Timeout proces: " + tmpName + " in: " + _workDirectory);
                    _jobStatus = CaeJob.JobStatus.TimedOut;
                }
                _exe.Close();
            }
        }

        public void Kill(string message)
        {
            try
            {
                if (_exe != null)
                {
                    AddDataToOutput(message);
                    _watch.Stop();
                    
                    KillAllProcessesSpawnedBy((UInt32)_exe.Id);
                    _exe.Kill();
                }
            }
            finally
            {
                _jobStatus = JobStatus.Killed;
            }
        }
        private static void KillAllProcessesSpawnedBy(UInt32 parentProcessId)
        {
            // NOTE: Process Ids are reused!
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_Process " +
                "WHERE ParentProcessId=" + parentProcessId);
            ManagementObjectCollection collection = searcher.Get();
            if (collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    UInt32 childProcessId = (UInt32)item["ProcessId"];
                    if ((int)childProcessId != Process.GetCurrentProcess().Id)
                    {

                        KillAllProcessesSpawnedBy(childProcessId);

                        try
                        {
                            Process childProcess = Process.GetProcessById((int)childProcessId);
                            childProcess.Kill();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void AddDataToOutput(string data)
        {
            _output += data + Environment.NewLine;
            GetStatusFileContents();
            GetConvergenceFileContents();
        }
        private void GetStatusFileContents()
        {
            try
            {
                string statusFileName = Path.Combine(_workDirectory, Name + ".sta");
                if (!File.Exists(statusFileName)) return;

                long size = new System.IO.FileInfo(statusFileName).Length;

                if (size != _statusFileLength)
                {
                    using (FileStream fileStream = new FileStream(statusFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            _statusFileContents = streamReader.ReadToEnd();
                        }
                    }
                    _statusFileLength = size;
                }
            }
            catch
            {
            }
        }
        private void GetConvergenceFileContents()
        {
            try
            {
                string convergenceFileName = Path.Combine(_workDirectory, Name + ".cvg");
                if (!File.Exists(convergenceFileName)) return;

                long size = new System.IO.FileInfo(convergenceFileName).Length;

                if (size != _convergenceFileLength)
                {
                    using (FileStream fileStream = new FileStream(convergenceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            _convergenceFileContents = streamReader.ReadToEnd();
                        }
                    }
                    _convergenceFileLength = size;
                }
            }
            catch
            {
            }
        }

        public static bool IsAdministrator()
        {
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            // DOMAINNAME\Domain Admins RID: 0x200
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator) || principal.IsInRole(0x200);
        }
        public void ResetJobStatus()
        {
            _jobStatus = CaeJob.JobStatus.None;
        }

        private void SetEnvironmentVariables(ProcessStartInfo psi)
        {
            SetNumberOfProcessors(psi);
            if (_environmentVariables != null)
            {
                foreach (var environmentVariable in _environmentVariables)
                {
                    SetEnvironmentVariable(psi, environmentVariable);
                }
            }
        }
        private void SetNumberOfProcessors(ProcessStartInfo psi)
        {
            SetEnvironmentVariable(psi, new EnvironmentVariable() { Name = "OMP_NUM_THREADS", Value = _numCPUs.ToString() });
        }
        private void SetEnvironmentVariable(ProcessStartInfo psi, EnvironmentVariable environmentVariable)
        {
            try
            {
                if (psi.EnvironmentVariables.ContainsKey(environmentVariable.Name)) psi.EnvironmentVariables.Remove(environmentVariable.Name);
                psi.EnvironmentVariables.Add(environmentVariable.Name, environmentVariable.Value);
                if (!psi.EnvironmentVariables.ContainsKey(environmentVariable.Name)) throw new Exception();
            }
            catch
            {
                AddDataToOutput("To add environmental variable '" + environmentVariable.Name + "' to the analysis the program must be run with elevated permisions (Run as administrator).");
            }
        }


    }
}
