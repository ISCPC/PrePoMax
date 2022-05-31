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
        

        // Properties                                                                                                               


        // Constructors                                                                                                             
        public WidgetBase(string name, Controller controller)
            :base(name)
        {
            _controller = controller;
        }


        // Methods
        public abstract void GetWidgetData(out string text, out double[] coor);

    }
}
