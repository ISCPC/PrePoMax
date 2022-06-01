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
        protected Controller _controller;
        protected string _overridenText;


        // Properties                                                                                                               
        public bool IsTextOverriden { get { return _overridenText != null; } }
        public string OverridenText { get { return _overridenText; } set { _overridenText = value; } }


        // Constructors                                                                                                             
        public WidgetBase(string name, Controller controller)
            : base(name)
        {
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


    }
}
