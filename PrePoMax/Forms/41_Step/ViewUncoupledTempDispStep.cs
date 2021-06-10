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
    public class ViewUncoupledTempDispStep : ViewHeatTransferStep
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        

        // Constructors                                                                                                             
        public ViewUncoupledTempDispStep(CaeModel.UncoupledTempDispStep step)
            : base(step)
        {
            _dctd.GetProperty(nameof(Nlgeom)).SetIsBrowsable(true);
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _heatTransferStep;
        }
    }
}
