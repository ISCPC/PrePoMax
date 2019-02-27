using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;


namespace PrePoMax.Commands
{
    [Serializable]
    class CShowLoads : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _loadNames;


        // Constructor                                                                                                              
        public CShowLoads(string stepName, string[] loadNames)
            : base("Show loads")
        {
            _stepName = stepName; 
            _loadNames = loadNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ShowLoads(_stepName, _loadNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_loadNames);
        }
    }
}
