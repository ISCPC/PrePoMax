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
    class CReplaceModelPart : Command
    {
        // Variables                                                                                                                
        private string _oldPartName;
        private PartProperties _newPartProperties;


        // Constructor                                                                                                              
        public CReplaceModelPart(string oldPartName, PartProperties newPartProperties)
            : base("Replace mesh part properties")
        {
            _oldPartName = oldPartName;
            _newPartProperties = newPartProperties; //.DeepCopy();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceModelPartProperties(_oldPartName, _newPartProperties);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldPartName + ", " + _newPartProperties.ToString();
        }
    }
}
