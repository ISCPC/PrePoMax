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
        private CustomPropertyDescriptor cpd = null;
        private CaeModel.PreTensionLoad _preTenLoad;


        // Properties                                                                                                               
        public override string Name { get { return _preTenLoad.Name; } set { _preTenLoad.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Type")]
        [DescriptionAttribute("Select the pre-tension type.")]
        public CaeModel.PreTensionLoadType Type 
        { 
            get { return _preTenLoad.Type; } 
            set 
            { 
                _preTenLoad.Type = value;
                //
                if (_preTenLoad.Type == CaeModel.PreTensionLoadType.Force && double.IsInfinity(_preTenLoad.Magnitude))
                    _preTenLoad.Magnitude = 0;
                //
                cpd = base.DynamicCustomTypeDescriptor.GetProperty("ForceMagnitude");
                cpd.SetIsBrowsable(_preTenLoad.Type == CaeModel.PreTensionLoadType.Force);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty("DisplacementMagnitude");
                cpd.SetIsBrowsable(_preTenLoad.Type == CaeModel.PreTensionLoadType.Displacement);
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface which will be used for the load.")]
        public string SurfaceName { get { return _preTenLoad.SurfaceName; } set { _preTenLoad.SurfaceName = value; } }
        //
        [CategoryAttribute("Pre-tension direction")]
        [OrderedDisplayName(1, 10, "Auto compute")]
        [DescriptionAttribute("Auto compute the pre-tension direction.")]
        [Id(1, 2)]
        public bool AutoComputeDirection 
        { 
            get { return _preTenLoad.AutoComputeDirection; }
            set 
            {
                _preTenLoad.AutoComputeDirection = value;
                //
                cpd = base.DynamicCustomTypeDescriptor.GetProperty("X");
                cpd.SetIsBrowsable(!_preTenLoad.AutoComputeDirection);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty("Y");
                cpd.SetIsBrowsable(!_preTenLoad.AutoComputeDirection);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty("Z");
                cpd.SetIsBrowsable(!_preTenLoad.AutoComputeDirection);
            }
        }
        //
        [CategoryAttribute("Pre-tension direction")]
        [OrderedDisplayName(2, 10, "X")]
        [DescriptionAttribute("X component of the pre-tension direction.")]
        [Id(1, 2)]
        public double X { get { return _preTenLoad.X; } set { _preTenLoad.X = value; } }
        //
        [CategoryAttribute("Pre-tension direction")]
        [OrderedDisplayName(3, 10, "Y")]
        [DescriptionAttribute("Y component of the pre-tension direction.")]
        [Id(1, 2)]
        public double Y { get { return _preTenLoad.Y; } set { _preTenLoad.Y = value; } }
        //
        [CategoryAttribute("Pre-tension direction")]
        [OrderedDisplayName(4, 10, "Z")]
        [DescriptionAttribute("Z component of the pre-tension direction.")]
        [Id(1, 2)]
        public double Z { get { return _preTenLoad.Z; } set { _preTenLoad.Z = value; } }
        //
        [CategoryAttribute("Force magnitude")]
        [OrderedDisplayName(5, 10, "Magnitude")]
        [DescriptionAttribute("Force magnitude for the pre-tension.")]
        [Id(1, 3)]
        public double ForceMagnitude { get { return _preTenLoad.Magnitude; } set { _preTenLoad.Magnitude = value; } }
        //
        [CategoryAttribute("Displacement magnitude")]
        [OrderedDisplayName(5, 10, "Magnitude")]
        [DescriptionAttribute("Displacement magnitude for the pre-tension.")]
        [TypeConverter(typeof(StringConstrainedDOFConverter))]
        [Id(1, 3)]
        public double DisplacementMagnitude { get { return _preTenLoad.Magnitude; } set { _preTenLoad.Magnitude = value; } }


        // Constructors                                                                                                             
        public ViewPreTensionLoad(CaeModel.PreTensionLoad preTenLoad)
        {
            // The order is important
            _preTenLoad = preTenLoad;
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // Category sorting
            base.DynamicCustomTypeDescriptor.CategorySortOrder = CustomSortOrder.AscendingById;
            // Now lets display Yes/No instead of True/False
            base.DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo("AutoComputeDirection");
            // Set initial visibilities
            Type = _preTenLoad.Type;
            AutoComputeDirection = _preTenLoad.AutoComputeDirection;
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
