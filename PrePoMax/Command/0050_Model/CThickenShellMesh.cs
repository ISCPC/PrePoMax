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
    class CThickenShellMesh : Command
    {
        // Variables                                                                                                                
        private string[] _partNames;
        private double _thickness;
        private int _numberOfLayers;
        private double _offset;


        // Constructor                                                                                                              
        public CThickenShellMesh(string[] partNames, double thickness, int numberOfLayers, double offset)
            : base("Thicken shell mesh")
        {
            _partNames = partNames;
            _thickness = thickness;
            _numberOfLayers = numberOfLayers;
            _offset = offset;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ThickenShellMesh(_partNames, _thickness, _numberOfLayers, _offset);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + "Thickness: " + _thickness.ToString() +
                " Number of layers: " + _numberOfLayers.ToString();
        }
    }
}
