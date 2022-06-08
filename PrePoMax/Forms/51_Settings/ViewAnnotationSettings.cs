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
    public class ViewAnnotationSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private AnnotationSettings _annotationSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Color spectrum values                                                                     
        [CategoryAttribute("Design")]
        [OrderedDisplayName(0, 10, "Background type")]
        [DescriptionAttribute("Select the background type.")]
        public AnnotationBackgroundType BackgroundType
        {
            get { return _annotationSettings.BackgroundType; }
            set { _annotationSettings.BackgroundType = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(1, 10, "Draw a border rectangle")]
        [DescriptionAttribute("Draw a border rectangle around the legend.")]
        public bool DrawBorder
        {
            get { return _annotationSettings.DrawBorder; }
            set { _annotationSettings.DrawBorder = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(2, 10, "Number format")]
        [DescriptionAttribute("Select the number format.")]
        public AnnotationNumberFormat LegendNumberFormat
        {
            get { return _annotationSettings.NumberFormat; }
            set { _annotationSettings.NumberFormat = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(3, 10, "Number of significant digits")]
        [DescriptionAttribute("Set the number of significant digits (2 ... 8).")]
        public int NumberOfSignificantDigits
        {
            get { return _annotationSettings.NumberOfSignificantDigits; }
            set { _annotationSettings.NumberOfSignificantDigits = value; }
        }
        //
        [CategoryAttribute("Node annotation")]
        [OrderedDisplayName(0, 10, "Show node id")]
        [DescriptionAttribute("Show node id in the annotation.")]
        public bool ShowNodeId
        {
            get { return _annotationSettings.ShowNodeId; }
            set { _annotationSettings.ShowNodeId = value; }
        }
        //
        [CategoryAttribute("Node annotation")]
        [OrderedDisplayName(1, 10, "Show coordinates")]
        [DescriptionAttribute("Show coordintates in the annotation.")]
        public bool ShowCoordinates
        {
            get { return _annotationSettings.ShowCoordinates; }
            set { _annotationSettings.ShowCoordinates = value; }
        }
        //
        [CategoryAttribute("Edge/Surface annotation")]
        [OrderedDisplayName(0, 10, "Show edge/surface id")]
        [DescriptionAttribute("Show edge/surface id in the annotation.")]
        public bool ShowEdgeSurId
        {
            get { return _annotationSettings.ShowEdgeSurId; }
            set { _annotationSettings.ShowEdgeSurId = value; }
        }
        //
        [CategoryAttribute("Edge/Surface annotation")]
        [OrderedDisplayName(1, 10, "Show edge/surface size")]
        [DescriptionAttribute("Show edge/surface size in the annotation.")]
        public bool ShowEdgeLength
        {
            get { return _annotationSettings.ShowEdgeSurSize; }
            set { _annotationSettings.ShowEdgeSurSize = value; }
        }
        //
        [CategoryAttribute("Edge/Surface annotation")]
        [OrderedDisplayName(2, 10, "Show maximum value")]
        [DescriptionAttribute("Show maximum value in the annotation.")]
        public bool ShowEdgeMax
        {
            get { return _annotationSettings.ShowEdgeSurMax; }
            set { _annotationSettings.ShowEdgeSurMax = value; }
        }
        //
        [CategoryAttribute("Edge/Surface annotation")]
        [OrderedDisplayName(3, 10, "Show minumum value")]
        [DescriptionAttribute("Show minumum value in the annotation.")]
        public bool ShowEdgeMin
        {
            get { return _annotationSettings.ShowEdgeSurMin; }
            set { _annotationSettings.ShowEdgeSurMin = value; }
        }
        //
        [CategoryAttribute("Edge/Surface annotation")]
        [OrderedDisplayName(4, 10, "Show sum value")]
        [DescriptionAttribute("Show summed value by nodes in the annotation.")]
        public bool ShowEdgeSum
        {
            get { return _annotationSettings.ShowEdgeSurSum; }
            set { _annotationSettings.ShowEdgeSurSum = value; }
        }
        //
        [CategoryAttribute("Part annotation")]
        [OrderedDisplayName(0, 10, "Show part name")]
        [DescriptionAttribute("Show part name in the annotation.")]
        public bool ShowPartName
        {
            get { return _annotationSettings.ShowPartName; }
            set { _annotationSettings.ShowPartName = value; }
        }
        //
        [CategoryAttribute("Part annotation")]
        [OrderedDisplayName(1, 10, "Show part id")]
        [DescriptionAttribute("Show part id in the annotation.")]
        public bool ShowPartId
        {
            get { return _annotationSettings.ShowPartId; }
            set { _annotationSettings.ShowPartId = value; }
        }
        //
        [CategoryAttribute("Part annotation")]
        [OrderedDisplayName(2, 10, "Show part type")]
        [DescriptionAttribute("Show part type in the annotation.")]
        public bool ShowPartType
        {
            get { return _annotationSettings.ShowPartType; }
            set { _annotationSettings.ShowPartType = value; }
        }
        //
        [CategoryAttribute("Part annotation")]
        [OrderedDisplayName(3, 10, "Show number of elements")]
        [DescriptionAttribute("Show part number of elements in the annotation.")]
        public bool ShowPartNumberOfElements
        {
            get { return _annotationSettings.ShowPartNumberOfElements; }
            set { _annotationSettings.ShowPartNumberOfElements = value; }
        }
        //
        [CategoryAttribute("Part annotation")]
        [OrderedDisplayName(4, 10, "Show number of nodes")]
        [DescriptionAttribute("Show part number of nodes in the annotation.")]
        public bool ShowPartNumberOfNodes
        {
            get { return _annotationSettings.ShowPartNumberOfNodes; }
            set { _annotationSettings.ShowPartNumberOfNodes = value; }
        }


        // Constructors                                                                               
        public ViewAnnotationSettings(AnnotationSettings annotationSettings)
        {
            _annotationSettings = annotationSettings;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(DrawBorder));
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowNodeId));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowCoordinates));
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeSurId));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeLength));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeMax));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeMin));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowEdgeSum));
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowPartName));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowPartId));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowPartType));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowPartNumberOfElements));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowPartNumberOfNodes));
        }


        // Methods                                                                               
        public ISettings GetBase()
        {
            return _annotationSettings;
        }
        public void Reset()
        {
            _annotationSettings.Reset();
        }
    }

}
