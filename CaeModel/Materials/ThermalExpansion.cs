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
    public class ThermalExpansion : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private double[][] _thermalExpansionTemp;       //ISerializable
        private double _zeroTemperature;                //ISerializable


        // Properties                                                                                                               
        public double[][] ThermalExpansionTemp
        {
            get { return _thermalExpansionTemp; }
            set
            {
                _thermalExpansionTemp = value;
                if (_thermalExpansionTemp != null)
                {
                    for (int i = 0; i < _thermalExpansionTemp.Length; i++)
                    {
                        if (_thermalExpansionTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }
        public double ZeroTemperature { get { return _zeroTemperature; } set { _zeroTemperature = value; } }


        // Constructors                                                                                                             
        public ThermalExpansion(double[][] thermalExpansionTemp)
        {
            _thermalExpansionTemp = thermalExpansionTemp;
            _zeroTemperature = 20;
        }
        public ThermalExpansion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_thermalExpansionTemp":
                        _thermalExpansionTemp = (double[][])entry.Value; break;
                    case "_zeroTemperature":
                        _zeroTemperature = (double)entry.Value; break;
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
            info.AddValue("_thermalExpansionTemp", _thermalExpansionTemp, typeof(double[][]));
            info.AddValue("_zeroTemperature", _zeroTemperature, typeof(double));
        }
    }
}
