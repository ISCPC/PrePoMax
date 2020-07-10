using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using Octree;

namespace CaeResults
{
    [Serializable]
    public class CylindricalPattern : Transformation
    {
        // Variables                                                                                                                
        protected double[] _axisStartPoint;
        protected double[] _axisEndPoint;
        protected double _angle;
        protected int _numberOfItems;


        // Properties                                                                                                               
        public double[] AxisStartPoint { get { return _axisStartPoint; } set { _axisStartPoint = value; } }
        public double[] AxisEndPoint { get { return _axisEndPoint; } set { _axisEndPoint = value; } }
        public double Angle { get { return _angle; } set { _angle = value; } }
        public int NumOfItems { get { return _numberOfItems; } set { _numberOfItems = value; } }


        // Constructor                                                                                                              
        public CylindricalPattern(string name, double[] axisStartPoint, double[] axisEndPoint, double angle, int numberOfItems)
            : base(name)
        {
            _axisStartPoint = axisStartPoint;
            _axisEndPoint = axisEndPoint;
            _angle = angle;
            _numberOfItems = numberOfItems;
        }



        // Static methods                                                                                                           


        // Methods                                                                                                                  


    }
}


