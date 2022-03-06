using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;

namespace CaeModel
{
    [Serializable]
    [Flags]
    public enum ContactFieldVariable
    {
        // Must start at 1 for the UI to work
        CDIS = 1,
        CSTR = 2,
        //CELS = 4,
        PCON = 8
    }

    [Serializable]
    public class ContactFieldOutput : FieldOutput
    {
        // Variables                                                                                                                
        private ContactFieldVariable _variables;


        // Properties                                                                                                               
        public ContactFieldVariable Variables { get { return _variables; } set { _variables = value; } }


        // Constructors                                                                                                             
        public ContactFieldOutput(string name, ContactFieldVariable variables)
            : base(name) 
        {
            _variables |= variables;
        }


        // Methods                                                                                                                  
    }
}
