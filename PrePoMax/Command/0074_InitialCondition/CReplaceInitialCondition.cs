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
    class CReplaceSection : Command
    {
        // Variables                                                                                                                
        private string _oldSectionName;
        private Section _newSection;

        // Constructor                                                                                                              
        public CReplaceSection(string oldSectionName, Section newSection)
            : base("Edit section")
        {
            _oldSectionName = oldSectionName;
            _newSection = newSection.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceSection(_oldSectionName, _newSection.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldSectionName + ", " + _newSection.ToString();
        }
    }
}
