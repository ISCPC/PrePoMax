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
    public class ViewBoundaryDisplacementStep : ViewStep
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        

        // Constructors                                                                                                             
        public ViewBoundaryDisplacementStep(CaeModel.BoundaryDisplacementStep step, bool installProvider = true)
            : base(step)
        {
            if (installProvider) InstallProvider();
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _step;
        }
        protected void InstallProvider()
        {
            _dctd = ProviderInstaller.Install(this);
            //
            UpdateVisibility();
        }
        public override void UpdateVisibility()
        {
            _dctd.GetProperty(nameof(SolverType)).SetIsBrowsable(false);
        }

    }
}
