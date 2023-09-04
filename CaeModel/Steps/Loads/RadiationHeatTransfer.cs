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
    public class RadiationHeatTransfer : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _surfaceName;                    //ISerializable
        private RegionTypeEnum _regionType;             //ISerializable
        private bool _cavityRadiation;                  //ISerializable
        private string _cavityName;                     //ISerializable
        private EquationContainer _sinkTemperature;     //ISerializable
        private EquationContainer _emissivity;          //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        //
        public bool CavityRadiation { get { return _cavityRadiation; } set { _cavityRadiation = value; } }
        public string CavityName
        {
            get { return _cavityName; }
            set
            {
                if (value.Length > 3) _cavityName = value.Substring(0, 3);
                else _cavityName = value;
            }
        }
        public EquationContainer SinkTemperature
        {
            get { return _sinkTemperature; }
            set { SetSinkTemperature(value); }
        }
        public EquationContainer Emissivity
        {
            get { return _emissivity; }
            set { SetEmissivity(value); }
        }


        // Constructors                                                                                                             
        public RadiationHeatTransfer(string name, string surfaceName, RegionTypeEnum regionType, double sinkTemperature,
                                     double emissivity, bool twoD)
            : base(name, twoD) 
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
            //
            _cavityRadiation = false;
            _cavityName = null;
            SinkTemperature = new EquationContainer(typeof(StringTemperatureConverter), sinkTemperature);
            Emissivity = new EquationContainer(typeof(StringDoubleConverter), sinkTemperature);
        }
        public RadiationHeatTransfer(SerializationInfo info, StreamingContext context)
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
                    case "_cavityRadiation":
                        _cavityRadiation = (bool)entry.Value; break;
                    case "_cavityName":
                        _cavityName = (string)entry.Value; break;
                    case "_sinkTemperature":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueTemp)
                            SinkTemperature = new EquationContainer(typeof(StringTemperatureConverter), valueTemp);
                        else
                            SetSinkTemperature((EquationContainer)entry.Value, false);
                        break;
                    case "_emissivity":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueEm)
                            Emissivity = new EquationContainer(typeof(StringDoubleConverter), valueEm);
                        else
                            SetEmissivity((EquationContainer)entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetSinkTemperature(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _sinkTemperature, value, null, checkEquation);
        }
        private void SetEmissivity(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _emissivity, value, CheckEmissivity, checkEquation);
        }
        //
        private double CheckEmissivity(double value)
        {
            if (value < 0) return 0;
            else if (value > 1) return 1;
            else return value;
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _sinkTemperature.CheckEquation();
            _emissivity.CheckEquation();
        }

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_surfaceName", _surfaceName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_cavityRadiation", _cavityRadiation, typeof(bool));
            info.AddValue("_cavityName", _cavityName, typeof(string));
            info.AddValue("_sinkTemperature", _sinkTemperature, typeof(EquationContainer));
            info.AddValue("_emissivity", _emissivity, typeof(EquationContainer));
        }
    }
}
