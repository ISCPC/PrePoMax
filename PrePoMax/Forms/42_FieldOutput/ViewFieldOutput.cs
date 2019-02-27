using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace PrePoMax
{
    [Serializable]
    public abstract class ViewFieldOutput
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the field output.")]
        public abstract string Name { get; set; }

      
        [Browsable(false)]
        public abstract CaeModel.FieldOutput Base { get; set; }

        // Constructors                                                                                                             


        // Methods
       
    }
}
