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
    //[Serializable]
    //public enum JobStatus
    //{
    //    None,
    //    InQueue,
    //    Running,
    //    OK,
    //    Killed,
    //    TimedOut,
    //    Failed
    //}

    [Serializable]
    public class NetgenJob : NamedClass
    {
        // Variables                                                                                                                
        protected string _workDirectory;
        protected string _executable;
        protected string _argument;
        protected JobStatus _jobStatus;
        protected int _numCPUs;
        protected string _output;
        protected object myLock = new object();


        [NonSerializedAttribute] protected System.Diagnostics.Stopwatch _watch;
        [NonSerializedAttribute] protected System.Diagnostics.Stopwatch _timerWatch;
        [NonSerializedAttribute] private Process _exe;
        [NonSerialized] private string _outputFileName;
        [NonSerialized] private string _errorFileName;


        // Properties                                                                                                               
        //public string Name { get { return _name; } set { _name = value; } }
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
        public string WorkDirectory
        {
            get { return _workDirectory; }
            set { _workDirectory = value; }
        }
        public JobStatus JobStatus { get { return _jobStatus; } }
        public string Output { get { return _output; } }
        public int NumCPUs { get { return _numCPUs; } set { _numCPUs = value; } }


        // Events                                                                                                                           
        public event Action<string> AppendOutput;
        public event Action<NetgenJob> JobStarted;
        //public event Action<NetgenJob> JobCompleted;


        // Constructor                                                                                                              
        public NetgenJob(string name, string executable, string argument, string workDirectory)
            : base(name)
        {
            _executable = executable;
            _argument = argument;
            _workDirectory = workDirectory;
            _numCPUs = 1;

            _exe = null;
            _jobStatus = JobStatus.None;
            _timerWatch = null;
            _watch = null;
        }

        // Methods                                                                                                                  
        public void Submit()
        {
            _watch = new Stopwatch();
            _timerWatch = new Stopwatch();

            _jobStatus = CaeJob.JobStatus.Running;

            if (JobStarted != null) JobStarted(this);

            Run();
            RunCompleted();
        }

        private void Run()
        {
            if (!File.Exists(_executable)) throw new Exception("The file '" + _executable + "' does not exist.");
            _timerWatch.Start();
            _watch.Start();

            _output = "";
            AddDataToOutput("Running command: " + _executable + " " + _argument);

            //string tmpName = Path.GetFileName(_executable) + "_" + _argument.Substring(0, Math.Min(_argument.Length, 15));
            string tmpName = Path.GetFileName(Name);
            _outputFileName = Path.Combine(_workDirectory, "_output_" + tmpName + ".txt");
            _errorFileName = Path.Combine(_workDirectory, "_error_" + tmpName + ".txt");

            if (File.Exists(_outputFileName)) File.Delete(_outputFileName);
            if (File.Exists(_errorFileName)) File.Delete(_errorFileName);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.FileName = _executable;
            psi.Arguments = _argument;
            psi.WorkingDirectory = _workDirectory;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            Debug.WriteLine(DateTime.Now + "   Start proces: " + tmpName + " in: " + _workDirectory);

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
                        File.AppendAllText(_errorFileName, e.Data + Environment.NewLine);
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
                    if (_jobStatus == CaeJob.JobStatus.Running) _jobStatus = CaeJob.JobStatus.OK;
                }
                else
                {
                    // Timed out.
                    Kill("Time out.");
                    _jobStatus = CaeJob.JobStatus.TimedOut;
                }
                _exe.Close();
            }

            //Debug.WriteLine(DateTime.Now + "   End proces: " + tmpName + " in: " + _workDirectory);
        }
        void RunCompleted()
        {
            _timerWatch.Stop();
            _watch.Stop();
            //
            AddDataToOutput("");
            AddDataToOutput("Elapsed time [s]: " + _watch.Elapsed.TotalSeconds.ToString(), 0);
            // Wait for the last optput
            lock (myLock) {}
            // Dereference the link to otheh objects
            JobStarted = null;
            // JobCompleted = null;
            AppendOutput = null;
        }

        public void Kill(string message)
        {
            try
            {
                if (_exe != null)
                {
                    myLock = new object();  // to unlock old lock in AddDataToOutput
                    //
                    AddDataToOutput(message, 0);
                    //
                    _timerWatch.Stop();
                    _watch.Stop();

                    _jobStatus = JobStatus.Killed;  // this has to be here before _exe.Kill, to return the correct status

                    KillAllProcessesSpawnedBy((UInt32)_exe.Id);

                    _exe.Kill();
                }
            }
            catch
            { }
            finally
            {
                // dereference the link to otheh objects
                JobStarted = null;
                //JobCompleted = null;
                AppendOutput = null;
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
        private void AddDataToOutput(string data, int elapsedMilliseconds = 200)
        {
            lock (myLock)
            {
                if (_output != null)
                {
                    _output += data + Environment.NewLine;
                    //
                    if (_timerWatch.ElapsedMilliseconds > elapsedMilliseconds)
                    {
                        if (AppendOutput != null && _output.Length > 0)
                        {
                            AppendOutput(_output);
                            File.AppendAllText(_outputFileName, _output);
                        }
                        _output = "";
                        _timerWatch.Restart();
                    }
                }
            }
        }
    }
}
