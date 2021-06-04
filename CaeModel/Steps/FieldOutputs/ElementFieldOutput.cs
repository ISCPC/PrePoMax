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
    public enum ElementFieldVariable
    {
        // Must start at 1 for the UI to work
        S = 1,
        E = 2,
        ME = 4,
        PEEQ = 8,
        ENER = 16,
        // Thermal
        HFL = 32,
        // Error
        ERR = 64,
        HER = 128,
        ZZS = 256
    }

    [Serializable]
    public class ElementFieldOutput : FieldOutput
    {
        // Variables                                                                                                                
        private ElementFieldVariable _variables;


        // Properties                                                                                                               
        public ElementFieldVariable Variables { get { return _variables; } set { _variables = value; } }


        // Constructors                                                                                                             
        public ElementFieldOutput(string name, ElementFieldVariable variables)
            : base(name) 
        {
            _variables |= variables;
        }


        // Methods                                                                                                                  
    }
}
