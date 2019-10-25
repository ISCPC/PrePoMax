using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octree
{
    public interface IIntersect
    {
        bool Intersect(Plane plane, out double distance);
    }
}
