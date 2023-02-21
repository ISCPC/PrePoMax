using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace CaeResults
{
    [Serializable]
    public class ResultHistoryOutputFromField : ResultHistoryOutput
    {
        // Variables                                                                                                                
        private string _fieldName;
        private string _componentName;
        private int _stepId;
        private int _stepIncrementId;


        // Properties                                                                                                               
        public string FieldName { get { return _fieldName; } set { _fieldName = value; } }
        public string ComponentName { get { return _componentName; } set { _componentName = value; } }
        public int StepId { get { return _stepId; } set { _stepId = value; } }
        public int StepIncrementId { get { return _stepIncrementId; } set { _stepIncrementId = value; } }


        // Constructors                                                                                                             
        public ResultHistoryOutputFromField(string name, string filedName, string componentName,
                                            int stepId, int incrementId, string regionName, RegionTypeEnum regionType)
            : base(name, regionName, regionType)
        {
            _fieldName = filedName;
            _componentName = componentName;
            _stepId = stepId;
            _stepIncrementId = incrementId;
        }


        // Methods                                                                                                                  
    }
}
