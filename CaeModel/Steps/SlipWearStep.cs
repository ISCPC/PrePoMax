using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class SlipWearStep : StaticStep, ISerializable
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public SlipWearStep(string name)                          // must be a separate constructor!
            : this(name, true)
        {
        }
        public SlipWearStep(string name, bool addFieldOutputs)    // must be a separate constructor!
            : base(name, addFieldOutputs)
        {
            if (addFieldOutputs)
            {
                AddFieldOutput(new ContactFieldOutput("CF-Output-1", ContactFieldVariable.CDIS));
            }
        }
        //ISerializable
        public SlipWearStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                //switch (entry.Name)
                //{
                //    case "_numOfCycles":
                //        _numOfCycles = (int)entry.Value; break;
                //}
            }
        }
        // Methods                                                                                                                  

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            //info.AddValue("_numOfCycles", _numOfCycles, typeof(int));
        }
    }
}
