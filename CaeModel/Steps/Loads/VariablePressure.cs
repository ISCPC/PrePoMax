using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;

namespace CaeModel
{
    [Serializable]
    public abstract class VariablePressure : Load, ISerializable
    {
        // Variables                                                                                                                
        protected string _surfaceName;          //ISerializable
        protected RegionTypeEnum _regionType;   //ISerializable


        // Properties                                                                                                               
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        

        // Constructors                                                                                                             
        public VariablePressure(string name, string regionName, RegionTypeEnum regionType, bool twoD, bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg)
        {
            _surfaceName = regionName;
            _regionType = regionType;
        }
        public VariablePressure(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_surfaceName":
                        _surfaceName = (string)entry.Value; break;
                    case "_regionType":
                    case "VariablePressure+_regionType":
                        _regionType = (RegionTypeEnum)entry.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        // Methods                                                                                                                  
        public abstract double GetPressureForPoint(double[] point);

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_surfaceName", _surfaceName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
        }
    }
}
