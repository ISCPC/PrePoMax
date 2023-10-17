using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;


namespace CaeModel
{
    [Serializable]
    public class Tie : Constraint, ISerializable
    {
        // Variables                                                                                                                
        private static string _nonNegative = "The value must be larger or equal to 0.";
        private EquationContainer _positionTolerance;       //ISerializable
        private bool _adjust;                               //ISerializable


        // Properties                                                                                                               
        public EquationContainer PositionTolerance { get { return _positionTolerance; } set { SetPositionTolerance(value); } }
        public bool Adjust { get { return _adjust; } set { _adjust = value; } }       


        // Constructors                                                                                                             
        public Tie(string name, string masterSurfaceName, RegionTypeEnum masterRegionType,
                   string slaveSurfaceName, RegionTypeEnum slaveRegionType, bool twoD)
           : this(name, double.NaN, true, masterSurfaceName, masterRegionType, slaveSurfaceName, slaveRegionType, twoD)
        {
        }
        public Tie(string name, double positionTolerance, bool adjust, string masterSurfaceName, RegionTypeEnum masterRegionType,
                   string slaveSurfaceName, RegionTypeEnum slaveRegionType, bool twoD)
            : base(name, masterSurfaceName, masterRegionType, slaveSurfaceName, slaveRegionType, twoD)
        {
            if (masterRegionType == RegionTypeEnum.SurfaceName && slaveRegionType == RegionTypeEnum.SurfaceName &&
                slaveSurfaceName == masterSurfaceName) throw new CaeException("Master and slave surface names must be different.");
            //
            PositionTolerance = new EquationContainer(typeof(StringLengthDefaultConverter), positionTolerance);
            _adjust = adjust;
        }
        public Tie(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            bool foundPosTol = false;   // Compatibility for version v1.4.0 ?
            bool foundAdjust = false;   // Compatibility for version v1.4.0 ?
            //
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_positionTolerance":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueDouble)
                        {
                            if (valueDouble < 0) valueDouble = double.NaN; // Compatibility for version v1.4.0 ?
                            PositionTolerance = new EquationContainer(typeof(StringLengthDefaultConverter), valueDouble);
                        }
                        else
                            SetPositionTolerance((EquationContainer)entry.Value, false);
                        foundPosTol = true;
                        break;
                    case "_adjust":
                        _adjust = (bool)entry.Value; foundAdjust = true; break;
                    // Compatibility for version v1.1.1
                    case "_masterSurfaceName":
                        MasterRegionName = (string)entry.Value; break;
                    // Compatibility for version v1.1.1
                    case "_slaveSurfaceName":
                        SlaveRegionName = (string)entry.Value; break;
                    default:
                        break;
                }
            }
            // Compatibility for version v1.4.0 ?
            if (!foundPosTol) PositionTolerance = new EquationContainer(typeof(StringLengthDefaultConverter), double.NaN);
            if (!foundAdjust) _adjust = true;
        }


        // Methods                                                                                                                  
        private void SetPositionTolerance(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _positionTolerance, value, CheckPositionTolerance, checkEquation);
        }
        //
        private double CheckPositionTolerance(double value)
        {
            if (double.IsNaN(value) || value >= 0) return value;
            else throw new CaeException(_nonNegative);
        }
        public override void CheckEquations()
        {
            _positionTolerance.CheckEquation();
        }
        //
        public void SwapMasterSlave()
        {
            if (_name.Contains(Globals.MasterSlaveSeparator))
            {
                string[] tmp = _name.Split(new string[] { Globals.MasterSlaveSeparator }, StringSplitOptions.None);
                if (tmp.Length == 2) _name = tmp[1] + Globals.MasterSlaveSeparator + tmp[0];
            }
            //
            RegionTypeEnum tmpRegionType = MasterRegionType;
            MasterRegionType = SlaveRegionType;
            SlaveRegionType = tmpRegionType;
            //
            string tmpSurfaceName = MasterRegionName;
            MasterRegionName = SlaveRegionName;
            SlaveRegionName = tmpSurfaceName;
            //
            int[] tmpCreationIds = MasterCreationIds;
            MasterCreationIds = SlaveCreationIds;
            SlaveCreationIds = tmpCreationIds;
            //
            Selection tmpCreationData = MasterCreationData;
            MasterCreationData = SlaveCreationData;
            SlaveCreationData = tmpCreationData;
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_positionTolerance", _positionTolerance, typeof(EquationContainer));
            info.AddValue("_adjust", _adjust, typeof(bool));
        }


    }
}
