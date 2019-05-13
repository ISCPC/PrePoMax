using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public class FrequencyStep : Step
    {
        // Variables                                                                                                                
        private double _numOfFrequencies;
        private bool _storage;


        // Properties                                                                                                               
        public double NumOfFrequencies
        {
            get { return _numOfFrequencies; }
            set 
            {
                if (value <= 1) throw new Exception("The number of frequencies must be larger than 0.");
                _numOfFrequencies = value;
            }
        }
        public bool Storage { get { return _storage; } set { _storage = value; } }


        // Constructors                                                                                                             
        public FrequencyStep(string name)
            :base(name)
        {
            _perturbation = true;
            _supportsLoads = false;

            _numOfFrequencies = 10;
            _storage = false;

            AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalFieldVariable.U | NodalFieldVariable.RF));
            AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementFieldVariable.E | ElementFieldVariable.S));
        }

        // Methods                                                                                                                  
    }
}
