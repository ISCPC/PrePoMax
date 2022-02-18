using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class AvgData
    {
        public Dictionary<int, AvgNodalData> Nodes = new Dictionary<int, AvgNodalData>();


        // Methods
        public void AddRange(Dictionary<int, AvgNodalData> nodes)
        {
            foreach (var entry in nodes) Nodes.Add(entry.Key, entry.Value);
        }
        public Dictionary<int, double> GetAveragedValues()
        {
            Dictionary<int, double> values = new Dictionary<int, double>();
            double value;
            double areaSum;
            //
            foreach (var nodeEntry in Nodes)
            {
                areaSum = 0;
                value = 0;
                foreach (var surfaceEntry in nodeEntry.Value.Surfaces)
                {
                    foreach (var elementEntry in surfaceEntry.Value.Elements)
                    {
                        foreach (var tupleEntry in elementEntry.Value.Data)
                        {
                            if (tupleEntry.Item1 != 0)
                            {
                                value += tupleEntry.Item1 * tupleEntry.Item2;
                                areaSum += tupleEntry.Item2;
                            }
                        }
                    }
                }
                if (areaSum != 0)
                {
                    value /= areaSum;
                    values.Add(nodeEntry.Key, value);
                }
            }
            //
            return values;
        }
    }
    [Serializable]
    public class AvgNodalData
    {
        public Dictionary<int, AvgNodalSurfaceData> Surfaces = new Dictionary<int, AvgNodalSurfaceData>();
    }
    [Serializable]
    public class AvgNodalSurfaceData
    {
        public Dictionary<int, AvgNodalSurfaceElementData> Elements = new Dictionary<int, AvgNodalSurfaceElementData>();
    }
    [Serializable]
    public class AvgNodalSurfaceElementData
    {
        // value, area, normal
        public List<Tuple<double, double, Vec3D>> Data = new List<Tuple<double, double, Vec3D>>();
    }
    public class AvgEntryData
    {
        public int[] NodeIds;
        public int SurfaceId;
        public int ElementId;
        public double Area;
        public Vec3D Normal;
    }
}
