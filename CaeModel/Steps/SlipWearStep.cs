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
        private int _numOfCycles;                 //ISerializable


        // Properties                                                                                                               
        public int NumOfCycles
        {
            get { return _numOfCycles; }
            set
            {
                if (value < 1) _numOfCycles = 1;
                else _numOfCycles = value;
            }
        }


        // Constructors                                                                                                             
        public SlipWearStep(string name)                          // must be a separate constructor!
            : this(name, true)
        {
        }
        public SlipWearStep(string name, bool addFieldOutputs)    // must be a separate constructor!
            : base(name, addFieldOutputs)
        {
            _numOfCycles = 1;
            //
            if (addFieldOutputs)
            {
                AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalFieldVariable.U | NodalFieldVariable.RF));
                AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementFieldVariable.E | ElementFieldVariable.S));
                AddFieldOutput(new ContactFieldOutput("CF-Output-1", ContactFieldVariable.CDIS));
            }
        }
        //ISerializable
        public SlipWearStep(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            int count = 0;
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_numOfCycles":
                        _numOfCycles = (int)entry.Value; count++; break;
                }
            }
        }
        // Methods                                                                                                                  

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_numOfCycles", _numOfCycles, typeof(int));
        }
    }
}
