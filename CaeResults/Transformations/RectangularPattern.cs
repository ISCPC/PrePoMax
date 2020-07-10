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
    public class RectangularPattern : Transformation
    {
        // Variables                                                                                                                
        protected double[] _displacement;
        protected int _numberOfItems;


        // Properties                                                                                                               
        public double[] Displacement { get { return _displacement; } set { _displacement = value; } }
        public int NumberOfItems { get { return _numberOfItems; } set { _numberOfItems = value; } }


        // Constructor                                                                                                              
        public RectangularPattern(string name, double[] displacement, int numberOfItems)
            : base(name)
        {
            _displacement = displacement;
            _numberOfItems = numberOfItems;
        }



        // Static methods                                                                                                           


        // Methods                                                                                                                  


    }
}

