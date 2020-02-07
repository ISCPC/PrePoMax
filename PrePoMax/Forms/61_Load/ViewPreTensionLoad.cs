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
    public class ViewPreTensionLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.PreTensionLoad _preTenLoad;


        // Properties                                                                                                               
        public override string Name { get { return _preTenLoad.Name; } set { _preTenLoad.Name = value; } }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Surface")]
        [DescriptionAttribute("Select the surface which will be used for the load.")]
        public string SurfaceName { get { return _preTenLoad.SurfaceName; } set { _preTenLoad.SurfaceName = value; } }
        //
        [CategoryAttribute("Force direction")]
        [OrderedDisplayName(2, 10, "X")]
        [DescriptionAttribute("X component of the force direction.")]
        public double X { get { return _preTenLoad.X; } set { _preTenLoad.X = value; } }
        //
        [CategoryAttribute("Force direction")]
        [OrderedDisplayName(3, 10, "Y")]
        [DescriptionAttribute("Y component of the force direction.")]
        public double Y { get { return _preTenLoad.Y; } set { _preTenLoad.Y = value; } }
        //
        [CategoryAttribute("Force direction")]
        [OrderedDisplayName(4, 10, "Z")]
        [DescriptionAttribute("Z component of the force direction.")]
        public double Z { get { return _preTenLoad.Z; } set { _preTenLoad.Z = value; } }
        //
        [CategoryAttribute("Force magnitude")]
        [OrderedDisplayName(5, 10, "Magnitude")]
        [DescriptionAttribute("Force magnitude.")]
        public double ForceMagnitude { get { return _preTenLoad.ForceMagnitude; } set { _preTenLoad.ForceMagnitude = value; } }


        // Constructors                                                                                                             
        public ViewPreTensionLoad(CaeModel.PreTensionLoad preTenLoad)
        {
            // The order is important
            _preTenLoad = preTenLoad;
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _preTenLoad;
        }

        public void PopululateDropDownLists(string[] surfaceNames)
        {
            base.DynamicCustomTypeDescriptor.PopulateProperty(() => this.SurfaceName, surfaceNames);
        }
    }

}
