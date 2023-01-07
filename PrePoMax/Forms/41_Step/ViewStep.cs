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
    public class ViewStep
    {
        // Variables                                                                                                                
        protected CaeModel.Step _step;
        protected DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               

        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the step.")]
        public string Name { get { return _step.Name; } set { _step.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Solver")]
        [DescriptionAttribute("Select the matrix solver type.")]
        public CaeModel.SolverTypeEnum SolverType
        {
            get { return _step.SolverType; }
            set { _step.SolverType = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(11, 11, "No analysis")]
        [DescriptionAttribute("Select yes for input deck and geometry checking only. The set of equations is built but not solved " +
                              "(the Jacobian determinant is checked).")]
        public bool NoAnalysis
        {
            get { return _step.NoAnalysis; }
            set { _step.NoAnalysis = value; }
        }



        // Constructors                                                                                                             
        public ViewStep(CaeModel.Step step)
        {
            if (step == null)
                throw new ArgumentNullException();
            //
            _step = step;
            //
            _dctd.RenameBooleanPropertyToYesNo(nameof(NoAnalysis));
        }

        // Methods
        public virtual CaeModel.Step GetBase()
        {
            return _step;
        }
        public virtual void UpdateVisibility()
        { }
    }
}
