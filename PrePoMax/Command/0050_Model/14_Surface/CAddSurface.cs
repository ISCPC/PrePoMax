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
        private bool _update;


        // Constructor                                                                                                              
        public CAddSurface(FeSurface surface, bool update)
            : base("Add surface")
        {
            _surface = surface.DeepClone();
            _update = update;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddSurface(_surface.DeepClone(), _update);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _surface.ToString();
        }
    }
}
