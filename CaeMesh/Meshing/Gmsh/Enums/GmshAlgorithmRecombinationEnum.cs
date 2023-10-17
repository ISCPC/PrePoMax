using DynamicTypeDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh.Meshing
{
    [Serializable]
    public enum GmshAlgorithmRecombineEnum
    {
        None = -1,
        Simple = 0,
        Blossom = 1,
        [StandardValue("SimpleFullQuad", DisplayName = "Simple full-quad")]
        SimpleFullQuad = 2,
        [StandardValue("BlossomFullQuad", DisplayName = "Blossom full-quad")]
        BlossomFullQuad = 3
    }
}
