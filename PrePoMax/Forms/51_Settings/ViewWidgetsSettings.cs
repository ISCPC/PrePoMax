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
        [CategoryAttribute("Node widget")]
        [OrderedDisplayName(0, 10, "Show node id")]
        [DescriptionAttribute("Show node id in the widget.")]
        public bool ShowNodeId
        {
            get { return _widgetSettings.ShowNodeId; }
            set { _widgetSettings.ShowNodeId = value; }
        }
        //
        [CategoryAttribute("Node widget")]
        [OrderedDisplayName(1, 10, "Show coordinates")]
        [DescriptionAttribute("Show coordintates in the widget.")]
        public bool ShowCoordinates
        {
            get { return _widgetSettings.ShowCoordinates; }
            set { _widgetSettings.ShowCoordinates = value; }
        }
        //
        [CategoryAttribute("Edge widget")]
        [OrderedDisplayName(0, 10, "Show edge id")]
        [DescriptionAttribute("Show edge id in the widget.")]
        public bool ShowEdgeId
        {
            get { return _widgetSettings.ShowEdgeId; }
            set { _widgetSettings.ShowEdgeId = value; }
        }
        //
        [CategoryAttribute("Edge widget")]
        [OrderedDisplayName(1, 10, "Show edge length")]
        [DescriptionAttribute("Show edge length in the widget.")]
        public bool ShowEdgeLength
        {
            get { return _widgetSettings.ShowEdgeLength; }
            set { _widgetSettings.ShowEdgeLength = value; }
        }
        //
        [CategoryAttribute("Edge widget")]
        [OrderedDisplayName(2, 10, "Show maximum value")]
        [DescriptionAttribute("Show maximum value in the widget.")]
        public bool ShowEdgeMax
        {
            get { return _widgetSettings.ShowEdgeMax; }
            set { _widgetSettings.ShowEdgeMax = value; }
        }
        //
        [CategoryAttribute("Edge widget")]
        [OrderedDisplayName(3, 10, "Show minumum value")]
        [DescriptionAttribute("Show minumum value in the widget.")]
        public bool ShowEdgeMin
        {
            get { return _widgetSettings.ShowEdgeMin; }
            set { _widgetSettings.ShowEdgeMin = value; }
        }
        //
        [CategoryAttribute("Edge widget")]
        [OrderedDisplayName(4, 10, "Show average value")]
        [DescriptionAttribute("Show average value in the widget.")]
        public bool ShowEdgeAvg
        {
            get { return _widgetSettings.ShowEdgeAvg; }
            set { _widgetSettings.ShowEdgeAvg = value; }
        }



        // Constructors                                                                               
        public ViewWidgetsSettings(WidgetsSettings widgetsSettings)
        {
            _widgetSettings = widgetsSettings;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(DrawBorder));
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowNodeId));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowCoordinates));
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeId));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeLength));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeMax));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeMin));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeAvg));
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
