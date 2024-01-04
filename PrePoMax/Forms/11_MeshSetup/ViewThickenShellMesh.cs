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
    public class ViewThickenShellMesh : ViewMeshSetupItem
    {
        // Variables                                                                                                                
        private ThickenShellMesh _thickenShellMesh;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the mesh setup item.")]
        public override string Name { get { return _thickenShellMesh.Name; } set { _thickenShellMesh.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Thickness")]
        [DescriptionAttribute("Enter teh thickness of the resulting solid mesh.")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double Thickness { get { return _thickenShellMesh.Thickness; } set { _thickenShellMesh.Thickness = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Number of layers")]
        [DescriptionAttribute("Enter the number of finite element layers of the resulting solid mesh.")]
        public int NumberOfLayers
        {
            get { return _thickenShellMesh.NumberOfLayers; }
            set { _thickenShellMesh.NumberOfLayers = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Offset")]
        [DescriptionAttribute("Enter the offset of the resulting solid mesh.")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double Offset { get { return _thickenShellMesh.Offset; } set { _thickenShellMesh.Offset = value; } }
        

        // Constructors                                                                                                             
        public ViewThickenShellMesh(ThickenShellMesh thickenShellMesh)
        {
            _thickenShellMesh = thickenShellMesh;               // 1 command
            _dctd = ProviderInstaller.Install(this);            // 2 command
        }


        // Methods                                                                                                                  
        public override MeshSetupItem GetBase()
        {
            return _thickenShellMesh;
        }



    }
}
