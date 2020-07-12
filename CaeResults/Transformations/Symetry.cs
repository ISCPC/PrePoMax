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
    public enum SymetryPlaneEnum
    {
        X,
        Y,
        Z
    }

    [Serializable]
    public class Symetry : Transformation
    {
        // Variables                                                                                                                
        protected double[] _pointCoor;
        protected SymetryPlaneEnum _symetryPlane;


        // Properties                                                                                                               
        public double[] PointCoor { get { return _pointCoor; } set { _pointCoor = value; } }
        public SymetryPlaneEnum SymetryPlane { get { return _symetryPlane; } set { _symetryPlane = value; } }


        // Constructor                                                                                                              
        public Symetry(string name, double[] pointCoor, SymetryPlaneEnum symetryPlane)
            : base(name)
        {
            _pointCoor = pointCoor;
            _symetryPlane = symetryPlane;
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  
       

    }
}

