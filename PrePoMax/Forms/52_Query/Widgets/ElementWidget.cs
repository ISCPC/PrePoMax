using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class ElementWidget : WidgetBase
    {
        // Variables                                                                                                                
        private int _elementId;


        // Properties                                                                                                               
        public int ElementId { get { return _elementId; } set { _elementId = value; } }


        // Constructors                                                                                                             
        public ElementWidget(string name, int elementId, Controller controller)
            : base(name, controller)
        {
            _elementId = elementId;
        }


        //
        public override void GetWidgetData(out string text, out double[] coor)
        {
            text = string.Format("Element id: {0}", _elementId);
            //
            string elementType = _controller.GetElementType(_elementId);
            if (elementType != null)
            {
                text += string.Format("{0}Element type: {1}", Environment.NewLine, elementType);
            }
            coor = _controller.GetElement(_elementId).GetCG(_controller.DisplayedMesh.Nodes);
        }
    }
}
