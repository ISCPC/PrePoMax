using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class ElementAnnotation : AnnotationBase
    {
        // Variables                                                                                                                
        private int _elementId;


        // Properties                                                                                                               
        public int ElementId { get { return _elementId; } set { _elementId = value; } }


        // Constructors                                                                                                             
        public ElementAnnotation(string name, int elementId)
            : base(name)
        {
            _elementId = elementId;
            _partId = Controller.DisplayedMesh.Elements[_elementId].PartId;
        }


        //
        public override void GetAnnotationData(out string text, out double[] coor)
        {
            // Item name
            string itemName = "Element id:";
            if (Controller.CurrentView == ViewGeometryModelResults.Geometry) itemName = "Facet id:";
            //
            text = string.Format("{0} {1}", itemName, _elementId);
            //
            if (Controller.CurrentView == ViewGeometryModelResults.Model)
            {
                string elementType = Controller.GetElementType(_elementId);
                if (elementType != null)
                {
                    text += string.Format("{0}Element type: {1}", Environment.NewLine, elementType);
                }
            }
            coor = Controller.GetElement(_elementId).GetCG(Controller.DisplayedMesh.Nodes);
            //
            if (IsTextOverriden) text = OverridenText;
        }
    }
}
