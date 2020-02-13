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
    class CCreateBoundaryLayer : Command
    {
        // Variables                                                                                                                
        private int[] _geometryIds;
        private double _thickness;


        // Constructor                                                                                                              
        public CCreateBoundaryLayer(int[] geometryIds, double thickness)
            : base("Create boundary layer")
        {
            _geometryIds = geometryIds;
            _thickness = thickness;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.CreateBoundaryLayer(_geometryIds, _thickness);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + "Thickness: " + _thickness.ToString();
        }
    }
}
