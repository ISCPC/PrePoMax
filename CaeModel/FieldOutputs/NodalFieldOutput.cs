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
    public enum NodalVariable
    {
        RF = 1,
        U = 2
    }

    [Serializable]
    public class NodalFieldOutput : FieldOutput
    {
        // Variables                                                                                                                
        private NodalVariable _variables;


        // Properties                                                                                                               
        public NodalVariable Variables 
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
        public NodalFieldOutput(string name, NodalVariable variables)
            : base(name) 
        {
            _variables |= variables;
        }


        // Methods                                                                                                                  
    }
}
