using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CaeGlobals
{
    public interface IMultiRegion
    {
        string RegionName { get; set; }
        RegionTypeEnum RegionType { get; set; }
        int[] CreationIds { get; set; }
        Selection CreationData { get; set; }
    }
}
