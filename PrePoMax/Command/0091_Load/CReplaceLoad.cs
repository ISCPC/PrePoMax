using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeGlobals;

namespace PrePoMax.Commands
{
    [Serializable]
    class CReplaceLoad : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string _oldLoadName;
        private Load _newLoad;


        // Constructor                                                                                                              
        public CReplaceLoad(string stepName, string oldLoadName, Load newLoad)
            : base("Edit load")
        {
            _stepName = stepName;
            _oldLoadName = oldLoadName;
            _newLoad = newLoad.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceLoad(_stepName, _oldLoadName, _newLoad.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _oldLoadName + ", " + _newLoad.ToString();
        }
    }
}
