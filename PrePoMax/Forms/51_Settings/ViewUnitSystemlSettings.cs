using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Settings
{
    [Serializable]
    public class ViewUnitSystemlSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private UnitSystemSettings _unitSystemSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Unit system")]
        [OrderedDisplayName(0, 10, "Model unit system type")]
        [DescriptionAttribute("Select the unit system type for the geometry and FE model.")]
        public UnitSystemType ModelUnitSystemType
        {
            get { return _unitSystemSettings.ModelUnitSystemType; }
            set { _unitSystemSettings.ModelUnitSystemType = value; }
        }
        //
        [CategoryAttribute("Unit system")]
        [OrderedDisplayName(1, 10, "Results unit system type")]
        [DescriptionAttribute("Select the unit system type for the results")]
        public UnitSystemType ResultsUnitSystemType
        {
            get { return _unitSystemSettings.ModelUnitSystemType; }
            set { _unitSystemSettings.ModelUnitSystemType = value; }
        }


        // Constructors                                                                                                             
        public ViewUnitSystemlSettings(UnitSystemSettings unitSystemSettings)
        {
            _unitSystemSettings = unitSystemSettings;
            _dctd = ProviderInstaller.Install(this);
        }

        // Methods                                                                                                                  
        public ISettings GetBase()
        {
            return _unitSystemSettings;
        }
        public void Reset()
        {
            _unitSystemSettings.Reset();
        }
    }

}
