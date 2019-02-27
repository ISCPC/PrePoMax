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
    public class Tie : Constraint
    {
        // Variables                                                                                                                
        private string _slaveSurfaceName;
        private string _masterSurfaceName;
        private double _positionTolerance;
        private bool _adjust;


        // Properties                                                                                                               
        public string SlaveSurfaceName { get { return _slaveSurfaceName; } set { _slaveSurfaceName = value; } }
        public string MasterSurfaceName { get { return _masterSurfaceName; } set { _masterSurfaceName = value; } }
        public double PositionTolerance { get { return _positionTolerance; } set { _positionTolerance = value; } }
        public bool Adjust { get { return _adjust; } set { _adjust = value; } }


        // Constructors                                                                                                             
        public Tie(string name, string slaveSurfaceName, string masterSurfaceName)
           : this(name, slaveSurfaceName, masterSurfaceName, 0, true)
        {
        }
        public Tie(string name, string slaveSurfaceName, string masterSurfaceName, double positionTolerance, bool adjust)
            : base(name)
        {
            if (slaveSurfaceName == masterSurfaceName) throw new CaeException("The master and slave surface names must be different.");

            _slaveSurfaceName = slaveSurfaceName;
            _masterSurfaceName = masterSurfaceName;
            _positionTolerance = positionTolerance;
            _adjust = adjust;
        }


        // Methods                                                                                                                  
    }
}
