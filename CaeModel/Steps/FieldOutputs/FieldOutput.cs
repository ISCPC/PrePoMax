using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public abstract class FieldOutput : NamedClass
    {
        // Variables                                                                                                                
        private int _frequency;


        // Properties                                                                                                               
        public int Frequency
        {
            get { return _frequency;}
            set
            {
                if (value < 1) throw new Exception("The frequency value must be larger or equal to 1.");
                _frequency = value;
            }
        }


        // Constructors                                                                                                             
        public FieldOutput(string name)
            : base(name)
        {
            _frequency = 1;
        }


        // Methods                                                                                                                  
    }
}
