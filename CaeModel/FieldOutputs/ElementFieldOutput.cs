using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;

namespace CaeModel
{
    [Serializable]
    [Flags]
    public enum ElementVariable
    {
        E = 1,
        PEEQ = 2,
        S = 4,
        ENER = 8,
        ERR = 16
    }

    [Serializable]
    public class ElementFieldOutput : FieldOutput
    {
        // Variables                                                                                                                
        private ElementVariable _variables;

        // Properties                                                                                                               
        public ElementVariable Variables 
        {
            get 
            { 
                return _variables; 
            }
            set
            {
                _variables = value;
            }
        }

        // Constructors                                                                                                             
        public ElementFieldOutput(string name, ElementVariable variables)
            : base(name) 
        {
            _variables |= variables;
        }
    

        // Methods                                                                                                                  
    }
}
