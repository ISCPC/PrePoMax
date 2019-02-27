using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PrePoMax.PropertyViews
{
    [Serializable]
    public class ViewDensity : IViewMaterialProperty
    {
        // Variables                                                                                                                
        private CaeModel.Density _density;

        // Properties                                                                                                               
        [Browsable(false)]
        public string Name
        {
            get { return "Density"; }
        }

        [Browsable(false)]
        public CaeModel.MaterialProperty Base
        {
            get { return _density; }
        }

        [CategoryAttribute("Data"),
        DisplayName("Density"),
        DescriptionAttribute("The value of the density.")]
        public double Value { get { return _density.Value; } set { _density.Value = value; } }

        // Constructors                                                                                                             
        public ViewDensity(CaeModel.Density density)
        {
            _density = density;
        }

    }
}
