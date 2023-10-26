using DynamicTypeDescriptor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh.Meshing
{
    [Serializable]
    public enum GmshAlgorithmMesh2DEnum
    {
        [StandardValue("MeshAdapt", DisplayName = "MeshAdapt")]
        MeshAdapt = 1,
        [StandardValue("Automatic", DisplayName = "Automatic")]
        Automatic = 2,
        [StandardValue("InitialMeshOnly", DisplayName = "Initial mesh only")]
        InitialMeshOnly = 3,
        [StandardValue("Delaunay", DisplayName = "Delaunay")]
        Delaunay = 5,
        [StandardValue("FrontalDelaunay", DisplayName = "Frontal-Delaunay")]
        FrontalDelaunay = 6,
        //[StandardValue("BAMG", DisplayName = "BAMG")]
        //BAMG = 7,
        [StandardValue("FrontalDelaunayQuads", DisplayName = "Frontal-Delaunay for quads")]
        FrontalDelaunayQuads = 8,
        [StandardValue("PackingOfParallelograms", DisplayName = "Packing of parallelograms")]
        PackingOfParallelograms = 9,
        [StandardValue("QuasiStructuredQuad", DisplayName = "Quasi-structured quad")]
        QuasiStructuredQuad = 11
    }
}
