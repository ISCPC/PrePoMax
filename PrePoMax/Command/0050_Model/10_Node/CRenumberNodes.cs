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
    class CRenumberNodes : Command
    {
        // Variables                                                                                                                
        private int _startNodeId;
        private ViewGeometryModelResults _view;

        // Constructor                                                                                                              
        public CRenumberNodes(int startNodeId, ViewGeometryModelResults currentView)
            : base("Renumber nodes")
        {
            _startNodeId = startNodeId;
            _view = currentView;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.CurrentView = _view;
            receiver.RenumberNodes(_startNodeId);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _startNodeId;
        }
    }
}
