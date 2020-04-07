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
    public class ViewTie : ViewConstraint
    {
        // Variables                                                                                                                
        private CaeModel.Tie _tie;
        private DynamicCustomTypeDescriptor _dctd = null;

        // Properties                                                                                                               
        public override string Name { get { return _tie.Name; } set { _tie.Name = value; } }

        [OrderedDisplayName(1, 10, "Slave surface")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the slave surface which will be used for the tie definition.")]
        [Id(2, 1)]
        public string SlaveSurfaceName { get { return _tie.SlaveSurfaceName; } set { _tie.SlaveSurfaceName = value; } }

        [OrderedDisplayName(2, 10, "Master surface")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the master surface which will be used for the tie definition.")]
        [Id(3, 1)]
        public string MasterSurfaceName { get { return _tie.MasterSurfaceName; } set { _tie.MasterSurfaceName = value; } }

        [OrderedDisplayName(3, 10, "Position tolerance")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Enter the value of largest distance from the master surface for which the slave nodes will be included in the tie constraint."
                              + " Default value equals 2.5 % of the typical element size.")]
        [TypeConverter(typeof(StringPosTolConverter))]
        [Id(4, 1)]
        public double PositionTolerance { get { return _tie.PositionTolerance; } set { _tie.PositionTolerance = value; } }

        [OrderedDisplayName(4, 10, "Adjust")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Set adjust to No to prevent the projection of the slave nodes on the master surface.")]
        [Id(5, 1)]
        public bool Adjust { get { return _tie.Adjust; } set { _tie.Adjust = value; } }


        // Constructors                                                                                                             
        public ViewTie(CaeModel.Tie tie)
        {
            _tie = tie;
            _dctd = ProviderInstaller.Install(this);

            ApplyAdjustYesNo();
        }


        // Methods                                                                                                                  
        public override CaeModel.Constraint GetBase()
        {
            return _tie;
        }
        protected void ApplyAdjustYesNo()
        {
            CustomPropertyDescriptor cpd = _dctd.GetProperty("Adjust");

            foreach (StandardValueAttribute sva in cpd.StatandardValues)
            {
                if ((bool)sva.Value == true) sva.DisplayName = "Yes";
                else sva.DisplayName = "No";
            }
        }
        public void PopululateDropDownLists(string[] surfaceNames)
        {
            _dctd.PopulateProperty(() => this.SlaveSurfaceName, surfaceNames);
            _dctd.PopulateProperty(() => this.MasterSurfaceName, surfaceNames);
        }
    }

}
