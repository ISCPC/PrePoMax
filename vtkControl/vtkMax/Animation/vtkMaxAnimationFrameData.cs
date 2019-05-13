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
        public List<Dictionary<int, string>> AnimatedActorNames;
        public HashSet<string> InitializedActorNames;
        public double MemMb;

        // Constructors                                                                                                             
        public vtkMaxAnimationFrameData()
            : this(null, null, null)
        {
        }

        public vtkMaxAnimationFrameData(float[] time, float[] scale, double[] scalarRange)
        {
            Time = time;
            ScaleFactor = scale;
            AllFramesScalarRange = scalarRange;
            AnimatedActorNames = new List<Dictionary<int, string>>();
            InitializedActorNames = new HashSet<string>();
            MemMb = 0;
        }
    }
}
