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
    class CScaleGeometryParts : Command
    {
        // Variables                                                                                                                
        private string[] _partNames;
        private double[] _scaleCenter;
        private double[] _scaleFactors;
        private bool _copy;


        // Constructor                                                                                                              
        public CScaleGeometryParts(string[] partNames, double[] scaleCenter, double[] scaleFactors, bool copy)
            : base("Scale geometry parts")
        {
            _partNames = partNames;
            _scaleCenter = scaleCenter;
            _scaleFactors = scaleFactors;
            _copy = copy;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ScaleGeometryParts(_partNames, _scaleCenter, _scaleFactors, _copy);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_partNames);
        }
    }
}
