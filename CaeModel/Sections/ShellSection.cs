using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class ShellSection : Section
    {
        // Variables                                                                                                                
        private double _thickness;
        private double _offset;


        // Properties                                                                                                               
        public double Thickness 
        {
            get { return _thickness; } 
            set 
            {
                _thickness = value;
                if (_thickness <= 0) _thickness = 0.001;
            }
        }
        public double Offset { get { return _offset; } set { _offset = value; } }


        // Constructors                                                                                                             
        public ShellSection(string name, string materialName, string regionName, RegionTypeEnum regionType, double thickness,
                            bool twoD)
            : base(name, materialName, regionName, regionType, twoD)
        {
            Thickness = thickness;
            _offset = 0;
        }
    }
}
