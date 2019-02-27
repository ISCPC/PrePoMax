using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using DynamicTypeDescriptor;
using System.ComponentModel;
using CaeGlobals;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewDLoad : ViewLoad
    {
        // Variables                                                                                                                
        private DLoad _dLoad;


        // Properties                                                                                                               
        public override string Name { get { return _dLoad.Name; } set { _dLoad.Name = value; } }

        [OrderedDisplayName(1, 10, "Surface")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the surface which will be used for the load.")]
        public string SurfaceName { get { return _dLoad.SurfaceName; } set {_dLoad.SurfaceName = value;} }

        [OrderedDisplayName(2, 10, "Magnitude")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Magnitude of the pressure load.")]
        public double Magnitude { get { return _dLoad.Magnitude; } set { _dLoad.Magnitude = value; } }

        // Constructors                                                                                                             
        public ViewDLoad(DLoad dLoad)
        {
            _dLoad = dLoad;
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }

        // Methods                                                                                                                  
        public override Load GetBase()
        {
            return _dLoad;
        }
        public void PopululateDropDownLists(string[] surfaceNames)
        {
            base.DynamicCustomTypeDescriptor.PopulateProperty(() => this.SurfaceName, surfaceNames);
        }
    }
}
