using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Runtime.Serialization;
using CaeResults;
using FileInOut.Output.Calculix;
using System.Xml.Linq;

namespace CaeModel
{
    [Serializable]
    public class Elastic : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private double[][] _youngsPoissonsTemp;         //ISerializable


        // Properties                                                                                                               
        public double[][] YoungsPoissonsTemp
        {
            get { return _youngsPoissonsTemp; }
            set
            {
                _youngsPoissonsTemp = value;
                if (_youngsPoissonsTemp != null)
                {
                    for (int i = 0; i < _youngsPoissonsTemp.Length; i++)
                    {
                        if (_youngsPoissonsTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public Elastic(double[][] youngsPoissonsTemp)
        {
            // The constructor must wotk with E = 0
            _youngsPoissonsTemp = youngsPoissonsTemp;
        }
        public Elastic(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_youngsPoissonsTemp":
                        _youngsPoissonsTemp = (double[][])entry.Value; break;
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
            info.AddValue("_youngsPoissonsTemp", _youngsPoissonsTemp, typeof(double[][]));
        }
    }
}
