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
    class CHideReferencePoints : Command
    {
        // Variables                                                                                                                
        private string[] _referencePoints;


        // Constructor                                                                                                              
        public CHideReferencePoints(string[] referencePoints)
            : base("Hide reference points")
        {
            _referencePoints = referencePoints;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.HideReferencePoints(_referencePoints);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_referencePoints);
        }
    }
}
