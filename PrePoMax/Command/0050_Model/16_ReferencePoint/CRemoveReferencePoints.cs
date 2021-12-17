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
    class CRemoveReferencePoints : Command
    {
        // Variables                                                                                                                
        private string[] _referencePointNames;


        // Constructor                                                                                                              
        public CRemoveReferencePoints(string[] referencePointNames)
            : base("Remove reference points")
        {
            _referencePointNames = referencePointNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveReferencePoints(_referencePointNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_referencePointNames);
        }
    }
}
