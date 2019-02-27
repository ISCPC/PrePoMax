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
    class CClear : Command
    {
        // Variables                                                                                                                


        // Constructor                                                                                                              
        public CClear()
            : base("Clear")
        {
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.Clear();
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString();
        }
    }
}
