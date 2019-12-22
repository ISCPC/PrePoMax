using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class InitialStep : Step, ISerializable
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public InitialStep(string name)
            :base(name)
        {
        }

        //ISerializable
        public InitialStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        // Methods                                                                                                                  

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            base.GetObjectData(info, context);
        }
    }
}
