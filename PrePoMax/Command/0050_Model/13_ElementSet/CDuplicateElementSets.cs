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
    class CDuplicateElementSets : Command
    {
        // Variables                                                                                                                
        private string[] _elementSetNames;


        // Constructor                                                                                                              
        public CDuplicateElementSets(string[] elementSetNames)
            : base("Duplicate element set")
        {
            _elementSetNames = elementSetNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateElementSets(_elementSetNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_elementSetNames);
        }
    }
}
