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
    class CScaleModelParts : Command
    {
        // Variables                                                                                                                
        private string[] _partNames;
        private double[] _scaleCenter;
        private double[] _scaleFactors;
        private bool _copy;

        // Constructor                                                                                                              
        public CScaleModelParts(string[] partNames, double[] scaleCenter, double[] scaleFactors, bool copy)
            : base("Scale mesh parts")
        {
            _partNames = partNames;
            _scaleCenter = scaleCenter;
            _scaleFactors = scaleFactors;
            _copy = copy;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ScaleModelParts(_partNames, _scaleCenter, _scaleFactors, _copy);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_partNames);
        }
    }
}
