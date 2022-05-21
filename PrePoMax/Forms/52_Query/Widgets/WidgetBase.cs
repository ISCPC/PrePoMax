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
        private double[] _anchorPoint;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public WidgetBase(string name)
            :base(name)
        {

        }

    }
}
