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
    class CAddReferencePoint : Command
    {
        // Variables                                                                                                                
        private FeReferencePoint _referencePoint;


        // Constructor                                                                                                              
        public CAddReferencePoint(FeReferencePoint referencePoint)
            : base("Add reference point")
        {
            _referencePoint = referencePoint.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddReferencePoint(_referencePoint.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _referencePoint.ToString();
        }
    }
}
