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
    public class ViewBuckleStep : ViewStep
    {
        // Variables                                                                                                                
        private CaeModel.BuckleStep _step;


        // Properties                                                                                                               
        public override string Name { get { return _step.Name; } set { _step.Name = value; } }

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Perturbation")]
        [DescriptionAttribute("Perturbation parameter set to On applies preloads from the previous step if it exists.")]
        public bool Perturbation { get { return _step.Perturbation; } set { _step.Perturbation = value; } }

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Num. of buckling factors")]
        [DescriptionAttribute("Number of buckling factors desired (default: 1).")]
        public int NumBucklingFactors { get { return _step.NumOfBucklingFactors; } set { _step.NumOfBucklingFactors = value; } }

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(1, 10, "Accuracy")]
        [DescriptionAttribute("Accuracy desired (default: 0.01).")]
        public double Accuracy { get { return _step.Accuracy; } set { _step.Accuracy = value; } }

        public override CaeModel.Step Base { get { return _step; } set { _step = (CaeModel.BuckleStep)value; } }


        // Constructors                                                                                                             
        public ViewBuckleStep(CaeModel.BuckleStep step)
        {
            _step = step;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToOnOff("Perturbation");
            //
            UpdateFieldView();
        }


        // Methods
        public override void UpdateFieldView()
        {
        }

    }
}
