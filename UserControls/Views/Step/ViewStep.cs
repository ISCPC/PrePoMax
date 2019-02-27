using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace UserControls
{
    [Serializable]
    public abstract class ViewStep
    {
        // Variables                                                                                                                



        // Properties                                                                                                               

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the step.")]
        public abstract string Name { get; set; }

        //public abstract Dictionary<string, CaeModel.BoundaryCondition> BoundaryConditions { get; }
        //public abstract Dictionary<string, CaeModel.Load> Loads { get; }
        //public abstract Dictionary<string, CaeModel.FieldOutput> FieldOutputs { get; }
        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(1, 10, "Nlgeom")]
        [DescriptionAttribute("Enable/disable the nonlinear effects of large deformations and large displacements.")]
        public abstract bool Nlgeom { get; set; }


        [Browsable(false)]
        public abstract CaeModel.Step Base { get; set; }

        // Constructors                                                                                                             


        // Methods
     
    }
}
