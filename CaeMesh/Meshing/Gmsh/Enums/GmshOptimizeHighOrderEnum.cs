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
    public enum GmshOptimizeHighOrderEnum
    {
        [StandardValue("None", DisplayName = "None")]
        None,
        [StandardValue("HighOrder", DisplayName = "High order")]
        HighOrder,
        [StandardValue("HighOrderElastic", DisplayName = "High order elastic")]
        HighOrderElastic,
        [StandardValue("HighOrderFastCurving", DisplayName = "High order fast curving")]
        HighOrderFastCurving,
    }
}
