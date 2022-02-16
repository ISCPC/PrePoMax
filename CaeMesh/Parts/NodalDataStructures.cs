using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public class AvgData
    {
        public Dictionary<int, AvgNodalData> Nodes = new Dictionary<int, AvgNodalData>();
    }
    public class AvgNodalData
    {
        public Dictionary<int, AvgNodalSurfaceData> Surfaces = new Dictionary<int, AvgNodalSurfaceData>();
    }

    public class AvgNodalSurfaceData
    {
        public Dictionary<int, AvgNodalSurfaceElementData> Elements = new Dictionary<int, AvgNodalSurfaceElementData>();
    }

    public class AvgNodalSurfaceElementData
    {
        public List<Tuple<double, Vec3D>> Data = new List<Tuple<double, Vec3D>>();
    }
}
