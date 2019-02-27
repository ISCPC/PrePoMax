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
    class CAddSurface : Command
    {
        // Variables                                                                                                                
        private FeSurface _surface;


        // Constructor                                                                                                              
        public CAddSurface(FeSurface surface)
            : base("Add surface")
        {
            _surface = surface.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddSurface(_surface.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _surface.ToString();
        }
    }
}
