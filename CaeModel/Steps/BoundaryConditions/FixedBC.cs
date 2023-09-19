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
    public class FixedBC : BoundaryCondition, ISerializable
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FixedBC(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, twoD, false, 0)
        {
        }
        public FixedBC(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
        }
        
        
        // Methods                                                                                                                  
        public int[] GetConstrainedDirections()
        {
            if (_twoD) return new int[] { 1, 2, 6 };
            else return new int[] { 1, 2, 3, 4, 5, 6 };
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
        }
    }
}
