using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Drawing;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public abstract class Constraint : NamedClass, IMasterSlaveMultiRegion, ISerializable
    {
        // Variables                                                                                                                
        private string _masterRegionName;               //ISerializable
        private RegionTypeEnum _masterRegionType;       //ISerializable
        private string _slaveRegionName;                //ISerializable
        private RegionTypeEnum _slaveRegionType;        //ISerializable
        //
        private int[] _masterCreationIds;               //ISerializable
        private Selection _masterCreationData;          //ISerializable
        private int[] _slaveCreationIds;                //ISerializable
        private Selection _slaveCreationData;           //ISerializable
        //
        protected Color _masterColor;                   //ISerializable
        protected Color _slaveColor;                    //ISerializable
        //
        private bool _twoD;                             //ISerializable


        // Properties                                                                                                                       
        public string MasterRegionName { get { return _masterRegionName; } set { _masterRegionName = value; } }
        public RegionTypeEnum MasterRegionType { get { return _masterRegionType; } set { _masterRegionType = value; } }
        public string SlaveRegionName { get { return _slaveRegionName; } set { _slaveRegionName = value; } }
        public RegionTypeEnum SlaveRegionType { get { return _slaveRegionType; } set { _slaveRegionType = value; } }
        //
        public int[] MasterCreationIds { get { return _masterCreationIds; } set { _masterCreationIds = value; } }
        public Selection MasterCreationData { get { return _masterCreationData; } set { _masterCreationData = value; } }
        public int[] SlaveCreationIds { get { return _slaveCreationIds; } set { _slaveCreationIds = value; } }
        public Selection SlaveCreationData { get { return _slaveCreationData; } set { _slaveCreationData = value; } }
        //
        public Color MasterColor { get { return _masterColor; } set { _masterColor = value; } }
        public Color SlaveColor { get { return _slaveColor; } set { _slaveColor = value; } }
        //
        public bool TwoD { get { return _twoD; } }


        // Constructors                                                                                                             
        public Constraint(string name, string masterRegionName, RegionTypeEnum masterRegionType,
                          string slaveRegionName, RegionTypeEnum slaveRegionType, bool twoD)
            : base(name) 
        {
            _masterRegionName = masterRegionName;
            _masterRegionType = masterRegionType;
            _slaveRegionName = slaveRegionName;
            _slaveRegionType = slaveRegionType;
            //
            _masterCreationIds = null;
            _masterCreationData = null;
            _slaveCreationIds = null;
            _slaveCreationData = null;
            //
            _masterColor = Color.Yellow;
            _slaveColor = Color.Yellow;
            //
            _twoD = twoD;
        }
        public Constraint(SerializationInfo info, StreamingContext context)
            : base (info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_masterRegionName":
                        _masterRegionName = (string)entry.Value; break;
                    case "_masterRegionType":
                        _masterRegionType = (RegionTypeEnum)entry.Value; break;
                    case "_slaveRegionName":
                        _slaveRegionName = (string)entry.Value; break;
                    case "_slaveRegionType":
                        _slaveRegionType = (RegionTypeEnum)entry.Value; break;
                    //
                    case "_masterCreationIds":
                        _masterCreationIds = (int[])entry.Value; break;
                    case "_masterCreationData":
                        _masterCreationData = (Selection)entry.Value; break;
                    case "_slaveCreationIds":
                        _slaveCreationIds = (int[])entry.Value; break;
                    case "_slaveCreationData":
                        _slaveCreationData = (Selection)entry.Value; break;
                    //
                    case "_masterColor":
                        _masterColor = (Color)entry.Value; break;
                    case "_slaveColor":
                        _slaveColor = (Color)entry.Value; break;
                    //
                    case "_twoD":
                        _twoD = (bool)entry.Value; break;
                    //
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_masterRegionName", _masterRegionName, typeof(string));
            info.AddValue("_masterRegionType", _masterRegionType, typeof(RegionTypeEnum));
            info.AddValue("_slaveRegionName", _slaveRegionName, typeof(string));
            info.AddValue("_slaveRegionType", _slaveRegionType, typeof(RegionTypeEnum));
            //
            info.AddValue("_masterCreationIds", _masterCreationIds, typeof(int[]));
            info.AddValue("_masterCreationData", _masterCreationData, typeof(Selection));
            info.AddValue("_slaveCreationIds", _slaveCreationIds, typeof(int[]));
            info.AddValue("_slaveCreationData", _slaveCreationData, typeof(Selection));
            //
            info.AddValue("_masterColor", _masterColor, typeof(Color));
            info.AddValue("_slaveColor", _slaveColor, typeof(Color));
            //
            info.AddValue("_twoD", _twoD, typeof(bool));
        }

    }
}
