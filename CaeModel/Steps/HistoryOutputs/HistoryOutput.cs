using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeModel
{
    [Serializable]
    public enum TotalsTypeEnum
    {
        [StandardValue("Yes", Description = "Sum of external forces is printed in addition to the individual node values.")]
        Yes,
        [StandardValue("Only", Description = "Only sum of external forces is printed.")]
        Only,
        [StandardValue("No", Description = "Only individual node values are printed (default).")]
        No
    }

    [Serializable]
    public abstract class HistoryOutput : NamedClass, IMultiRegion
    {
        // Variables                                                                                                                
        private int _frequency;
        private RegionTypeEnum _regionType;
        private string _regionName;
        private TotalsTypeEnum _totals;
        private int[] _creationIds;
        private Selection _creationData;


        // Properties                                                                                                               
        public int Frequency
        {
            get { return _frequency; }
            set
            {
                if (value < 1) throw new Exception("The frequency value must be larger or equal to 1.");
                _frequency = value;
            }
        }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public TotalsTypeEnum TotalsType { get { return _totals; } set { _totals = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }

        // Constructors                                                                                                             
        public HistoryOutput(string name, string regionName, RegionTypeEnum regionType)
            : base(name)
        {
            _frequency = 1;
            _regionName = regionName;
            _regionType = regionType;
            _totals = TotalsTypeEnum.No;
            _creationIds = null;
            _creationData = null;
        }


        // Methods                                                                                                                  


    }
}
