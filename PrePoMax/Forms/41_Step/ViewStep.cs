using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public abstract class ViewStep
    {
        // Variables                                                                                                                
        protected DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the step.")]
        public abstract string Name { get; set; }

        [Browsable(false)]
        public abstract CaeModel.Step Base { get; set; }

        // Constructors                                                                                                             


        // Methods
        public abstract void UpdateFieldView();
    
     
    }
}
