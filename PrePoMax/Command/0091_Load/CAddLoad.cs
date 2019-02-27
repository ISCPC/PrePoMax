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
    class CAddLoad : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private Load _load;


        // Constructor                                                                                                              
        public CAddLoad(string stepName, Load load)
            : base("Add load")
        {
            _stepName = stepName;
            _load = load.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddLoad(_stepName, _load.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _load.ToString();
        }
    }
}
