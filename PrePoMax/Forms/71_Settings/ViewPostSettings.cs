using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Settings
{
    [Serializable]
    public enum ColorSpectrum
    {
        [StandardValue("CoolWarm", DisplayName = "Cool-warm", Description = "Cool-warm")]
        CoolWarm,

        [StandardValue("Rainbow", DisplayName = "Rainbow", Description = "Rainbow")]
        Rainbow
    }

    [Serializable]
    public class ViewPostSettings : ViewSettings, IReset
    {
        // Variables                                                                                                                
        private PostSettings _postSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Deformation                                                                               
        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(0, 10, "Deformation scale factor")]
        [DescriptionAttribute("Select the deformation scale factor type.")]
        public DeformationScaleFactorType DeformationScaleFactorType
        {
            get { return _postSettings.DeformationScaleFactorType; }
            set
            {
                _postSettings.DeformationScaleFactorType = value;

                if (value == PrePoMax.DeformationScaleFactorType.Automatic)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("DeformationScaleFactorValue");
                    cpd.SetIsBrowsable(false);
                }
                else if (value == PrePoMax.DeformationScaleFactorType.TrueScale)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("DeformationScaleFactorValue");
                    cpd.SetIsBrowsable(false);
                }
                else if (value == PrePoMax.DeformationScaleFactorType.Off)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("DeformationScaleFactorValue");
                    cpd.SetIsBrowsable(false);
                }
                else if (value == PrePoMax.DeformationScaleFactorType.UserDefined)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("DeformationScaleFactorValue");
                    cpd.SetIsBrowsable(true);
                }
                else throw new NotSupportedException();
            }
        }

        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(1, 10, "Value")]
        [DescriptionAttribute("Select the deformation scale factor value.")]
        public double DeformationScaleFactorValue 
        { 
            get { return _postSettings.DeformationScaleFactorValue; } 
            set 
            {
                _postSettings.DeformationScaleFactorValue = value;
                if (_postSettings.DeformationScaleFactorValue < 0) _postSettings.DeformationScaleFactorValue = 0;
            } 
        }

        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(2, 10, "Draw undeformed model")]
        [DescriptionAttribute("Draw undeformed model.")]
        public bool DrawUndeformedModel
        {
            get { return _postSettings.DrawUndeformedModel; }
            set
            {
                _postSettings.DrawUndeformedModel = value;

                CustomPropertyDescriptor cpd;
                cpd = _dctd.GetProperty("UndeformedModelColor");
                cpd.SetIsBrowsable(_postSettings.DrawUndeformedModel);
            }
        }

        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(3, 10, "Undeformed model color")]
        [DescriptionAttribute("Set the color of the undeformed model.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        public System.Drawing.Color UndeformedModelColor { get { return _postSettings.UndeformedModelColor; } set { _postSettings.UndeformedModelColor = value; } }


        // Color spectrum settings                                                                   
        [CategoryAttribute("Color spectrum settings")]
        [OrderedDisplayName(0, 10, "Color spectrum type")]
        [DescriptionAttribute("Select the color spectrum type for the visualization of the results.")]
        public ColorSpectrum ColorSpectrum 
        { 
            get 
            {
                if (_postSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.CoolWarm) return PrePoMax.Settings.ColorSpectrum.CoolWarm;
                else if (_postSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Rainbow) return PrePoMax.Settings.ColorSpectrum.Rainbow;
                else throw new NotSupportedException();
            }
            set
            {
                if (value == PrePoMax.Settings.ColorSpectrum.CoolWarm) _postSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.CoolWarm;
                else if (value == PrePoMax.Settings.ColorSpectrum.Rainbow) _postSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Rainbow;
                else throw new NotSupportedException();
            } 
        }

        [CategoryAttribute("Color spectrum settings")]
        [OrderedDisplayName(1, 10, "Number of discrete colors")]
        [DescriptionAttribute("Set the number of discrete colors (2 ... 24).")]
        public int NumberOfDiscreteColors
        {
            get { return _postSettings.ColorSpectrum.NumberOfColors; }
            set { _postSettings.ColorSpectrum.NumberOfColors = value; }
        }


        // Color spectrum values                                                                     
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(0, 10, "Number format")]
        [DescriptionAttribute("Select the number format.")]
        public ChartNumberFormat ChartNumberFormat { get { return _postSettings.ChartNumberFormat; } set { _postSettings.ChartNumberFormat = value; } }

        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(1, 10, "Number of significant digits")]
        [DescriptionAttribute("Set the number of significant digits (2 ... 8).")]
        public int NumberOfSignificantDigits { get { return _postSettings.NumberOfSignificantDigits; } set { _postSettings.NumberOfSignificantDigits = value; } }

        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(2, 10, "Min/max limit type")]
        [DescriptionAttribute("Select the min/max limit type.")]
        public vtkControl.vtkColorSpectrumMinMaxType ColorSpectrumMinMaxType
        {
            get { return _postSettings.ColorSpectrum.MinMaxType; }
            set 
            {
                _postSettings.ColorSpectrum.MinMaxType = value;
                if (value == vtkControl.vtkColorSpectrumMinMaxType.Automatic)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("ColorSpectrumMin");
                    cpd.SetIsBrowsable(false);
                    cpd = _dctd.GetProperty("ColorSpectrumMax");
                    cpd.SetIsBrowsable(false);
                    cpd = _dctd.GetProperty("ColorSpectrumMinColor");
                    cpd.SetIsBrowsable(false);
                    cpd = _dctd.GetProperty("ColorSpectrumMaxColor");
                    cpd.SetIsBrowsable(false);
                }
                else 
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("ColorSpectrumMin");
                    cpd.SetIsBrowsable(true);
                    cpd = _dctd.GetProperty("ColorSpectrumMax");
                    cpd.SetIsBrowsable(true);
                    cpd = _dctd.GetProperty("ColorSpectrumMinColor");
                    cpd.SetIsBrowsable(true);
                    cpd = _dctd.GetProperty("ColorSpectrumMaxColor");
                    cpd.SetIsBrowsable(true);
                }
            }
        }

        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(4, 10, "Max value")]
        [DescriptionAttribute("Set the max limit value.")]
        public double ColorSpectrumMax
        {
            get { return _postSettings.ColorSpectrum.MaxUserValue; }
            set { _postSettings.ColorSpectrum.MaxUserValue = value; }
        }

        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(3, 10, "Min value")]
        [DescriptionAttribute("Set the min limit value.")]
        public double ColorSpectrumMin
        {
            get { return _postSettings.ColorSpectrum.MinUserValue; }
            set { _postSettings.ColorSpectrum.MinUserValue = value; }
        }

        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(6, 10, "Max color")]
        [DescriptionAttribute("Select the color for the values above the max value.")]
        public System.Drawing.Color ColorSpectrumMaxColor
        {
            get { return _postSettings.ColorSpectrum.MaxColor; }
            set { _postSettings.ColorSpectrum.MaxColor = value; }
        }

        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(5, 10, "Min color")]
        [DescriptionAttribute("Select the color for the values below the min value.")]
        public System.Drawing.Color ColorSpectrumMinColor
        {
            get { return _postSettings.ColorSpectrum.MinColor; }
            set { _postSettings.ColorSpectrum.MinColor = value; }
        }

        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(7, 10, "Show max value location")]
        [DescriptionAttribute("Show max value location.")]
        public bool ShowMaxValueLocation
        {
            get { return _postSettings.ShowMaxValueLocation; }
            set { _postSettings.ShowMaxValueLocation = value; }
        }

        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(8, 10, "Show min value location")]
        [DescriptionAttribute("Show min value location.")]
        public bool ShowMinValueLocation
        {
            get { return _postSettings.ShowMinValueLocation; }
            set { _postSettings.ShowMinValueLocation = value; }
        }


        // Constructors                                                                               
        public ViewPostSettings(PostSettings postSettings)
        {
            _postSettings = postSettings;
            _dctd = ProviderInstaller.Install(this);
            //
            DeformationScaleFactorType = _postSettings.DeformationScaleFactorType;      // add this also to Reset()
            ColorSpectrumMinMaxType = _postSettings.ColorSpectrum.MinMaxType;           // add this also to Reset()
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo("ShowMinValueLocation");
            _dctd.RenameBooleanPropertyToYesNo("ShowMaxValueLocation");
            _dctd.RenameBooleanPropertyToYesNo("DrawUndeformedModel");
        }

        // Methods                                                                               
        public override ISettings GetBase()
        {
            return _postSettings;
        }
        public void Reset()
        {
            _postSettings.Reset();

            DeformationScaleFactorType = _postSettings.DeformationScaleFactorType;
            ColorSpectrumMinMaxType = _postSettings.ColorSpectrum.MinMaxType;
        }
    }

}
