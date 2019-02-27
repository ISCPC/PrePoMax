using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public class StaticStep : Step
    {
        // Variables                                                                                                                
        private double _timePeriod;
        private double _initialTimeIncrement;
        private double _minTimeIncrement;
        private double _maxTimeIncrement;


        // Properties                                                                                                               
        public double TimePeriod 
        {
            get { return _timePeriod; }
            set 
            {
                if (value <= 0) throw new Exception("The time period value must be positive.");
                _timePeriod = value;
            }
        }
        public double InitialTimeIncrement
        {
            get { return _initialTimeIncrement; }
            set
            {
                if (value <= 0) throw new Exception("The initial time increment value must be positive.");
                _initialTimeIncrement = value;
            }
        }
        public double MinTimeIncrement
        {
            get { return _minTimeIncrement; }
            set
            {
                if (value <= 0) throw new Exception("The min time increment value must be positive.");
                _minTimeIncrement = value;
            }
        }
        public double MaxTimeIncrement
        {
            get { return _maxTimeIncrement; }
            set
            {
                if (value <= 0) throw new Exception("The max time increment value must be positive.");
                _maxTimeIncrement = value;
            }
        }
        public bool Direct { get; set; }


        // Constructors                                                                                                             
        public StaticStep(string name)
            :base(name)
        {
            _timePeriod = 1;
            _initialTimeIncrement = 1;
            _minTimeIncrement = 1E-5;
            _maxTimeIncrement = 1E30;
            Direct = false;

            AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalVariable.U | NodalVariable.RF));
            AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementVariable.E | ElementVariable.S));
        }

        // Methods                                                                                                                  
    }
}
