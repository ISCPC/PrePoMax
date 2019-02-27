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
    class CReplaceMaterial : Command
    {
        // Variables                                                                                                                
        private string _oldMaterialName;
        private Material _newMaterial;

        // Constructor                                                                                                              
        public CReplaceMaterial(string oldMaterialName, Material newMaterial)
            : base("Edit material")
        {
            _oldMaterialName = oldMaterialName;
            _newMaterial = newMaterial.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceMaterial(_oldMaterialName, _newMaterial.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldMaterialName + ", " + _newMaterial.ToString();
        }
    }
}
