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
    class CCreateMesh : Command
    {
        // Variables                                                                                                                
        private string _partName;


        // Constructor                                                                                                              
        public CCreateMesh(string partName)
            : base("Create mesh")
        {
            _partName = partName;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            return receiver.CreateMesh(_partName);
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _partName;
        }
    }
}
