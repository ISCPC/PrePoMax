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
        private static string _positive = "The value must be larger than 0.";
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
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_positionTolerance":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueDouble)
                            PositionTolerance = new EquationContainer(typeof(StringLengthDefaultConverter), valueDouble);
                        else
                            SetPositionTolerance((EquationContainer)entry.Value, false);
                        break;
                    case "_adjust":
                        _adjust = (bool)entry.Value; break;
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
        }


        // Methods                                                                                                                  
        private void SetPositionTolerance(EquationContainer value, bool checkEquation = true)
        {
            value.CheckValue = CheckPositionTolerance;
            if (checkEquation) value.CheckEquation();
            //
            _positionTolerance = value;
        }
        private double CheckPositionTolerance(double value)
        {
            if (double.IsNaN(value) || value > 0) return value;
            else throw new CaeException(_positive);
        }
        public void CheckEquations()
        {
            _positionTolerance.CheckEquation();
        }
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
