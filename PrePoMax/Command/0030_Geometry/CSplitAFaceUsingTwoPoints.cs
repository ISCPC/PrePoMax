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
    class CSplitAFaceUsingTwoPoints : Command
    {
        // Variables                                                                                                                
        private GeometrySelection _surfaceSelection;
        private GeometrySelection _verticesSelection;


        // Constructor                                                                                                              
        public CSplitAFaceUsingTwoPoints(GeometrySelection surfaceSelection, GeometrySelection verticesSelection)
            : base("Split a face using two points")
        {
            _surfaceSelection = surfaceSelection.DeepClone();
            _verticesSelection = verticesSelection.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.SplitAFaceUsingTwoPoints(_surfaceSelection.DeepClone(), _verticesSelection.DeepClone());
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + "Surface: " + _surfaceSelection.ToString() + ", Vertices: " + _verticesSelection.ToString();
        }
    }
}
