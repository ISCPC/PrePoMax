using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class NodeWidget : WidgetBase
    {
        // Variables                                                                                                                
        private int _nodeId;


        // Properties                                                                                                               
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }


        // Constructors                                                                                                             
        public NodeWidget(string name, int nodeId)
            : base(name)
        {
            _nodeId = nodeId;
        }

    }
}
