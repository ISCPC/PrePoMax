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
    public class SurfaceSpring : SpringConstraint, ISerializable
    {
        // Variables                                                                                                                
        private bool _stiffnessPerArea;         //ISerializable


        // Properties                                                                                                               
        public bool StiffnessPerArea { get { return _stiffnessPerArea; } set { SetStiffnessPerArea(value); } }


        // Constructors                                                                                                             
        public SurfaceSpring(string name, string regionName, RegionTypeEnum regionType, bool twoD, bool checkPositive)
            : base(name, regionName, regionType, twoD, checkPositive)
        {
            _stiffnessPerArea = false;
        }
        public SurfaceSpring(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            bool compatibility140 = false;
            //
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_stiffnessPerArea":
                        _stiffnessPerArea = (bool)entry.Value; break;
                    case "_k1":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double) compatibility140 = true; break;
                            default:
                        break;
                }
            }
            //
            if (compatibility140) UpdateStiffnessPerArea();
        }


        // Methods                                                                                                                  
        private void SetStiffnessPerArea(bool value)
        {
            _stiffnessPerArea = value;
            UpdateStiffnessPerArea();
        }
        private void UpdateStiffnessPerArea()
        {
            Type stringDoubleConverterType;
            if (_stiffnessPerArea) stringDoubleConverterType = typeof(StringForcePerVolumeConverter);
            else stringDoubleConverterType = typeof(StringForcePerLengthConverter);
            //
            K1.SetConverterType(stringDoubleConverterType);
            K2.SetConverterType(stringDoubleConverterType);
            K3.SetConverterType(stringDoubleConverterType);
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_stiffnessPerArea", _stiffnessPerArea, typeof(bool));
        }
    }
}
