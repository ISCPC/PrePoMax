using DynamicTypeDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh.Meshing
{
    /* Optimization of a linear mesh
    Netgen
    HighOrder
    HighOrderElastic
    HighOrderFastCurving
    Laplace2D
    Relocate2D
    Relocate3D
    QuadQuasiStructured
    UntangleMeshGeometry
    */
    [Serializable]
    public enum GmshOptimizeFirstOrderShellEnum
    {
        [StandardValue("None", DisplayName = "None")]
        None,
        [StandardValue("Laplace2D", DisplayName = "Laplace 2D")]
        Laplace2D,
        [StandardValue("Relocate2D", DisplayName = "Relocate 2D")]
        Relocate2D,
        [StandardValue("QuadQuasiStructured", DisplayName = "Quasi-structured quad")]
        QuadQuasiStructured,
    }
}
