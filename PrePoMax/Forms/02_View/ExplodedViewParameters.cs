using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public enum ExplodedViewTypeEnum
    {
        XYZ = 0,
        X = 1,
        Y = 2,
        Z = 3,
        XY = 4,
        XZ = 5,
        YZ = 6
    }
    [Serializable]
    public class ExplodedViewParameters
    {
        // Variables                                                                                                                
        private ExplodedViewTypeEnum _explodedViewType;
        private double _scaleFactor;
        private double _magnification;


        // Properties                                                                                                               
        public ExplodedViewTypeEnum Type { get { return _explodedViewType; } set { _explodedViewType = value; } }
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
            _explodedViewType = ExplodedViewTypeEnum.XYZ;
            _scaleFactor = -1;
            _magnification = 2;
        }
    }
}
