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
    class CAddMaterial : Command
    {
        // Variables                                                                                                                
        private Material _material;


        // Constructor                                                                                                              
        public CAddMaterial(Material material)
            : base("Add material")
        {
            _material = material.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddMaterial(_material.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _material.ToString();
        }
    }
}
