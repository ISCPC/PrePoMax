using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public enum PlasticHardening
    {
        Isotropic,
        Kinematic,
        Combined
    }

    [Serializable]
    public class Plastic : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private double[][] _stressStrainTemp;       //ISerializable
        private PlasticHardening _hardening;        //ISerializable


        // Properties                                                                                                               
        public double[][] StressStrainTemp { get { return _stressStrainTemp; } set { _stressStrainTemp = value; } }
        public PlasticHardening Hardening { get { return _hardening; } set {_hardening = value; } }


        // Constructors                                                                                                             
        public Plastic(double[][] stressStrainTemp)
        {
            _stressStrainTemp = stressStrainTemp;
            _hardening = PlasticHardening.Isotropic;
        }
        public Plastic(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_stressStrainTemp":
                    case "<StressStrainTemp>k__BackingField":       // Compatibility for version v1.4.0
                        _stressStrainTemp = (double[][])entry.Value; break;
                    case "_hardening":
                    case "<Hardening>k__BackingField":              // Compatibility for version v1.4.0
                        _hardening = (PlasticHardening)entry.Value; break;
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
            info.AddValue("_stressStrainTemp", _stressStrainTemp, typeof(double[][]));
            info.AddValue("_hardening", _hardening, typeof(PlasticHardening));
        }
    }
}
