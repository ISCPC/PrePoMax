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


        // Constructors                                                                                                             
        public FrequencyStep(string name)
            :base(name)
        {
            _numOfFrequencies = 10;

            AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalVariable.U | NodalVariable.RF));
            AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementVariable.E | ElementVariable.S));
        }

        // Methods                                                                                                                  
    }
}
