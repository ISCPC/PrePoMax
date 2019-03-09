using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Kitware.VTK;

namespace vtkControl
{
    public enum DataFieldType
    {
        Static,
        Modal,
        Buckling
    }
    class vtkMaxStatusBlock : vtkMaxTextBackgroundWidget
    {
        // Variables                                                                                                                
        private string _text;
        private string _name;
        private DateTime _dateTime;
        private float _analysisTime;
        private float _animationScaleFactor;
        private float _deformationScaleFactor;
        private DataFieldType _fieldType;
        

        // Properties                                                                                                               
        public string Name { get { return _name; } set { _name = value; SetText(); } }
        public DateTime DateTime { get { return _dateTime; } set { _dateTime = value; SetText(); } }
        public float AnalysisTime { get { return _analysisTime; } set { _analysisTime = value; SetText(); } }
        public float DeformationScaleFactor { get { return _deformationScaleFactor; } set { _deformationScaleFactor = value; SetText(); } }
        public float AnimationScaleFactor { get { return _animationScaleFactor; } set { _animationScaleFactor = value; SetText(); } }
        public DataFieldType FieldType { get { return _fieldType; } set { _fieldType = value; SetText(); } }


        // Constructors                                                                                                             
        public vtkMaxStatusBlock()
        {
            _text = "text";
            _name = "name";
            _dateTime = DateTime.Now;
            _deformationScaleFactor = 1;
            _animationScaleFactor = -1;

            // Text property
            vtkTextProperty textProperty = vtkTextProperty.New();
            textProperty.SetFontFamilyToArial();
            textProperty.SetFontSize(16);
            textProperty.SetColor(0, 0, 0);
            textProperty.SetLineOffset(-Math.Round(textProperty.GetFontSize() / 5.0));
            textProperty.SetLineSpacing(1.2);
            this.SetTextProperty(textProperty);


            // Border representation
            vtkProperty2D borderProperty = vtkProperty2D.New();
            borderProperty.SetColor(0, 0, 0);
            this.SetBorderProperty(borderProperty);
        }


        // Private methods                                                                                                          
        private void SetText()
        {
            string sysUIFormat = System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;

            _text = "Name: " + _name + "   ";
            _text += "Date: " + _dateTime.ToString(sysUIFormat) + "   Time: " + _dateTime.ToString("HH:mm:ss") + Environment.NewLine;

            if (_fieldType == DataFieldType.Static) _text += "Step: Static   Analysis time: " + _analysisTime.ToString();
            else if(_fieldType == DataFieldType.Modal) _text += "Step: Frequency   Eigen frequency: " + _analysisTime.ToString();
            else if (_fieldType == DataFieldType.Buckling) _text += "Step: Buckling   Buckling factor: " + _analysisTime.ToString();

            if (_animationScaleFactor >= 0) _text += "    Animation scale factor: " + _animationScaleFactor.ToString();
            _text += Environment.NewLine;
            _text += "Deformation scale factor: " + _deformationScaleFactor.ToString();

            this.SetText(_text);
        }

        // Public methods                                                                                                           
        
    }
}
