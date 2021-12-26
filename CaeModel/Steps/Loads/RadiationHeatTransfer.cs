using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class RadiationHeatTransfer : Load
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private RegionTypeEnum _regionType;
        private bool _cavityRadiation;
        private string _cavityName;
        private double _sinkTemperature;
        private double _emissivity;


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
        public double SinkTemperature { get { return _sinkTemperature; } set { _sinkTemperature = value; } }
        public double Emissivity
        {
            get { return _emissivity; }
            set
            {
                
                if (value < 0) _emissivity = 0;
                else if (value > 1) _emissivity = 1;
                else _emissivity = value;
            }
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
            _sinkTemperature = sinkTemperature;
            _emissivity = emissivity;
        }


        // Methods                                                                                                                  
    }
}
