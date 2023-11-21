using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;
using FileInOut.Output.Calculix;
using System.Drawing.Design;

namespace CaeModel
{
    [Serializable]
    public class CLoadData
    {
        // Variables                                                                                                                
        private string _name;
        private string _regionName;
        private RegionTypeEnum _regionType;
        private int _nodeId;
        private double _f1;
        private double _f2;
        private double _f3;
        private double _magnitude;
        private string _amplitudeName;


        // Properties                                                                                                               
        public string Name { get { return _name; } set { _name = value; } }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }
        public double F1 { get { return _f1; } set { _f1 = value; } }
        public double F2 { get { return _f2; } set { _f2 = value; } }
        public double F3 { get { return _f3; } set { _f3 = value; } }
        public double Magnitude { get { return _magnitude; } set { _magnitude = value; } }
        public string AmplitudeName { get { return _amplitudeName; } set { _amplitudeName = value; } }
        public double GetComponent(int direction)
        {
            if (direction == 0) return F1;
            else if (direction == 1) return F2;
            else return F3;
        }


        // Constructors                                                                                                             
        public CLoadData(string name, int nodeId, double f1, double f2, double f3)
            : this(name, null, RegionTypeEnum.NodeId, f1, f2, f3)
        {
            _nodeId = nodeId;
        }
        public CLoadData(string name, string regionName, RegionTypeEnum regionType, double f1, double f2, double f3)
        {
            _name = name;
            _regionName = regionName;
            _regionType = regionType;
            _nodeId = -1;
            //
            double mag = Math.Sqrt(f1 * f1 + f2 * f2 + f3 * f3);
            _f1 = f1;
            _f2 = f2;
            _f3 = f3;
            _magnitude = mag;
        }


        // Methods                                                                                                                  

    }
}
