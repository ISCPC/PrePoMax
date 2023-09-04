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
    public class CFlux : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _regionName;                 //ISerializable
        private RegionTypeEnum _regionType;         //ISerializable
        private bool _addFlux;                      //ISerializable
        private EquationContainer _magnitude;       //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public bool AddFlux { get { return _addFlux; } set { _addFlux = value; } }
        public EquationContainer Magnitude { get { return _magnitude; } set { SetMagnitude(value); } }


        // Constructors                                                                                                             
        public CFlux(string name, string regionName, RegionTypeEnum regionType, double magnitude, bool twoD)
            : base(name, twoD) 
        {
            _regionName = regionName;
            RegionType = regionType;
            _addFlux = false;
            Magnitude = new EquationContainer(typeof(StringPowerConverter), magnitude);
        }
        public CFlux(SerializationInfo info, StreamingContext context)
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
                    case "_addFlux":
                    case "<AddFlux>k__BackingField":
                        _addFlux = (bool)entry.Value; break;
                    case "_magnitude":
                    case "<Magnitude>k__BackingField":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueDouble)
                            Magnitude = new EquationContainer(typeof(StringPowerConverter), valueDouble);
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
            info.AddValue("_addFlux", _addFlux, typeof(bool));
            info.AddValue("_magnitude", _magnitude, typeof(EquationContainer));
        }
    }
}
