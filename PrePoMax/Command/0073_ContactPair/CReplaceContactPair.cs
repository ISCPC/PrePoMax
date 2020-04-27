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
    class CReplaceContactPair : Command
    {
        // Variables                                                                                                                
        private string _oldContactPairName;
        private ContactPair _newContactPair;


        // Constructor                                                                                                              
        public CReplaceContactPair(string oldContactPairName, ContactPair newContactPair)
            : base("Edit contact pair")
        {
            _oldContactPairName = oldContactPairName;
            _newContactPair = newContactPair.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceContactPair(_oldContactPairName, _newContactPair.DeepClone());
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldContactPairName + ", " + _newContactPair.ToString();
        }
    }
}
