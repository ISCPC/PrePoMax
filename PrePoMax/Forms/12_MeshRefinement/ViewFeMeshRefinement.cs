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
    public class ViewFeMeshRefinement
    {
        // Variables                                                                                                                
        private FeMeshRefinement _meshRefinement;
        private ItemSetData _itemSetData;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the mesh refinement.")]
        public string Name { get { return _meshRefinement.Name; } set { _meshRefinement.Name = value; } }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Select items")]
        [DescriptionAttribute("Select the items for the mesh refinement.")]
        [EditorAttribute(typeof(ItemSetDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ItemSetData ItemSetData
        {
            get { return _itemSetData; }
            set { if (value != _itemSetData) _itemSetData = value; }
        }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Mesh size")]
        [DescriptionAttribute("Local size of the mesh.")]
        public double MeshSize { get { return _meshRefinement.MeshSize; } set { _meshRefinement.MeshSize = value; } }


        // Constructors                                                                                                             
        public ViewFeMeshRefinement(FeMeshRefinement meshRefinement)
        {
            _meshRefinement = meshRefinement;                               // 1 command
            _dctd = ProviderInstaller.Install(this);                        // 2 command
            _itemSetData = new ItemSetData(_meshRefinement.GeometryIds);    // 3 command
        }


        // Methods                                                                                                                  
        public FeMeshRefinement GetBase()
        {
            _meshRefinement.GeometryIds = _itemSetData.ItemIds;     // this must be here, since _itemSetData is changed as pointer
            return _meshRefinement;
        }
    }
}
