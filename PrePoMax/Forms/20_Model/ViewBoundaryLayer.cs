using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace PrePoMax.Forms
{
    [Serializable]
    class BoundaryLayer
    {
        // Properties                                                                                                               
        public Selection CreationData;
        public double Thickness;


        // Constructors                                                                                                             
        public BoundaryLayer()
        {
            Thickness = 0.1;
            CreationData = null;
        }
    }


    [Serializable]
    public class ViewBoundaryLayer
    {
        // Variables                                                                                                                
        private ItemSetData _itemSetData;
        private BoundaryLayer _boundaryLayer;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Select items")]
        [DescriptionAttribute("Select the surfaces for the boundary layer.")]
        [EditorAttribute(typeof(ItemSetDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ItemSetData ItemSetData
        {
            get { return _itemSetData; }
            set { if (value != _itemSetData) _itemSetData = value; }
        }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Layer thickness")]
        [DescriptionAttribute("Thickness of the boundary layer.")]
        public double Thickness { get { return _boundaryLayer.Thickness; } set { _boundaryLayer.Thickness = value; } }

        [Browsable(false)]
        public Selection CreationData { get { return _boundaryLayer.CreationData; } set { _boundaryLayer.CreationData = value; } }


        // Constructors                                                                                                             
        public ViewBoundaryLayer()
        {
            _boundaryLayer = new BoundaryLayer();
            _itemSetData = new ItemSetData(new int[0]);
            _dctd = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public int[] GetGeometryIds()
        { 
            return _itemSetData.ItemIds;
        }

        
    }
}
