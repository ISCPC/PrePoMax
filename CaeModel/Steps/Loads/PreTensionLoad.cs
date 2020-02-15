using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using DynamicTypeDescriptor;

namespace CaeModel
{
    [Serializable]
    public enum PreTensionLoadType
    {
        [StandardValue("Force", Description = "Pre-tension by force.")]
        Force = 0,
        [StandardValue("Displacement", Description = "Pre-tension by displacement.")]
        Displacement = 1
    }


    [Serializable]
    public class PreTensionLoad : Load
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private bool _autoComputeDirection;
        private double _x;
        private double _y;
        private double _z;
        private PreTensionLoadType _type;
        private double _magnitude;


        // Properties                                                                                                               
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public bool AutoComputeDirection { get { return _autoComputeDirection; } set { _autoComputeDirection = value; } }
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }
        public PreTensionLoadType Type { get { return _type; } set { _type = value; } }
        public double Magnitude { get { return _magnitude; } set { _magnitude = value; } }


        // Constructors                                                                                                             
        public PreTensionLoad(string name, string surfaceName, double magnitude)
            : base(name)
        {
            _surfaceName = surfaceName;
            _autoComputeDirection = true;
            _magnitude = magnitude;
        }
        public PreTensionLoad(string name, string surfaceName, double x, double y, double z, double magnitude)
            : base(name) 
        {
            _surfaceName = surfaceName;
            _autoComputeDirection = false;
            _x = x;
            _y = y;
            _z = z;
            _magnitude = magnitude;
        }


        // Methods                                                                                                                  
        
    }
}
