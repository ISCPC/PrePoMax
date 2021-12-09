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
    // Compatibility v1.1.1 - the command was renamed to CSetNewModelProperties
    [Serializable]
    class CSetModelUnitSystem : Command
    {
        // Variables                                                                                                                
        private UnitSystemType _unitSystemType;


        // Constructor                                                                                                              
        public CSetModelUnitSystem(UnitSystemType unitSystemType)
            : base("Set unit system")
        {
            _unitSystemType = unitSystemType;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.SetNewModelProperties(ModelSpaceEnum.ThreeD, _unitSystemType);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _unitSystemType.GetDescription();
        }
    }
}
