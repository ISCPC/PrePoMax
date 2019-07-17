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
    class CTranslateModelParts : Command
    {
        // Variables                                                                                                                
        private string[] _partNames;
        private double[] _translateVector;
        private bool _copy;

        // Constructor                                                                                                              
        public CTranslateModelParts(string[] partNames, double[] translateVector, bool copy)
            : base("Translate mesh parts")
        {
            _partNames = partNames;
            _translateVector = translateVector;
            _copy = copy;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.TranslateModelParts(_partNames, _translateVector, _copy);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_partNames);
        }
    }
}
