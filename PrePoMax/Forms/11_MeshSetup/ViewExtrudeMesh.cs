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
    public class ViewExtrudeMesh : ViewGmshSetupItem
    {
        // Variables                                                                                                                
        private ExtrudeMesh _extrudeMesh;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public ViewExtrudeMesh(ExtrudeMesh extrudeMesh)
        {
            _extrudeMesh = extrudeMesh;
            SetBase(_extrudeMesh);
            //
            _dctd.GetProperty(nameof(AlgorithmMesh3D)).SetIsBrowsable(false);
            //
            _dctd.GetProperty(nameof(OptimizeFirstOrderShell)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(OptimizeFirstOrderSolid)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(OptimizeHighOrder)).SetIsBrowsable(false);
            //
            UpdateVisibility();
        }


        // Methods                                                                                                                  
        public override MeshSetupItem GetBase()
        {
            return _extrudeMesh;
        }
    }
}

