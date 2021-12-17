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
    class CReplaceReferencePoint : Command
    {
        // Variables                                                                                                                
        private string _oldReferencePointName;
        private FeReferencePoint _newReferencePoint;

        // Constructor                                                                                                              
        public CReplaceReferencePoint(string oldReferencePointName, FeReferencePoint newReferencePoint)
            : base("Edit reference point")
        {
            _oldReferencePointName = oldReferencePointName;
            _newReferencePoint = newReferencePoint.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceReferencePoint(_oldReferencePointName, _newReferencePoint.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldReferencePointName + ", " + _newReferencePoint.ToString();
        }
    }
}
