using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    public interface IMultiRegion
    {
        string RegionName { get; set; }
        RegionTypeEnum RegionType { get; set; }
    }
}
