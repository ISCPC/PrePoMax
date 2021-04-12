using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class ExplodedViewParameters
    {
        // Variables                                                                                                                
        private double _scaleFactor;
        private double _magnification;


        // Properties                                                                                                               
        public double ScaleFactor { get { return _scaleFactor; } set { _scaleFactor = value; } }
        public double Magnification { get { return _magnification; } set { _magnification = value; } }


        // Constructors                                                                                                             
        public ExplodedViewParameters()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void Reset()
        {
            _scaleFactor = -1;
            _magnification = 3;
        }
    }
}
