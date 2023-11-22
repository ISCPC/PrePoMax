using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;
using System.Drawing;

namespace CaeModel
{
    [Serializable]
    public abstract class SectionData : NamedClass, IMultiRegion, ISerializable
    {
        // Variables                                                                                                                
        private string _materialName;               //ISerializable
        private string _regionName;                 //ISerializable
        private RegionTypeEnum _regionType;         //ISerializable
        private int[] _creationIds;                 //ISerializable
        private Selection _creationData;            //ISerializable
        private double _thickness;                  //ISerializable


        // Properties                                                                                                               
        public string MaterialName { get { return _materialName; } set { _materialName = value; } }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public double Thickness { get { return _thickness; } set { _thickness = value; } }


        // Constructors                                                                                                             
        public SectionData(string name, string materialName, string regionName, RegionTypeEnum regionType, double thickness) 
            : base(name)
        {
            _materialName = materialName;
            _regionName = regionName;
            _regionType = regionType;
            _creationIds = null;
            _creationData = null;
            _thickness = thickness;
        }
        public SectionData(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_materialName":
                        _materialName = (string)entry.Value; break;
                    case "_regionName":
                        _regionName = (string)entry.Value; break;
                    case "_regionType":
                        _regionType = (RegionTypeEnum)entry.Value; break;
                    case "_creationIds":
                        _creationIds = (int[])entry.Value; break;
                    case "_creationData":
                        _creationData = (Selection)entry.Value; break;
                    case "_thickness":
                        _thickness = (double)entry.Value; break;
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
            info.AddValue("_materialName", _materialName, typeof(string));
            info.AddValue("_regionName", _regionName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_creationIds", _creationIds, typeof(int[]));
            info.AddValue("_creationData", _creationData, typeof(Selection));
            info.AddValue("_thickness", _thickness, typeof(double));
        }

    }

}
