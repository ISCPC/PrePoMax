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
        private bool _isInWearStep;


        // Properties                                                                                                               
        public ContactFieldVariable Variables { get { return _variables; } set { _variables = value; } }
        public bool IsInWearStep { get { return _isInWearStep; } set { _isInWearStep = value; } }


        // Constructors                                                                                                             
        public ContactFieldOutput(string name, ContactFieldVariable variables, bool isInWearStep)
            : base(name) 
        {
            _variables |= variables;
            _isInWearStep = isInWearStep;
        }


        // Methods                                                                                                                  
    }
}
