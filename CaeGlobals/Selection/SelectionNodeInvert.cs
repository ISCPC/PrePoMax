using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CaeGlobals
{
    [Serializable]
    public class SelectionNodeInvert : SelectionNode, ISerializable
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public SelectionNodeInvert()
            : base(vtkSelectOperation.Invert)
        {
        }
        public SelectionNodeInvert(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        // Methods                                                                                                                  

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // using typeof() works also for null fields
            //info.AddValue("_pickedPoint", _pickedPoint, typeof(double[]));
        }
    }
}
