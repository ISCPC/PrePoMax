using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Kitware.VTK;
using CaeGlobals;

namespace vtkControl
{
    public class vtkMaxAnimationFrameData
    {
        // Variables                                                                                                                
        public float[] Time;
        public float[] ScaleFactor;
        public double[] AllFramesScalarRange;
        public bool UseAllFrameData;
        

        // Constructors                                                                                                             
        public vtkMaxAnimationFrameData()
        {
            Time = null;
            ScaleFactor = null;
            AllFramesScalarRange = null;
        }
        public vtkMaxAnimationFrameData(float[] time, float[] scale, double[] scalarRange)
        {
            Time = time;
            ScaleFactor = scale;
            AllFramesScalarRange = scalarRange;
        }
    }
}
