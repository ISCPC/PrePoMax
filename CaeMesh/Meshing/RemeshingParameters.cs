using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class RemeshingParameters : MeshingParameters, IMultiRegion, ISerializable
    {
        // Variables                                                                                                                
        private RegionTypeEnum _regionType;         //ISerializable
        private string _regionName;                 //ISerializable


        // Properties                                                                                                               
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }


        // Constructors                                                                                                             
        public RemeshingParameters(string regionName, RegionTypeEnum regionType)
            : base("RemeshingParameters")
        {
            UseMmg = true;
            //
            _regionName = regionName;
            _regionType = regionType;
            _creationData = null;
            _creationIds = null;
        }
        public RemeshingParameters(string regionName, RegionTypeEnum regionType, MeshingParameters meshingParameters)
            : base(meshingParameters)
        {
            UseMmg = true;
            //
            _regionName = regionName;
            _regionType = regionType;
            _creationData = null;
            _creationIds = null;
        }
        public RemeshingParameters(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_regionType":
                        _regionType = (RegionTypeEnum)entry.Value; break;
                    case "_regionName":
                        _regionName = (string)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_regionName", _regionName, typeof(string));
        }
    }
}
