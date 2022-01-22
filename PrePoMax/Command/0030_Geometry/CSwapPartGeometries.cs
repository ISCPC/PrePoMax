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
    class CSwapPartGeometries : Command
    {
        // Variables                                                                                                                
        private string _partName1;
        private string _partName2;


        // Constructor                                                                                                              
        public CSwapPartGeometries(string partName1, string partName2)
            : base("Swap part geometries")
        {
            _partName1 = partName1;
            _partName2 = partName2;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.SwapGeometryPartGeometries(_partName1, _partName2);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _partName1 + ", " + _partName2;
        }
    }
}
