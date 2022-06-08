using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax.Settings
{
    [Serializable]
    public class ViewPreSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private PreSettings _preSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Selection")]
        [OrderedDisplayName(0, 10, "Primary highlight color")]
        [DescriptionAttribute("Select the primary highlight color.")]
        public Color PrimaryHighlightColor
        {
            get { return _preSettings.PrimaryHighlightColor; }
            set { _preSettings.PrimaryHighlightColor = value; }
        }
        //
        [CategoryAttribute("Selection")]
        [OrderedDisplayName(1, 10, "Secondary highlight color")]
        [DescriptionAttribute("Select the secondary highlight color.")]
        public Color SecondaryHighlightColor
        {
            get { return _preSettings.SecondaryHighlightColor; }
            set { _preSettings.SecondaryHighlightColor = value; }
        }
        //
        [CategoryAttribute("Selection")]
        [OrderedDisplayName(2, 10, "Mouse highlight color")]
        [DescriptionAttribute("Select the mouse highlight color.")]
        public Color MouseHighlightColor
        {
            get { return _preSettings.MouseHighlightColor; }
            set { _preSettings.MouseHighlightColor = value; }
        }
        //
        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(0, 10, "Constraint color")]
        [DescriptionAttribute("Select the constraint symbol color.")]
        public Color ConstraintSymbolColor
        {
            get { return _preSettings.ConstraintSymbolColor; }
            set { _preSettings.ConstraintSymbolColor = value; }
        }
        //
        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(1, 10, "Boundary condition color")]
        [DescriptionAttribute("Select the boundary condition symbol color.")]
        public Color BoundaryConditionSymbolColor
        {
            get { return _preSettings.BoundaryConditionSymbolColor; }
            set { _preSettings.BoundaryConditionSymbolColor = value; }
        }
        //
        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(2, 10, "Load color")]
        [DescriptionAttribute("Select the load symbol color.")]
        public Color LoadSymbolColor
        {
            get { return _preSettings.LoadSymbolColor; }
            set { _preSettings.LoadSymbolColor = value; }
        }
        //
        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(3, 10, "Symbol size")]
        [DescriptionAttribute("Select the symbol size.")]
        [TypeConverter(typeof(CaeGlobals.StringLengthPixelConverter))]
        public int SymbolSize
        {
            get { return _preSettings.SymbolSize; }
            set { _preSettings.SymbolSize = value; }
        }
        //
        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(4, 10, "Node symbol size")]
        [DescriptionAttribute("Select the node symbol size.")]
        [TypeConverter(typeof(CaeGlobals.StringLengthPixelConverter))]
        public int NodeSymbolSize
        {
            get { return _preSettings.NodeSymbolSize; }
            set { _preSettings.NodeSymbolSize = value; }
        }
        //
        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(5, 10, "Draw symbol edges")]
        [DescriptionAttribute("Draw symbol edges.")]
        public bool DrawSymbolEdges
        {
            get { return _preSettings.DrawSymbolEdges; }
            set { _preSettings.DrawSymbolEdges = value; }
        }
        //
        [CategoryAttribute("Color bar")]
        [OrderedDisplayName(0, 10, "Background type")]
        [DescriptionAttribute("Select the background type.")]
        public AnnotationBackgroundType ColorBarBackgroundType
        {
            get { return _preSettings.ColorBarBackgroundType; }
            set { _preSettings.ColorBarBackgroundType = value; }
        }
        //
        [CategoryAttribute("Color bar")]
        [OrderedDisplayName(1, 10, "Draw a border rectangle")]
        [DescriptionAttribute("Draw a border rectangle around the legend.")]
        public bool ColorBarDrawBorder
        {
            get { return _preSettings.ColorBarDrawBorder; }
            set { _preSettings.ColorBarDrawBorder = value; }
        }


        // Constructors                                                                                                             
        public ViewPreSettings(PreSettings preSettings)
        {
            _preSettings = preSettings;
            _dctd = ProviderInstaller.Install(this);
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(DrawSymbolEdges));
        }


        // Methods                                                                                                                  
        public ISettings GetBase()
        {
            return _preSettings;
        }

        public void Reset()
        {
            _preSettings.Reset();
        }
    }

}
