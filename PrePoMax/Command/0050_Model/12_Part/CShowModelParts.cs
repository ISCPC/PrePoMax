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
    class CShowModelParts : Command
    {
        // Variables                                                                                                                
        private string[] _partNames;


        // Constructor                                                                                                              
        public CShowModelParts(string[] partNames)
            : base("Show mesh parts")
        {
            _partNames = partNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ShowModelParts(_partNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_partNames);
        }
    }
}
