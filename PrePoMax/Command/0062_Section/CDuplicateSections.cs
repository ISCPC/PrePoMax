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
    class CDuplicateSections : Command
    {
        // Variables                                                                                                                
        private string[] _sectionNames;


        // Constructor                                                                                                              
        public CDuplicateSections(string[] sectionNames)
            : base("Duplicate sections")
        {
            _sectionNames = sectionNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateSections(_sectionNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_sectionNames);
        }
    }
}
