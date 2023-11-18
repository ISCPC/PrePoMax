using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class PointSpring : SpringConstraint, ISerializable
    {
        // Variables                                                                                                                
        private int _nodeId;                  //ISerializable


        // Properties                                                                                                               
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }


        // Constructors                                                                                                             
        public PointSpring(string name, string regionName, RegionTypeEnum regionType, bool twoD, bool checkPositive)
            : base(name, regionName, regionType, twoD, checkPositive)
        {
        }
        public PointSpring(string name, int nodeId, double k1, double k2, double k3, bool twoD, bool checkPositive)
           : base(name, null, RegionTypeEnum.NodeId, k1, k2, k3, twoD, checkPositive)
        {
            _nodeId = nodeId;
        }

        public PointSpring(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_nodeId":
                        _nodeId = (int)entry.Value; break;
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
            info.AddValue("_nodeId", _nodeId, typeof(int));
        }
    }
}
