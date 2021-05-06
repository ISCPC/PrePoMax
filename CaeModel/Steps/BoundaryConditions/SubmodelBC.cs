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
    public class SubmodelBC : BoundaryCondition
    {
        // Variables                                                                                                                
        private int _stepNumber;


        // Properties                                                                                                               
        public int StepNumber 
        { 
            get { return _stepNumber; } 
            set 
            {
                _stepNumber = value;
                if (_stepNumber < 1) _stepNumber = 1;
            }
        }
        public bool U1 { get; set; }
        public bool U2 { get; set; }
        public bool U3 { get; set; }
        public bool UR1 { get; set; }
        public bool UR2 { get; set; }
        public bool UR3 { get; set; }


        // Constructors                                                                                                             
        public SubmodelBC(string name, string regionName, RegionTypeEnum regionType)
            : base(name, regionName, regionType) 
        {
            _stepNumber = 1;
            U1 = false;
            U2 = false;
            U3 = false;
            UR1 = false;
            UR2 = false;
            UR3 = false;
        }


        // Methods                                                                                                                  
        public int[] GetConstrainedDirections()
        {
            List<int> directions = new List<int>();
            if (U1) directions.Add(1);
            if (U2) directions.Add(2);
            if (U3) directions.Add(3);
            if (UR1) directions.Add(4);
            if (UR2) directions.Add(5);
            if (UR3) directions.Add(6);
            return directions.ToArray();
        }
    }
}
