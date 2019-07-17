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

        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Frequency")]
        [DescriptionAttribute("Integer N, which indicates that only results of every N-th increment will be stored.")]
        public abstract int Frequency { get ; set; }


        [Browsable(false)]
        public abstract CaeModel.FieldOutput Base { get; set; }

        // Constructors                                                                                                             


        // Methods
       
    }
}
