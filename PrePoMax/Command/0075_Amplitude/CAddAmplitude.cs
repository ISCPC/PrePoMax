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
    class CAddAmplitude : Command
    {
        // Variables                                                                                                                
        private Amplitude _amplitude;


        // Constructor                                                                                                              
        public CAddAmplitude(Amplitude amplitude)
            :base("Add amplitude")
        {
            _amplitude = amplitude.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddAmplitude(_amplitude.DeepClone());
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _amplitude.ToString();
        }
    }
}
