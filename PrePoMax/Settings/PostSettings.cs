using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public enum ChartNumberFormat
    {
        Scientific,
        General
    }

    [Serializable]
    public enum DeformationScaleFactorType
    {
        [StandardValue("Automatic", Description = "Automatic")]
        Automatic,

        [StandardValue("TrueScale", DisplayName = "True scale", Description = "True scale")]
        TrueScale,

        [StandardValue("Off", DisplayName = "Off", Description = "Off")]
        Off,

        [StandardValue("UserDefined", DisplayName = "User defined", Description = "User defined")]
        UserDefined,
    }


    [Serializable]
    public class PostSettings : ISettings
    {
        // Variables                                                                                                                
        private double _deformationScaleFactor;        
        private ChartNumberFormat _chartNumberFormat;
        private int _numberOfSignificantDigits;
        private bool _drawUndeformedModel;
        private System.Drawing.Color _undeformedModelColor;
        private DeformationScaleFactorType _dsfType;
        private vtkControl.vtkMaxColorSpectrum _colorSpectrum;
        private bool _showMinValueLocation;
        private bool _showMaxValueLocation;


        // Properties                                                                                                               
        public DeformationScaleFactorType DeformationScaleFactorType
        {
            get { return _dsfType; }
            set
            {
                if (_dsfType != value)
                {
                    _dsfType = value;
                    if (_dsfType == PrePoMax.DeformationScaleFactorType.Automatic)
                        _deformationScaleFactor = -1;
                    else if (_dsfType == PrePoMax.DeformationScaleFactorType.TrueScale)
                        _deformationScaleFactor = 1;
                    else if (_dsfType == PrePoMax.DeformationScaleFactorType.Off)
                        _deformationScaleFactor = 0;
                    else if (_dsfType == PrePoMax.DeformationScaleFactorType.UserDefined)
                        _deformationScaleFactor = 1;
                    else throw new NotSupportedException();
                }
            }
        }
        public double DeformationScaleFactorValue 
        {
            get { return _deformationScaleFactor; }
            set 
            {
                _deformationScaleFactor = value;
                if (_deformationScaleFactor < -1) _deformationScaleFactor = -1;
            } 
        }
        public bool DrawUndeformedModel
        {
            get { return _drawUndeformedModel; }
            set { _drawUndeformedModel = value; }
        }
        public System.Drawing.Color UndeformedModelColor
        {
            get { return _undeformedModelColor; }
            set { _undeformedModelColor = value; }
        }
        public ChartNumberFormat ChartNumberFormat { get { return _chartNumberFormat; } set { _chartNumberFormat = value; } }
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
        public bool ShowMinValueLocation { get { return _showMinValueLocation; } set { _showMinValueLocation = value; } }
        public bool ShowMaxValueLocation { get { return _showMaxValueLocation; } set { _showMaxValueLocation = value; } }

        // Constructors                                                                                                             
        public PostSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _deformationScaleFactor = -1;
            _chartNumberFormat = PrePoMax.ChartNumberFormat.Scientific;
            _numberOfSignificantDigits = 4;
            _drawUndeformedModel = true;
            _undeformedModelColor = System.Drawing.Color.FromArgb(128, 128, 128, 128);
            _dsfType = PrePoMax.DeformationScaleFactorType.Automatic;

            _colorSpectrum = new vtkControl.vtkMaxColorSpectrum();

            _showMinValueLocation = false;
            _showMaxValueLocation = true;
        }
        public string GetColorChartNumberFormat()
        {
            string numberformat = "";
            if (_chartNumberFormat == PrePoMax.ChartNumberFormat.General)
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
