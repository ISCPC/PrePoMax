using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;

namespace CaeModel
{
    [Serializable]
    public class STLoad : Load
    {
        // Variables                                                                                                                
        private string _surfaceName;

        // Properties                                                                                                               
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public double F1 { get; set; }
        public double F2 { get; set; }
        public double F3 { get; set; }

        // Constructors                                                                                                             
        public STLoad(string name, string surfaceName, double f1, double f2, double f3)
            : base(name) 
        {
            _surfaceName = surfaceName;
            F1 = f1;
            F2 = f2;
            F3 = f3;
        }

        // Methods                                                                                                                  
        
    }
}
