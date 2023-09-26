using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh.Meshing
{
    public enum GmshAlgorithmRecombineEnum
    {
        None = -1,
        Simple = 0,
        Blossom = 1,
        SimpleFullQuad = 2,
        BlossomFullQuad = 3
    }
}
