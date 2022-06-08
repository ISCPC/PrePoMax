using CaeGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class MaxValueAnnotation : AnnotationBase
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public MaxValueAnnotation()
            : base(AnnotationContainer.MaxAnnotationName)
        {
        }


        // Methods
        public override void GetAnnotationData(out string text, out double[] coor)
        {
            text = "MaxAnnotation";
            coor = new double[3];
        }

    }
}
