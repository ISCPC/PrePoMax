using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace PrePoMax
{
    [Serializable]
    public class GeneralSettings : ISettings
    {
        // Variables                                                                                                                
        private bool _openLastFile;
        private string _lastFileName;
        private bool _saveResultsInPmx;
        private LinkedList<string> _recentFiles;
        private UnitSystemType _unitSystemType;

        // Form size and position
        private FormWindowState _formWindowState;
        private Size _formSize;
        private double _formRelativeXLocation;
        private double _formRelativeYLocation;

        // Properties                                                                                                               
        public bool OpenLastFile { get { return _openLastFile; } set { _openLastFile = value; } }
        public string LastFileName { get { return _lastFileName; } set { _lastFileName = value; } }
        public bool SaveResultsInPmx { get { return _saveResultsInPmx; } set { _saveResultsInPmx = value; } }
        public UnitSystemType UnitSystemType { get { return _unitSystemType; } set { _unitSystemType = value; } }


        // Constructors                                                                                                             
        public GeneralSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _openLastFile = true;
            _lastFileName = null;
            _saveResultsInPmx = true;
            _recentFiles = null;
            _unitSystemType = UnitSystemType.Undefined;
            //
            ResetFormSize();
        }
        //
        private void ResetFormSize()
        {
            _formWindowState = FormWindowState.Normal;
            _formSize = new Size(1280, 720);
            _formRelativeXLocation = 0.5;
            _formRelativeYLocation = 0.5;
        }
        public string[] GetRecentFiles()
        {
            if (_recentFiles != null) return _recentFiles.ToArray();
            else return null;
        }
        public void AddRecentFile(string fileNameWithpPath)
        {
            if (_recentFiles == null) _recentFiles = new LinkedList<string>();
            //
            if (_recentFiles.Count == 0) _recentFiles.AddFirst(fileNameWithpPath);
            else
            {
                if (!_recentFiles.Contains(fileNameWithpPath))
                {
                    int _maxRecentFiles = 15;
                    while (_recentFiles.Count >= _maxRecentFiles) _recentFiles.RemoveLast();
                    _recentFiles.AddFirst(fileNameWithpPath);
                }
                else
                {
                    _recentFiles.Remove(fileNameWithpPath);
                    _recentFiles.AddFirst(fileNameWithpPath);
                }
            }
        }
        public void ClearRecentFiles()
        {
            _recentFiles.Clear();
        }
        //
        public void SaveFormSize(Form form)
        {
            Size size;
            Point location;
            Rectangle resolution = Screen.FromControl(form).Bounds;
            //
            if (_formWindowState == FormWindowState.Normal && form.Size.Width < resolution.Width * 0.9 &&
                form.Size.Height < resolution.Height * 0.9)
            {
                size = form.Size;
                location = form.Location;
            }
            else
            {
                size = form.RestoreBounds.Size;
                location = form.RestoreBounds.Location;
            }
            //
            Point center = new Point(location.X + size.Width / 2, location.Y + size.Height / 2);
            // Set values
            _formWindowState = form.WindowState;
            _formSize = size;
            _formRelativeXLocation = (double)center.X / resolution.Width;
            _formRelativeYLocation = (double)center.Y / resolution.Height;
        }
        public void ApplyFormSize(Form form)
        {
            Rectangle resolution = Screen.FromControl(form).Bounds;
            //
            if (_formSize.Width <= form.MinimumSize.Width || _formSize.Height <= form.MinimumSize.Height)
                ResetFormSize(); // also resets _formRelativeXLocation and _formRelativeYLocation
            //
            Point center = new Point((int)(resolution.Width * _formRelativeXLocation),
                                     (int)(resolution.Height * _formRelativeYLocation));
            //
            Rectangle bounds = new Rectangle(new Point(center.X - _formSize.Width / 2, center.Y - _formSize.Height / 2), _formSize);
            //
            if (bounds.Right > resolution.Width) bounds.X = resolution.Width - bounds.Width;
            if (bounds.Left <= 0) bounds.X = 0;
            if (bounds.Bottom > resolution.Bottom) bounds.Y = resolution.Height - bounds.Height;
            if (bounds.Top <= 0) bounds.Y = 0;
            //
            form.WindowState = _formWindowState;
            form.Bounds = bounds;
            // Prevent minimized window at startup
            if (form.WindowState == FormWindowState.Minimized) form.WindowState = FormWindowState.Normal;
        }


    }
}
