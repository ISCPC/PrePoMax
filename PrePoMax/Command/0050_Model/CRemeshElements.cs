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
    class CRemeshElements : Command
    {
        // Variables                                                                                                                
        private RemeshingParameters _remeshingParameters;


        // Constructor                                                                                                              
        public CRemeshElements(RemeshingParameters remeshingParameters)
            : base("Remesh elements")
        {
            _remeshingParameters = remeshingParameters.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            return receiver.RemeshElements(_remeshingParameters);
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _remeshingParameters;
        }
    }
}
