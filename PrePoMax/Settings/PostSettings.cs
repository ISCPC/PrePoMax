using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax
{
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
        private DeformationScaleFactorType _dsfType;
        private double _deformationScaleFactorValue;
        private bool _drawUndeformedModel;
        private Color _undeformedModelColor;
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
                        _deformationScaleFactorValue = -1;
                    else if (_dsfType == PrePoMax.DeformationScaleFactorType.TrueScale)
                        _deformationScaleFactorValue = 1;
                    else if (_dsfType == PrePoMax.DeformationScaleFactorType.Off)
                        _deformationScaleFactorValue = 0;
                    else if (_dsfType == PrePoMax.DeformationScaleFactorType.UserDefined)
                        _deformationScaleFactorValue = 1;
                    else throw new NotSupportedException();
                }
            }
        }
        public double DeformationScaleFactorValue 
        {
            get { return _deformationScaleFactorValue; }
            set 
            {
                _deformationScaleFactorValue = value;
                if (_deformationScaleFactorValue < -1) _deformationScaleFactorValue = -1;
            } 
        }
        public bool DrawUndeformedModel
        {
            get { return _drawUndeformedModel; }
            set { _drawUndeformedModel = value; }
        }
        public Color UndeformedModelColor
        {
            get { return _undeformedModelColor; }
            set { _undeformedModelColor = value; }
        }
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
            _dsfType = PrePoMax.DeformationScaleFactorType.Automatic;
            _deformationScaleFactorValue = -1;
            _drawUndeformedModel = true;
            _undeformedModelColor = System.Drawing.Color.FromArgb(128, 128, 128, 128);
            //
            _showMinValueLocation = false;
            _showMaxValueLocation = true;
        }
        
    }
}
