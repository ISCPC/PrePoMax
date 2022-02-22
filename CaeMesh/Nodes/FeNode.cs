using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public struct FeNode
    {
        // Variables                                                                                                                
        public int Id;
        public double X;
        public double Y;
        public double Z;


        // Constructors                                                                                                             
        public FeNode(int id, double x, double y, double z)
        {
            Id = id;
            X = x;
            Y = y;
            Z = z;
        }
        public FeNode(int id, double[] coor)
            : this(id, coor[0], coor[1], coor[2])
        {
        }
        public FeNode(FeNode node)
            : this(node.Id, node.X, node.Y, node.Z)
        {
        }
       

        // Methods                                                                                                                  
        public void SetCoor(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public double[] Coor
        {
            get
            {
                return new double[] { X, Y, Z };
            }
            set
            {
                X = value[0];
                Y = value[1];
                Z = value[2];
            }
        }
        public bool IsEqual(FeNode node)
        {
            int div = 10000;
            // the <= sign solves the problem when a coordinate equals 0
            if (Id == node.Id && Math.Abs(X - node.X) <= Math.Abs(X / div) && Math.Abs(Y - node.Y) <= Math.Abs(Y / div) && Math.Abs(Z - node.Z) <= Math.Abs(Z / div))
                return true;
            else
                return false;
        }
        public FeNode DeepCopy()
        {
            return new FeNode(Id, X, Y, Z);
        }
    }
}
