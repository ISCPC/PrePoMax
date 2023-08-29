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
    public class SpecificHeat : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private double[][] _specificHeatTemp;           //ISerializable


        // Properties                                                                                                               
        public double[][] SpecificHeatTemp
        {
            get { return _specificHeatTemp; }
            set
            {
                _specificHeatTemp = value;
                if (_specificHeatTemp != null)
                {
                    for (int i = 0; i < _specificHeatTemp.Length; i++)
                    {
                        if (_specificHeatTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public SpecificHeat(double[][] specificHeatTemp)
        {
            _specificHeatTemp = specificHeatTemp;
        }
        public SpecificHeat(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_specificHeatTemp":
                        _specificHeatTemp = (double[][])entry.Value; break;
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
            info.AddValue("_specificHeatTemp", _specificHeatTemp, typeof(double[][]));
        }
    }
}
