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
    public class FilmHeatTransfer : Load
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private RegionTypeEnum _regionType;
        private double _sinkTemperature;
        private double _filmCoefficient;


        // Properties                                                                                                               
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public double SinkTemperature { get { return _sinkTemperature; } set { _sinkTemperature = value; } }
        public double FilmCoefficient { get { return _filmCoefficient; } set { _filmCoefficient = value; } }


        // Constructors                                                                                                             
        public FilmHeatTransfer(string name, string surfaceName, RegionTypeEnum regionType, double sinkTemperature,
                                double filmCoefficient, bool twoD)
            : base(name, twoD) 
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
            _sinkTemperature = sinkTemperature;
            _filmCoefficient = filmCoefficient;
        }


        // Methods                                                                                                                  
    }
}
