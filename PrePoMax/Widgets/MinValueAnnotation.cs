using CaeGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class MinValueAnnotation : AnnotationBase
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public MinValueAnnotation()
            : base(AnnotationContainer.MinAnnotationName)
        {
        }


        // Methods
        public override void GetAnnotationData(out string text, out double[] coor)
        {
            text = "MinAnnotation";
            coor = new double[3];
        }

    }
}
