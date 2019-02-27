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
    interface ICommandWithDialog
    {
        // Methods                                                                                                                  
        void ExecuteWithDialogs(Controller receiver);
    }
}
