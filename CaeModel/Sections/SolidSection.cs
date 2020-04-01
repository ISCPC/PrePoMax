using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public enum SolidSectionType
    {
        ThreeDimensional,
        TwoDimensional
    }

    [Serializable]
    public class SolidSection : Section
    {
        // Variables                                                                                                                
        private double _thickness;
        private SolidSectionType _type;
        


        // Properties                                                                                                               
        public double Thickness 
        {
            get { return _thickness; } 
            set 
            {
                _thickness = value;
                if (_thickness > 0) _type = SolidSectionType.TwoDimensional;
                else _type = SolidSectionType.ThreeDimensional;
            }
        }
        public SolidSectionType Type { get { return _type; } }
        


        // Constructors                                                                                                             
        public SolidSection(string name, string materialName, string regionName, RegionTypeEnum regionType, double thickness = 0)
            : base(name, materialName, regionName, regionType)
        {
            Thickness = thickness;  // sets the section type
        }
    }
}
