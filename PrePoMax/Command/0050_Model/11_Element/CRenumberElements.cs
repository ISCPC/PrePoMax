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
    class CRenumberElements : Command
    {
        // Variables                                                                                                                
        private int _startElementId;


        // Constructor                                                                                                              
        public CRenumberElements(int startElementId)
            : base("Renumber elements")
        {
            _startElementId = startElementId;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RenumberElements(_startElementId);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _startElementId;
        }
    }
}
