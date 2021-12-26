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
    class CRotateModelParts : Command
    {
        // Variables                                                                                                                
        private string[] _partNames;
        private double[] _rotateCenter;
        private double[] _rotateAxis;
        private double _rotateAngle;
        private bool _copy;

        // Constructor                                                                                                              
        public CRotateModelParts(string[] partNames, double[] rotateCenter, double[] rotateAxis, double rotateAngle, bool copy)
            : base("Rotate mesh parts")
        {
            _partNames = partNames;
            _rotateCenter = rotateCenter;
            _rotateAxis = rotateAxis;
            _rotateAngle = rotateAngle;
            _copy = copy;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RotateModelParts(_partNames, _rotateCenter, _rotateAxis, _rotateAngle, _copy);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_partNames);
        }
    }
}
