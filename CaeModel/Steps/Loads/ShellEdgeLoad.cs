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
    public class ShellEdgeLoad : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _surfaceName;            //ISerializable
        private RegionTypeEnum _regionType;     //ISerializable
        private EquationContainer _magnitude;   //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public EquationContainer Magnitude { get { return _magnitude; } set { SetMagnitude(value); } }


        // Constructors                                                                                                             
        public ShellEdgeLoad(string name, string surfaceName, RegionTypeEnum regionType, double magnitude, bool twoD,
                             bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg) 
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
            Magnitude = new EquationContainer(typeof(StringForcePerLengthConverter), magnitude);
        }
        public ShellEdgeLoad(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_surfaceName":
                        _surfaceName = (string)entry.Value; break;
                    case "_regionType":
                        _regionType = (RegionTypeEnum)entry.Value; break;
                    case "_magnitude":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueDouble)
                            Magnitude = new EquationContainer(typeof(StringForcePerLengthConverter), valueDouble);
                        else
                            SetMagnitude((EquationContainer)entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetMagnitude(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _magnitude, value, null, checkEquation);
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _magnitude.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_surfaceName", _surfaceName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_magnitude", _magnitude, typeof(EquationContainer));
        }
    }
}
