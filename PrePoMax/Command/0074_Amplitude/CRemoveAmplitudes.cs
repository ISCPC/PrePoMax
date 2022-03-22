using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;

namespace PrePoMax.Commands
{
    [Serializable]
    class CRemoveAmplitudes : Command
    {
        // Variables                                                                                                                
        private string[] _amplitudeNames;

        // Constructor                                                                                                              
        public CRemoveAmplitudes(string[] amplitudeNames)
            :base("Remove amplitude")
        {
            _amplitudeNames = amplitudeNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveAmplitudes(_amplitudeNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_amplitudeNames);
        }
    }
}
