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
    public class LinearPattern : Transformation
    {
        // Variables                                                                                                                
        protected double[] _startPoint;
        protected double[] _endPoint;
        protected int _numberOfItems;


        // Properties                                                                                                               
        public double[] StartPoint { get { return _startPoint; } set { _startPoint = value; } }
        public double[] EndPoint { get { return _endPoint; } set { _endPoint = value; } }
        public double[] Displacement
        {
            get
            {
                if (_startPoint != null && _endPoint != null)
                {
                    Vec3D start = new Vec3D(_startPoint);
                    Vec3D end = new Vec3D(_endPoint);
                    Vec3D delta = end - start;
                    return delta.Coor;
                }
                return new double[3];
            }
        }
        public double DisplacementLength
        {
            get
            {
                if (_startPoint != null && _endPoint != null)
                {
                    Vec3D start = new Vec3D(_startPoint);
                    Vec3D end = new Vec3D(_endPoint);
                    Vec3D delta = end - start;
                    return delta.Len;
                }
                return 0;
            }
        }
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
        public LinearPattern(string name, double[] startPoint, double[] endPoint, int numberOfItems)
            : base(name)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
            NumberOfItems = numberOfItems;
        }



        // Static methods                                                                                                           


        // Methods                                                                                                                  


    }
}

