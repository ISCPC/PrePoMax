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
    public class BodyFlux : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _regionName;                 //ISerializable
        private RegionTypeEnum _regionType;         //ISerializable
        private EquationContainer _magnitude;       //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public EquationContainer Magnitude { get { return _magnitude; } set { SetMagnitude(value); } }
        

        // Constructors                                                                                                             
        public BodyFlux(string name, string regionName, RegionTypeEnum regionType, double magnitude, bool twoD)
            : base(name, twoD) 
        {
            _regionName = regionName;
            _regionType = regionType;
            Magnitude = new EquationContainer(typeof(StringPowerPerVolumeConverter), magnitude);
        }
        public BodyFlux(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_regionName":
                        _regionName = (string)entry.Value; break;
                    case "_regionType":
                        _regionType = (RegionTypeEnum)entry.Value; break;
                    case "_magnitude":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueDouble)
                            Magnitude = new EquationContainer(typeof(StringPowerPerVolumeConverter), valueDouble);
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
            info.AddValue("_regionName", _regionName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_magnitude", _magnitude, typeof(EquationContainer));
        }
    }
}
