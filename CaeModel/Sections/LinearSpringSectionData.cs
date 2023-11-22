using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class LinearSpringSectionData : SectionData, ISerializable
    {
        // Variables                                                                                                                
        private int _direction;                 //ISerializable
        private double _stiffness;              //ISerializable


        // Properties                                                                                                               
        public int Direction { get { return _direction; } set { _direction = value; } }
        public double Stiffness { get { return _stiffness; } set { _stiffness = value; } }


        // Constructors                                                                                                             
        public LinearSpringSectionData(string name, string elementSetName, int direction, double stiffness)
            : base(name, null, elementSetName, RegionTypeEnum.ElementSetName, 1)
        {
            _direction = direction;
            _stiffness= stiffness;
        }
        public LinearSpringSectionData(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_direction":
                        _direction = (int)entry.Value;
                        break;
                    case "_stiffness":
                        _stiffness = (double)entry.Value;
                        break;
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
            info.AddValue("_direction", _direction, typeof(int));
            info.AddValue("_stiffness", _stiffness, typeof(double));
        }
    }
}
