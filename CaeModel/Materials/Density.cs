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
    public class Density : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private double[][] _densityTemp;        //ISerializable


        // Properties                                                                                                               
        public double[][] DensityTemp
        {
            get { return _densityTemp; }
            set
            {
                _densityTemp = value;
                if (_densityTemp != null)
                {
                    for (int i = 0; i < _densityTemp.Length; i++)
                    {
                        if (_densityTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public Density(double[][] densityTemp)
        {
            _densityTemp = densityTemp;
        }
        public Density(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_densityTemp":
                        _densityTemp = (double[][])entry.Value; break;
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
            info.AddValue("_densityTemp", _densityTemp, typeof(double[][]));
        }
    }
}
