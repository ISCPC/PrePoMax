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
    class CDuplicateAmplitudes : Command
    {
        // Variables                                                                                                                
        private string[] _amplitudeNames;


        // Constructor                                                                                                              
        public CDuplicateAmplitudes(string[] amplitudeNames)
            : base("Duplicate amplitudes")
        {
            _amplitudeNames = amplitudeNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateAmplitudes(_amplitudeNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_amplitudeNames);
        }
    }
}
