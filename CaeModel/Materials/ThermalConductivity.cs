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
    public class ThermalConductivity : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private double[][] _thermalConductivityTemp;        


        // Properties                                                                                                               
        public double[][] ThermalConductivityTemp
        {
            get { return _thermalConductivityTemp; }
            set
            {
                _thermalConductivityTemp = value;
                if (_thermalConductivityTemp != null)
                {
                    for (int i = 0; i < _thermalConductivityTemp.Length; i++)
                    {
                        if (_thermalConductivityTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public ThermalConductivity(double[][] thermalConductivityTemp)
        {
            _thermalConductivityTemp = thermalConductivityTemp;
        }
        public ThermalConductivity(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_thermalConductivityTemp":
                        _thermalConductivityTemp = (double[][])entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_thermalConductivityTemp", _thermalConductivityTemp, typeof(double[][]));
        }
    }
}
