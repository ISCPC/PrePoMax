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
    public enum SymmetryPlaneEnum
    {
        X,
        Y,
        Z
    }

    [Serializable]
    public class Symmetry : Transformation
    {
        // Variables                                                                                                                
        protected double[] _pointCoor;
        protected SymmetryPlaneEnum _symmetryPlane;


        // Properties                                                                                                               
        public double[] PointCoor { get { return _pointCoor; } set { _pointCoor = value; } }
        public SymmetryPlaneEnum SymmetryPlane { get { return _symmetryPlane; } set { _symmetryPlane = value; } }


        // Constructor                                                                                                              
        public Symmetry(string name, double[] pointCoor, SymmetryPlaneEnum symmetryPlane)
            : base(name)
        {
            _pointCoor = pointCoor;
            _symmetryPlane = symmetryPlane;
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  
       

    }
}

