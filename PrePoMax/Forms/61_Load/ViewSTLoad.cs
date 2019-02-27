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
    public class ViewSTLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.STLoad _stLoad;


        // Properties                                                                                                               
        public override string Name { get { return _stLoad.Name; } set { _stLoad.Name = value; } }

        [OrderedDisplayName(1, 10, "Surface")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the surface which will be used for the load.")]
        public string SurfaceName { get { return _stLoad.SurfaceName; } set { _stLoad.SurfaceName = value; } }

        [OrderedDisplayName(2, 10, "F1")]
        [CategoryAttribute("Force components")]
        [DescriptionAttribute("Force in the direction of the first axis.")]
        public double F1 { get { return _stLoad.F1; } set { _stLoad.F1 = value; } }
        
        [OrderedDisplayName(3, 10, "F2")]
        [CategoryAttribute("Force components")]
        [DescriptionAttribute("Force in the direction of the second axis.")]
        public double F2 { get { return _stLoad.F2; } set { _stLoad.F2 = value; } }

        [OrderedDisplayName(4, 10, "F3")]
        [CategoryAttribute("Force components")]
        [DescriptionAttribute("Force in the direction of the third axis.")]
        public double F3 { get { return _stLoad.F3; } set { _stLoad.F3 = value; } }

        [OrderedDisplayName(5, 10, "Magnitude")]
        [CategoryAttribute("Force magnitude")]
        [DescriptionAttribute("Force magnitude.")]
        public double Flength
        {
            get { return Math.Sqrt(_stLoad.F1 * _stLoad.F1 + _stLoad.F2 * _stLoad.F2 + _stLoad.F3 * _stLoad.F3); }
            set
            {
                if (value <= 0)
                    throw new Exception("The magnitude of the force must be greater than 0.");

                double len = Math.Sqrt(_stLoad.F1 * _stLoad.F1 + _stLoad.F2 * _stLoad.F2 + _stLoad.F3 * _stLoad.F3);
                double r;
                if (len == 0) r = 0;
                else r = value / len;
                _stLoad.F1 *= r;
                _stLoad.F2 *= r;
                _stLoad.F3 *= r;
            }
        }


        // Constructors                                                                                                             
        public ViewSTLoad(CaeModel.STLoad stLoad)
        {
            // the order is important
            _stLoad = stLoad;
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }



        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _stLoad;
        }

        public void PopululateDropDownLists(string[] surfaceNames)
        {
            base.DynamicCustomTypeDescriptor.PopulateProperty(() => this.SurfaceName, surfaceNames);
        }
    }

}
