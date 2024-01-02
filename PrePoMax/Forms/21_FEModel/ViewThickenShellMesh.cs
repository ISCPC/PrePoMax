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
    class ThickenShellMesh
    {
        // Properties                                                                                                               
        public int[] GeometryIds;
        public Selection CreationData;
        public double Thickness;
        public int NumberOfLayers;
        public double Offset;


        // Constructors                                                                                                             
        public ThickenShellMesh()
        {
            GeometryIds = null;
            CreationData = null;
            Thickness = 1;
            NumberOfLayers = 1;
            Offset = 0;
        }
    }


    [Serializable]
    public class ViewThickenShellMesh
    {
        // Variables                                                                                                                
        private ThickenShellMesh _thickenShellMesh;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Thickness")]
        [DescriptionAttribute("Enter teh thickness of the resulting solid mesh.")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double Thickness { get { return _thickenShellMesh.Thickness; } set { _thickenShellMesh.Thickness = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Number of layers")]
        [DescriptionAttribute("Enter the number of finite element layers of the resulting solid mesh.")]
        public int NumberOfLayers
        {
            get { return _thickenShellMesh.NumberOfLayers; }
            set { _thickenShellMesh.NumberOfLayers = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Offset")]
        [DescriptionAttribute("Enter the offset of the resulting solid mesh.")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double Offset { get { return _thickenShellMesh.Offset; } set { _thickenShellMesh.Offset = value; } }
        //
        [Browsable(false)]
        public Selection CreationData { get { return _thickenShellMesh.CreationData; } set { _thickenShellMesh.CreationData = value; } }
        //
        [Browsable(false)]
        public int[] GeometryIds { get { return _thickenShellMesh.GeometryIds; } set { _thickenShellMesh.GeometryIds = value; } }


        // Constructors                                                                                                             
        public ViewThickenShellMesh()
        {
            _thickenShellMesh = new ThickenShellMesh();
            _dctd = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  




    }
}
