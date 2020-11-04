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
    public class ViewLegendSettings : ViewSettings, IReset
    {
        // Variables                                                                                                                
        private LegendSettings _legendSettings;
        private DynamicCustomTypeDescriptor _dctd = null;
       

        // Color spectrum settings                                                                   
        [CategoryAttribute("Color spectrum settings")]
        [OrderedDisplayName(0, 10, "Color spectrum type")]
        [DescriptionAttribute("Select the color spectrum type for the visualization of the results.")]
        public ColorSpectrum ColorSpectrum 
        { 
            get 
            {
                if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.CoolWarm) return ColorSpectrum.CoolWarm;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Rainbow) return ColorSpectrum.Rainbow;
                else throw new NotSupportedException();
            }
            set
            {
                if (value == ColorSpectrum.CoolWarm) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.CoolWarm;
                else if (value == ColorSpectrum.Rainbow) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Rainbow;
                else throw new NotSupportedException();
            } 
        }
        //
        [CategoryAttribute("Color spectrum settings")]
        [OrderedDisplayName(1, 10, "Number of discrete colors")]
        [DescriptionAttribute("Set the number of discrete colors (2 ... 24).")]
        public int NumberOfDiscreteColors
        {
            get { return _legendSettings.ColorSpectrum.NumberOfColors; }
            set { _legendSettings.ColorSpectrum.NumberOfColors = value; }
        }


        // Color spectrum values                                                                     
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(0, 10, "Number format")]
        [DescriptionAttribute("Select the number format.")]
        public WidgetNumberFormat LegendNumberFormat { get { return _legendSettings.NumberFormat; } set { _legendSettings.NumberFormat = value; } }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(1, 10, "Number of significant digits")]
        [DescriptionAttribute("Set the number of significant digits (2 ... 8).")]
        public int NumberOfSignificantDigits { get { return _legendSettings.NumberOfSignificantDigits; } set { _legendSettings.NumberOfSignificantDigits = value; } }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(2, 10, "Min/max limit type")]
        [DescriptionAttribute("Select the min/max limit type.")]
        public vtkControl.vtkColorSpectrumMinMaxType ColorSpectrumMinMaxType
        {
            get { return _legendSettings.ColorSpectrum.MinMaxType; }
            set 
            {
                _legendSettings.ColorSpectrum.MinMaxType = value;
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
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(4, 10, "Max value")]
        [DescriptionAttribute("Set the max limit value.")]
        public double ColorSpectrumMax
        {
            get { return _legendSettings.ColorSpectrum.MaxUserValue; }
            set { _legendSettings.ColorSpectrum.MaxUserValue = value; }
        }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(3, 10, "Min value")]
        [DescriptionAttribute("Set the min limit value.")]
        public double ColorSpectrumMin
        {
            get { return _legendSettings.ColorSpectrum.MinUserValue; }
            set { _legendSettings.ColorSpectrum.MinUserValue = value; }
        }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(6, 10, "Max color")]
        [DescriptionAttribute("Select the color for the values above the max value.")]
        public System.Drawing.Color ColorSpectrumMaxColor
        {
            get { return _legendSettings.ColorSpectrum.MaxColor; }
            set { _legendSettings.ColorSpectrum.MaxColor = value; }
        }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(5, 10, "Min color")]
        [DescriptionAttribute("Select the color for the values below the min value.")]
        public System.Drawing.Color ColorSpectrumMinColor
        {
            get { return _legendSettings.ColorSpectrum.MinColor; }
            set { _legendSettings.ColorSpectrum.MinColor = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(0, 10, "Background type")]
        [DescriptionAttribute("Select the background type.")]
        public WidgetBackgroundType BackgroundType
        {
            get { return _legendSettings.BackgroundType; }
            set { _legendSettings.BackgroundType = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(1, 10, "Draw a border rectangle")]
        [DescriptionAttribute("Draw a border rectangle around the legend.")]
        public bool DrawBorder
        {
            get { return _legendSettings.DrawBorder; }
            set { _legendSettings.DrawBorder = value; }
        }




        // Constructors                                                                               
        public ViewLegendSettings(LegendSettings legendSettings)
        {
            _legendSettings = legendSettings;
            _dctd = ProviderInstaller.Install(this);
            //
            ColorSpectrumMinMaxType = _legendSettings.ColorSpectrum.MinMaxType; // add this also to Reset()
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(DrawBorder));
        }


        // Methods                                                                               
        public override ISettings GetBase()
        {
            return _legendSettings;
        }
        public void Reset()
        {
            _legendSettings.Reset();
            //
            ColorSpectrumMinMaxType = _legendSettings.ColorSpectrum.MinMaxType;
        }
    }

}
