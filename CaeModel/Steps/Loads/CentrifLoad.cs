using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class CentrifLoad : Load
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double N1 { get; set; }
        public double N2 { get; set; }
        public double N3 { get; set; }
        public double RotationalSpeed2 { get; set; }


        // Constructors                                                                                                             
        public CentrifLoad(string name, string regionName, RegionTypeEnum regionType, double[] point, double[] normal, double rotationalSpeed2)
            : base(name) 
        {
            _regionName = regionName;
            RegionType = regionType;

            X = point[0];
            Y = point[1];
            Z = point[2];

            N1 = normal[0];
            N2 = normal[1];
            N3 = normal[2];

            RotationalSpeed2 = rotationalSpeed2;
        }
        public CentrifLoad(string name, string regionName, RegionTypeEnum regionType)
            : this(name, regionName, regionType, new double[] { 0, 0, 0 }, new double[] { 0, 0, 0 }, 0)
        {
        }


        // Methods                                                                                                                  
    }
}
