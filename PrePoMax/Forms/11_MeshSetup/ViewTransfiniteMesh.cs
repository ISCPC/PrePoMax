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
    public class ViewTransfiniteMesh : ViewGmshSetupItem
    {
        // Variables                                                                                                                
        private TransfiniteMesh _transfiniteMesh;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Recombine")]
        [DescriptionAttribute("Apply recombination of triangles into quads.")]
        public bool Recombine
        {
            get { return AlgorithmRecombine != GmshAlgorithmRecombineEnum.None; }
            set
            {
                if (value) _gmshSetupItem.AlgorithmRecombine = GmshAlgorithmRecombineEnum.Simple;
                else _gmshSetupItem.AlgorithmRecombine = GmshAlgorithmRecombineEnum.None;
            }
        }
       

        // Constructors                                                                                                             
        public ViewTransfiniteMesh(TransfiniteMesh transfiniteMesh)
        {
            _transfiniteMesh = transfiniteMesh;
            SetBase(_transfiniteMesh);
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(Recombine));
            //
            _dctd.GetProperty(nameof(AlgorithmMesh2D)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(AlgorithmMesh3D)).SetIsBrowsable(false);
            //
            _dctd.GetProperty(nameof(AlgorithmRecombine)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(RecombineMinQuality)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(TransfiniteThreeSided)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(TransfiniteFourSided)).SetIsBrowsable(false);
            //
            _dctd.GetProperty(nameof(ElementSizeType)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(NumberOfLayers)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(ElementScaleFactor)).SetIsBrowsable(false);
            //
            UpdateVisibility();
        }


        // Methods                                                                                                                  
        public override MeshSetupItem GetBase()
        {
            return _transfiniteMesh;
        }
    }
}

