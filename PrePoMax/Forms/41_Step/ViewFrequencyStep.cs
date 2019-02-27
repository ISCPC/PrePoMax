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
    public class ViewFrequencyStep : ViewStep
    {
        // Variables                                                                                                                
        private CaeModel.FrequencyStep _step;


        // Properties                                                                                                               
        public override string Name { get { return _step.Name; } set { _step.Name = value; } }

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Number of frequencies")]
        [DescriptionAttribute("Number of eigenfrequencies to compute.")]
        public double NumOfFrequencies { get { return _step.NumOfFrequencies; } set { _step.NumOfFrequencies = value; } }

        public override CaeModel.Step Base { get { return _step; } set { _step = (CaeModel.FrequencyStep)value; } }


        // Constructors                                                                                                             
        public ViewFrequencyStep(CaeModel.FrequencyStep step)
        {
            _step = step;
            _dctd = ProviderInstaller.Install(this);

            UpdateFieldView();
        }


        // Methods
        public override void UpdateFieldView()
        {
        }

    }
}
