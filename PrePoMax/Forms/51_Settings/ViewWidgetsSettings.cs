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
    public class ViewWidgetsSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private WidgetsSettings _widgetSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Color spectrum values                                                                     
        [CategoryAttribute("Design")]
        [OrderedDisplayName(0, 10, "Background type")]
        [DescriptionAttribute("Select the background type.")]
        public WidgetBackgroundType BackgroundType
        {
            get { return _widgetSettings.BackgroundType; }
            set { _widgetSettings.BackgroundType = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(1, 10, "Draw a border rectangle")]
        [DescriptionAttribute("Draw a border rectangle around the legend.")]
        public bool DrawBorder
        {
            get { return _widgetSettings.DrawBorder; }
            set { _widgetSettings.DrawBorder = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(2, 10, "Number format")]
        [DescriptionAttribute("Select the number format.")]
        public WidgetNumberFormat LegendNumberFormat
        {
            get { return _widgetSettings.NumberFormat; }
            set { _widgetSettings.NumberFormat = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(3, 10, "Number of significant digits")]
        [DescriptionAttribute("Set the number of significant digits (2 ... 8).")]
        public int NumberOfSignificantDigits
        {
            get { return _widgetSettings.NumberOfSignificantDigits; }
            set { _widgetSettings.NumberOfSignificantDigits = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Show node id")]
        [DescriptionAttribute("Show node id in the widget.")]
        public bool ShowNodeId
        {
            get { return _widgetSettings.ShowNodeId; }
            set { _widgetSettings.ShowNodeId = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Show coordinates")]
        [DescriptionAttribute("Show coordintates in the widget.")]
        public bool ShowCoordinates
        {
            get { return _widgetSettings.ShowCoordinates; }
            set { _widgetSettings.ShowCoordinates = value; }
        }
        


        // Constructors                                                                               
        public ViewWidgetsSettings(WidgetsSettings widgetsSettings)
        {
            _widgetSettings = widgetsSettings;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(DrawBorder));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowCoordinates));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowNodeId));
        }


        // Methods                                                                               
        public ISettings GetBase()
        {
            return _widgetSettings;
        }
        public void Reset()
        {
            _widgetSettings.Reset();
        }
    }

}
