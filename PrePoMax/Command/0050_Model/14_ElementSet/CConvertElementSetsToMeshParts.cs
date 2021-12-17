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
    class CConvertElementSetsToMeshParts : Command
    {
        // Variables                                                                                                                
        private string[] _elementSetNames;


        // Constructor                                                                                                              
        public CConvertElementSetsToMeshParts(string[] elementSetNames)
            : base("Convert element sets to mesh parts")
        {
            _elementSetNames = elementSetNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ConvertElementSetsToMeshParts(_elementSetNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_elementSetNames);
        }
    }
}
