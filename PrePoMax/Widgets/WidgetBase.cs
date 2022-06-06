using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace PrePoMax
{
    public abstract class WidgetBase : NamedClass
    {
        // Variables                                                                                                                
        protected int _partId;
        protected Controller _controller;
        protected string _overridenText;


        // Properties                                                                                                               
        public int PartId { get { return _partId; } set { _partId = value; } }
        public bool IsTextOverriden { get { return _overridenText != null; } }
        public string OverridenText { get { return _overridenText; } set { _overridenText = value; } }


        // Constructors                                                                                                             
        public WidgetBase(string name, Controller controller)
            : base(name)
        {
            _partId = -1;   // always visible
            _controller = controller;
            _overridenText = null;
        }


        // Methods
        public abstract void GetWidgetData(out string text, out double[] coor);
        public string GetWidgetText()
        {
            GetWidgetData(out string text, out double[] coor);
            return text;
        }
        public string GetNotOverridenWidgetText()
        {
            string tmp = _overridenText;
            _overridenText = null;
            //
            GetWidgetData(out string text, out double[] coor);
            //
            _overridenText = tmp;
            //
            return text;
        }
        public bool IsWidgetVisible()
        {
            if (_partId == -1) return true;
            //
            CaeMesh.FeMesh mesh = _controller.DisplayedMesh;
            if (mesh != null)
            {
                CaeMesh.BasePart part = mesh.GetPartById(_partId);
                if (part != null && part.Visible) return true;
            }
            //
            return false;
        }
    }
}
