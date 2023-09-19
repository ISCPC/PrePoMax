using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;

namespace PrePoMax.Commands
{
    [Serializable]
    class CDuplicateFieldOutputs : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _fieldOutputNames;


        // Constructor                                                                                                              
        public CDuplicateFieldOutputs(string stepName, string[] fieldOutputNames)
            :base("Duplicate field outputs")
        {
            _stepName = stepName;
            _fieldOutputNames = fieldOutputNames;
        }
       

        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateFieldOutputs(_stepName, _fieldOutputNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_fieldOutputNames);
        }
    }
}
