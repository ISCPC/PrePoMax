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
        public int[] StepId;
        public int[] StepIncrementId;
        public float[] ScaleFactor;
        public double[] AllFramesScalarRange;
        public bool UseAllFrameData;
        public List<Dictionary<int, string>> AnimatedActorNames;
        public Dictionary<string, bool> ActorVisible;
        public HashSet<string> InitializedActorNames;
        public double MemMb;

        // Constructors                                                                                                             
        public vtkMaxAnimationFrameData()
            : this(null, null, null, null, null)
        {
        }

        public vtkMaxAnimationFrameData(float[] time, int[] stepId, int[] stepIncrementId, float[] scale, double[] scalarRange)
        {
            Time = time;
            StepId = stepId;
            StepIncrementId = stepIncrementId;
            ScaleFactor = scale;
            AllFramesScalarRange = scalarRange;
            AnimatedActorNames = new List<Dictionary<int, string>>();
            ActorVisible = new Dictionary<string, bool>();
            InitializedActorNames = new HashSet<string>();
            MemMb = 0;
        }
    }
}
