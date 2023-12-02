using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeMesh.Meshing;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewTetrahedralGmsh : ViewGmshSetupItem
    {
        // Variables                                                                                                                
        private TetrahedralGmsh _tetrahedronGmsh;


        // Properties                                                                                                               
        

        // Constructors                                                                                                             
        public ViewTetrahedralGmsh(TetrahedralGmsh tetrahedralGmsh)
        {
            _tetrahedronGmsh = tetrahedralGmsh;
            SetBase(_tetrahedronGmsh);
            //
            _dctd.GetProperty(nameof(AlgorithmRecombine)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(TransfiniteAngleDeg)).SetIsBrowsable(false);
            //
            _dctd.GetProperty(nameof(ElementSizeType)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(ElementScaleFactor)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(NumberOfElements)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(NormalizedLayerSizes)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(NumOfElementsPerLayer)).SetIsBrowsable(false);
            //
            UpdateVisibility();
        }


        // Methods                                                                                                                  
        public override MeshSetupItem GetBase()
        {
            return _tetrahedronGmsh;
        }
    }
}

