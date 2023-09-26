using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh.Meshing
{
    public enum GmshAlgorithmMesh2DEnum
    {
        MeshAdapt = 1,
        Automatic = 2,
        InitialMeshOnly = 3,
        Delaunay = 5,
        FrontalDelaunay = 6,
        BAMG = 7,
        FrontalDelaunayQuads = 8,
        PackingOfParallelograms = 9,
        QuasiStructuredQuad = 11
    }
}
