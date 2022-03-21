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
    class CReplaceAmplitude : Command
    {
        // Variables                                                                                                                
        private string _oldAmplitudeName;
        private Amplitude _newAmplitude;


        // Constructor                                                                                                              
        public CReplaceAmplitude(string oldAmplitudeName, Amplitude newAmplitude)
            : base("Edit amplitude")
        {
            _oldAmplitudeName = oldAmplitudeName;
            _newAmplitude = newAmplitude.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceAmplitude(_oldAmplitudeName, _newAmplitude.DeepClone());
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldAmplitudeName + ", " + _newAmplitude.ToString();
        }
    }
}
