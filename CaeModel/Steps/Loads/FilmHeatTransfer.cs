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
    public class FilmHeatTransfer : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _surfaceName;                    //ISerializable
        private RegionTypeEnum _regionType;             //ISerializable
        private EquationContainer _sinkTemperature;     //ISerializable
        private EquationContainer _filmCoefficient;     //ISerializable


        // Properties                                                                                                               
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public EquationContainer SinkTemperature { get { return _sinkTemperature; } set { _sinkTemperature = value; } }
        public EquationContainer FilmCoefficient { get { return _filmCoefficient; } set { _filmCoefficient = value; } }


        // Constructors                                                                                                             
        public FilmHeatTransfer(string name, string surfaceName, RegionTypeEnum regionType, double sinkTemperature,
                                double filmCoefficient, bool twoD)
            : base(name, twoD) 
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
            SinkTemperature = new EquationContainer(typeof(StringTemperatureConverter), sinkTemperature);
            FilmCoefficient = new EquationContainer(typeof(StringHeatTransferCoefficientConverter), filmCoefficient);
        }
        public FilmHeatTransfer(SerializationInfo info, StreamingContext context)
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
                    case "_sinkTemperature":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueSink)
                            SinkTemperature = new EquationContainer(typeof(StringTemperatureConverter), valueSink);
                        else
                            SinkTemperature = (EquationContainer)entry.Value;
                        break;
                    case "_filmCoefficient":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueCoefficient)
                            FilmCoefficient = new EquationContainer(typeof(StringHeatTransferCoefficientConverter),
                                                                        valueCoefficient);
                        else
                            FilmCoefficient = (EquationContainer)entry.Value;
                        break;
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
            info.AddValue("_surfaceName", _surfaceName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_sinkTemperature", _sinkTemperature, typeof(EquationContainer));
            info.AddValue("_filmCoefficient", _filmCoefficient, typeof(EquationContainer));
        }
    }
}
