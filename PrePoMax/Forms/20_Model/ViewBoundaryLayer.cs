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
        public int[] GeometryIds;
        public Selection CreationData;
        public double Thickness;


        // Constructors                                                                                                             
        public BoundaryLayer()
        {
            GeometryIds = null;
            CreationData = null;
            Thickness = 0.1;
        }
    }


    [Serializable]
    public class ViewBoundaryLayer
    {
        // Variables                                                                                                                
        private BoundaryLayer _boundaryLayer;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Layer thickness")]
        [DescriptionAttribute("Thickness of the boundary layer.")]
        [TypeConverter(typeof(CaeGlobals.StringLengthConverter))]
        public double Thickness { get { return _boundaryLayer.Thickness; } set { _boundaryLayer.Thickness = value; } }

        [Browsable(false)]
        public Selection CreationData { get { return _boundaryLayer.CreationData; } set { _boundaryLayer.CreationData = value; } }

        [Browsable(false)]
        public int[] GeometryIds { get { return _boundaryLayer.GeometryIds; } set { _boundaryLayer.GeometryIds = value; } }


        // Constructors                                                                                                             
        public ViewBoundaryLayer()
        {
            _boundaryLayer = new BoundaryLayer();
            _dctd = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  




    }
}
