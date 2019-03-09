using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public class BuckleStep : Step
    {
        // Variables                                                                                                                
        private int _numOfBucklingFactors;
        private double _accuracy;


        // Properties                                                                                                               
        public int NumOfBucklingFactors
        {
            get { return _numOfBucklingFactors; }
            set
            {
                _numOfBucklingFactors = value;
                if (_numOfBucklingFactors < 1) _numOfBucklingFactors = 1;
            }
        }
        public double Accuracy
        {
            get { return _accuracy; }
            set
            {
                _accuracy = value;

                if (_numOfBucklingFactors < 1) _numOfBucklingFactors = 1;
            }
        }


        // Constructors                                                                                                             
        public BuckleStep(string name)
            :base(name)
        {
            //_perturbation = false;

            _numOfBucklingFactors = 1;
            _accuracy = 0.01;

            AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalVariable.U | NodalVariable.RF));
            AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementVariable.E | ElementVariable.S));
        }


        // Methods                                                                                                                  
    }
}
