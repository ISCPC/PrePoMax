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
    public class CircularPattern : Transformation
    {
        // Variables                                                                                                                
        protected double[] _axisFirstPoint;
        protected double[] _axisSecondPoint;
        protected double _angle;
        protected int _numberOfItems;


        // Properties                                                                                                               
        public double[] AxisFirstPoint { get { return _axisFirstPoint; } set { _axisFirstPoint = value; } }
        public double[] AxisSecondPoint { get { return _axisSecondPoint; } set { _axisSecondPoint = value; } }
        public double[] AxisNormal
        {
            get
            {
                if (_axisFirstPoint != null && _axisSecondPoint != null)
                {
                    Vec3D start = new Vec3D(_axisFirstPoint);
                    Vec3D end = new Vec3D(_axisSecondPoint);
                    Vec3D normal = end - start;
                    normal.Normalize();
                    return normal.Coor;
                }
                return new double[3];
            }
        }
        public double AxisNormalLength
        {
            get
            {
                if (_axisFirstPoint != null && _axisSecondPoint != null)
                {
                    Vec3D start = new Vec3D(_axisFirstPoint);
                    Vec3D end = new Vec3D(_axisSecondPoint);
                    Vec3D normal = end - start;
                    return normal.Normalize();
                }
                return 0;
            }
        }
        public double Angle { get { return _angle; } set { _angle = value; } }
        public int NumberOfItems
        {
            get { return _numberOfItems; }
            set
            {
                _numberOfItems = value;
                if (_numberOfItems < 2) _numberOfItems = 2;
            }
        }


        // Constructor                                                                                                              
        public CircularPattern(string name, double[] axisFirstPoint, double[] axisSecondPoint, double angle, int numberOfItems)
            : base(name)
        {
            _axisFirstPoint = axisFirstPoint;
            _axisSecondPoint = axisSecondPoint;
            _angle = angle;
            NumberOfItems = numberOfItems;
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  


    }
}


