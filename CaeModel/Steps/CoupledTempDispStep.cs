using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class CoupledTempDispStep : UncoupledTempDispStep, ISerializable
    {
        // Variables                                                                                                                
        

        // Properties                                                                                                               


        // Constructors                                                                                                             
        public CoupledTempDispStep(string name)
            :base(name)
        {
        }
        //ISerializable
        public CoupledTempDispStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
       
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
