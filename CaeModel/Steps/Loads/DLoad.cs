using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;

namespace CaeModel
{
    [Serializable]
    public class DLoad : Load
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private double _magnitude;

        // Properties                                                                                                               
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public double Magnitude { get { return _magnitude; } set { _magnitude = value; } }

        // Constructors                                                                                                             
        public DLoad(string name, string surfaceName, double magnitude)
            : base(name) 
        {
            _surfaceName = surfaceName;
            _magnitude = magnitude;
        }

        // Methods                                                                                                                  
    }
}
