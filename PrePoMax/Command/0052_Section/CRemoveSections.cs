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
    class CRemoveSections : Command
    {
        // Variables                                                                                                                
        private string[] _sectionNames;


        // Constructor                                                                                                              
        public CRemoveSections(string[] sectionNames)
            : base("Remove sections")
        {
            _sectionNames = sectionNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveSections(_sectionNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_sectionNames);
        }
    }
}
