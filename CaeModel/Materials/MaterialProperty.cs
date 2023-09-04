using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh;

namespace CaeModel
{
    [Serializable]
    public abstract class MaterialProperty : ISerializable, IContainsEquations
    {
        // Variables                                                                                                                
        [NonSerialized]
        protected const string _positive = "The value must be larger than 0.";


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public MaterialProperty()
        {
        }
        public MaterialProperty(SerializationInfo info, StreamingContext context)
        {
        }


        // Methods                                                                                                                  
        // IContainsEquations
        public abstract void CheckEquations();
        public virtual bool TryCheckEquations()
        {
            try
            {
                CheckEquations();
                return true;
            }
            catch (Exception ex) { return false; }
        }
        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
