using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;

namespace CaeModel
{
    [Serializable]
    public class PreTensionLoad : Load
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private double _x;
        private double _y;
        private double _z;
        private double _forceMagnitude;


        // Properties                                                                                                               
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }
        public double ForceMagnitude { get { return _forceMagnitude; } set { _forceMagnitude = value; } }


        // Constructors                                                                                                             
        public PreTensionLoad(string name, string surfaceName, double x, double y, double z, double forceMagnitude)
            : base(name) 
        {
            _surfaceName = surfaceName;
            _x = x;
            _y = y;
            _z = z;
            _forceMagnitude = forceMagnitude;
        }


        // Methods                                                                                                                  
        
    }
}
