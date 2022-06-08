using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;
using System.Runtime.InteropServices;

namespace PrePoMax
{
    [Serializable]
    public enum AnnotationNumberFormat
    {
        Scientific,
        General
    }

    [Serializable]
    public enum AnnotationBackgroundType
    {
        None,
        White
    }


    [Serializable]
    public class LegendSettings : ISettings
    {
        // Variables                                                                                                                
        private AnnotationNumberFormat _numberFormat;
        private int _numberOfSignificantDigits;
        private vtkControl.vtkMaxColorSpectrum _colorSpectrum;
        private AnnotationBackgroundType _backgroundType;
        private bool _drawBorder;


        // Properties                                                                                                               
        public AnnotationNumberFormat NumberFormat { get { return _numberFormat; } set { _numberFormat = value; } }
        public int NumberOfSignificantDigits
        {
            get { return _numberOfSignificantDigits; }
            set
            {
                _numberOfSignificantDigits = value;
                if (_numberOfSignificantDigits < 2) _numberOfSignificantDigits = 2;
                if (_numberOfSignificantDigits > 8) _numberOfSignificantDigits = 8;
            }
        }
        public vtkControl.vtkMaxColorSpectrum ColorSpectrum { get { return _colorSpectrum; } set { _colorSpectrum = value; } }
        public AnnotationBackgroundType BackgroundType
        {
            get { return _backgroundType; }
            set
            {
                if (value != _backgroundType)
                {
                    _backgroundType = value;
                    if (_backgroundType == AnnotationBackgroundType.White) _drawBorder = true;
                }
            }
        }
        public bool DrawBorder { get { return _drawBorder; } set { _drawBorder = value; } }


        // Constructors                                                                                                             
        public LegendSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _numberFormat = AnnotationNumberFormat.General;
            _numberOfSignificantDigits = 4;
            //
            _colorSpectrum = new vtkControl.vtkMaxColorSpectrum();
            //
            _backgroundType = AnnotationBackgroundType.None;
            _drawBorder = false;
        }
        //
        public string GetColorChartNumberFormat()
        {
            string numberformat;
            if (_numberFormat == AnnotationNumberFormat.General)
            {
                numberformat = "G" + _numberOfSignificantDigits.ToString();
            }
            else
            {
                numberformat = "E" + (_numberOfSignificantDigits - 1).ToString();
            }
            return numberformat;
        }
    }
}
