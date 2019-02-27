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
    class CSetCalculixUserKeywords : Command
    {
        // Variables                                                                                                                
        private Dictionary<int[], FileInOut.Output.Calculix.CalculixUserKeyword> _userKeywords;


        // Constructor                                                                                                              
        public CSetCalculixUserKeywords(Dictionary<int[], FileInOut.Output.Calculix.CalculixUserKeyword> userKeywords)
            : base("Set CalculiX user keywords")
        {
            _userKeywords = userKeywords.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.SetCalculixUserKeywords(_userKeywords.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _userKeywords.Count;
        }
    }
}
