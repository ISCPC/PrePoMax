using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class MembraneSection : Section, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer _offset;         //ISerializable


        // Properties                                                                                                               
        public EquationContainer Offset { get { return _offset; } set { SetOffset(value); } }


        // Constructors                                                                                                             
        public MembraneSection(string name, string materialName, string regionName, RegionTypeEnum regionType, double thickness,
                               bool twoD)
            : base(name, materialName, regionName, regionType, thickness, twoD)
        {
            Offset = new EquationContainer(typeof(StringDoubleConverter), 0);
        }
        public MembraneSection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_offset":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double value)
                            Offset = new EquationContainer(typeof(StringDoubleConverter), value);
                        else
                            SetOffset((EquationContainer)entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetOffset(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _offset, value, null, checkEquation);
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _offset.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_offset", _offset, typeof(EquationContainer));
        }
    }
}
