using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public enum DOFType
    {
        Free,
        Zero,
        Fixed,
        Prescribed
    }

    [Serializable]
    public class DisplacementRotation : BoundaryCondition
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        public double U1 { get; set; }
        public double U2 { get; set; }
        public double U3 { get; set; }
        public double UR1 { get; set; }
        public double UR2 { get; set; }
        public double UR3 { get; set; }


        // Constructors                                                                                                             
        public DisplacementRotation(string name, string regionName, RegionTypeEnum regionType)
            : base(name, regionName, regionType) 
        {
            U1 = double.NaN;
            U2 = double.NaN;
            U3 = double.NaN;
            UR1 = double.NaN;
            UR2 = double.NaN;
            UR3 = double.NaN;
        }


        // Methods                                                                                                                  
        public int[] GetConstrainedDirections()
        {
            List<int> directions = new List<int>();
            if (!double.IsNaN(U1)) directions.Add(1);
            if (!double.IsNaN(U2)) directions.Add(2);
            if (!double.IsNaN(U3)) directions.Add(3);
            if (!double.IsNaN(UR1)) directions.Add(4);
            if (!double.IsNaN(UR2)) directions.Add(5);
            if (!double.IsNaN(UR3)) directions.Add(6);
            return directions.ToArray();
        }
        public int GetConstraintHash()
        {
            int[] dir = GetConstrainedDirections();
            int hash = 0;
            for (int i = 0; i < dir.Length; i++) hash += (int)Math.Pow(2, dir[i]);
            return hash;
        }
        public double[] GetConstrainValues()
        {
            List<double> values = new List<double>();
            if (!double.IsNaN(U1)) values.Add(U1);
            if (!double.IsNaN(U2)) values.Add(U2);
            if (!double.IsNaN(U3)) values.Add(U3);
            if (!double.IsNaN(UR1)) values.Add(UR1);
            if (!double.IsNaN(UR2)) values.Add(UR2);
            if (!double.IsNaN(UR3)) values.Add(UR3);
            return values.ToArray();
        }
        public int[] GetFixedDirections()
        {
            List<int> directions = new List<int>();
            if (double.IsPositiveInfinity(U1)) directions.Add(1);
            if (double.IsPositiveInfinity(U2)) directions.Add(2);
            if (double.IsPositiveInfinity(U3)) directions.Add(3);
            if (double.IsPositiveInfinity(UR1)) directions.Add(4);
            if (double.IsPositiveInfinity(UR2)) directions.Add(5);
            if (double.IsPositiveInfinity(UR3)) directions.Add(6);
            return directions.ToArray();
        }
        public bool IsProperlyDefined(out string error)
        {
            error = "";
            if (GetConstrainedDirections().Length + GetFixedDirections().Length == 0)
            {
                error = "At least one degree of freedom must be defined for the boundary condition.";
                return false;
            }
            if (GetFixedDirections().Length != 0 && GetConstrainedDirections().Length != GetFixedDirections().Length)
            {
                error = "Only fixed and unconstrained degrees of freedom can be used at the same time.";
                return false;
            }
            return true;
        }
        public DOFType GetDofType(int dof)
        {
            double value;
            switch (dof)
            {
                case 1: value = U1; break;
                case 2: value = U2; break;
                case 3: value = U3; break;
                case 4: value = UR1; break;
                case 5: value = UR2; break;
                case 6: value = UR3; break;
                default: throw new NotSupportedException();
            }
            if (double.IsNaN(value)) return DOFType.Free;
            else if (double.IsPositiveInfinity(value)) return DOFType.Fixed;
            else if (value == 0) return DOFType.Zero;
            else return DOFType.Prescribed;
        }
    }
}
