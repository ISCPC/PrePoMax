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
    class CSetTransparencyForGeometryParts : Command
    {
        // Variables                                                                                                                
        private string[] _partNames;
        private byte _alpha;


        // Constructor                                                                                                              
        public CSetTransparencyForGeometryParts(string[] partNames, byte alpha)
            : base("Set transparency for geometry parts")
        {
            _partNames = partNames;
            _alpha = alpha;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.SetTransparencyForGeometryParts(_partNames, _alpha);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + "Alpha = " + _alpha.ToString() + ": " + GetArrayAsString(_partNames);
        }
    }
}
