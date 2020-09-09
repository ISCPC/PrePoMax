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
    class CFlipFaceOrientations : Command
    {
        // Variables                                                                                                                
        private GeometrySelection _geometrySelection;


        // Constructor                                                                                                              
        public CFlipFaceOrientations(GeometrySelection geometrySelection)
            : base("Flip face orientations")
        {
            _geometrySelection = geometrySelection.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.FlipFaceOrientations(_geometrySelection.DeepClone());
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _geometrySelection.ToString();
        }
    }
}
