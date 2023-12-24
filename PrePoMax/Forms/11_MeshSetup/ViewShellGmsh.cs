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
    public class ViewShellGmsh : ViewGmshSetupItem
    {
        // Variables                                                                                                                
        private ShellGmsh _shellGmsh;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public ViewShellGmsh(ShellGmsh shellGmsh)
        {
            _shellGmsh = shellGmsh;
            SetBase(_shellGmsh);
            //
            _dctd.GetProperty(nameof(AlgorithmMesh3D)).SetIsBrowsable(false);
            //
            _dctd.GetProperty(nameof(OptimizeFirstOrderSolid)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(OptimizeHighOrder)).SetIsBrowsable(false);
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
            return _shellGmsh;
        }
    }
}

