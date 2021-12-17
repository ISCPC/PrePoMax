using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace vtkControl
{
    

    class vtkMaxStatusBlockWidget : vtkMaxTextWidget
    {
        // Variables                                                                                                                
        private string _text;
        private string _name;
        private DateTime _dateTime;
        private float _analysisTime;
        private string _analysisTimeUnit;
        private float _animationScaleFactor;
        private float _deformationScaleFactor;
        private DataFieldType _fieldType;
        private int _stepNumber;
        private int _incrementNumber;


        // Properties                                                                                                               
        public string Name { get { return _name; } set { _name = value; SetText(); } }
        public DateTime DateTime { get { return _dateTime; } set { _dateTime = value; SetText(); } }
        public float AnalysisTime { get { return _analysisTime; } set { _analysisTime = value; SetText(); } }
        public string AnalysisTimeUnit { get { return _analysisTimeUnit; } set { _analysisTimeUnit = value; SetText(); } }
        public float DeformationScaleFactor { get { return _deformationScaleFactor; } set { _deformationScaleFactor = value; SetText(); } }
        public float AnimationScaleFactor { get { return _animationScaleFactor; } set { _animationScaleFactor = value; SetText(); } }
        public DataFieldType FieldType { get { return _fieldType; } set { _fieldType = value; SetText(); } }
        public int StepNumber { get { return _stepNumber; } set { _stepNumber = value; SetText(); } }
        public int IncrementNumber { get { return _incrementNumber; } set { _incrementNumber = value; SetText(); } }


        // Constructors                                                                                                             
        public vtkMaxStatusBlockWidget()
        {
            _text = "text";
            _name = "name";
            _dateTime = DateTime.Now;
            _analysisTime = 0;
            _analysisTimeUnit = "";
            _animationScaleFactor = -1;
            _deformationScaleFactor = 1;
            _fieldType = DataFieldType.Static;
        }


        // Private methods                                                                                                          
        private void SetText()
        {
            string sysUIFormat = System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
            //
            _text = "Name: " + _name + "   ";
            _text += "Date: " + _dateTime.ToString(sysUIFormat) + "   Time: " + _dateTime.ToString("HH:mm:ss") + Environment.NewLine;
            //
            if (_fieldType == DataFieldType.Static)
            {
                _text += "Step: #" + _stepNumber + "   Increment: #" + _incrementNumber +
                         "   Analysis time: " + _analysisTime.ToString() + " " + _analysisTimeUnit;
            }
            else if (_fieldType == DataFieldType.Frequency)
            {
                _text += "Step: #" + _stepNumber + "   Mode: #" + _incrementNumber + "   Frequency: " +
                         _analysisTime.ToString() + " " + _analysisTimeUnit;
            }
            else if (_fieldType == DataFieldType.Buckling)
            {
                _text += "Step: #" + _stepNumber + "   Buckling factor: " + _analysisTime.ToString();
            }
            else if (_fieldType == DataFieldType.LastIterations)
            {
                _text += "Increment: #" + _stepNumber + "   Iteration: #" + _incrementNumber +
                         "   Analysis time: " + _analysisTime.ToString() + " " + _analysisTimeUnit;
            }
            else throw new NotSupportedException();
            //
            _text += Environment.NewLine;
            _text += "Deformation scale factor: " + _deformationScaleFactor.ToString();
            //
            if (_animationScaleFactor >= 0)
            {
                _text += Environment.NewLine;
                _text += "Animation scale factor: " + _animationScaleFactor.ToString();
            }
            //
            this.SetText(_text);
        }


        // Public methods                                                                                                           

    }
}
