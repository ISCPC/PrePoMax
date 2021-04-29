using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    class BoundingBoxVolmeComparer : IComparer<BoundingBox>
    {
        public int Compare(BoundingBox bb1, BoundingBox bb2)
        {
            // Use rounding to eliminate numerical error - different exploded view
            double v1 = Tools.RoundToSignificantDigits(bb1.GetVolume(), 6);
            double v2 = Tools.RoundToSignificantDigits(bb2.GetVolume(), 6);
            //
            if (v1 < v2) return 1;
            else if (v1 > v2) return -1;
            else return 0;
        }
    }

    [Serializable]
    public class BoundingBox
    {
        // Variables                                                                                                                
        public double MinX;
        public double MinY;
        public double MinZ;
        //
        public double MaxX;
        public double MaxY;
        public double MaxZ;
        //
        public object Tag;


        // Constructors                                                                                                             
        public BoundingBox()
        {
            Reset();
        }
        public BoundingBox(BoundingBox box)
        {
            MinX = box.MinX;
            MinY = box.MinY;
            MinZ = box.MinZ;
            //
            MaxX = box.MaxX;
            MaxY = box.MaxY;
            MaxZ = box.MaxZ;
            //
            if (box.Tag != null) Tag = box.Tag.DeepClone();
        }


        // Static methods                                                                                                           
        public static double[] GetCenter(IEnumerable<BoundingBox> boxes)
        {
            BoundingBox bb = new BoundingBox();
            foreach (var box in boxes) bb.IncludeBox(box);
            return bb.GetCenter();
        }


        // Methods                                                                                                                  
        public void Reset()
        {
            MinX = double.MaxValue;
            MinY = double.MaxValue;
            MinZ = double.MaxValue;
            MaxX = -double.MaxValue;
            MaxY = -double.MaxValue;
            MaxZ = -double.MaxValue;
        }
        public void AddOffset(double[] offset)
        {
            MinX += offset[0];
            MaxX += offset[0];
            MinY += offset[1];
            MaxY += offset[1];
            MinZ += offset[2];
            MaxZ += offset[2];
        }
        public void RemoveOffset(double[] offset)
        {
            MinX -= offset[0];
            MaxX -= offset[0];
            MinY -= offset[1];
            MaxY -= offset[1];
            MinZ -= offset[2];
            MaxZ -= offset[2];
        }
        //
        public void IncludeCoor(double[] coor)
        {
            if (coor[0] > MaxX) MaxX = coor[0];
            if (coor[0] < MinX) MinX = coor[0];
            //
            if (coor[1] > MaxY) MaxY = coor[1];
            if (coor[1] < MinY) MinY = coor[1];
            //
            if (coor[2] > MaxZ) MaxZ = coor[2];
            if (coor[2] < MinZ) MinZ = coor[2];
        }
        public void IncludeCoors(double[][] coors)
        {
            for (int i = 0; i < coors.Length; i++)
            {
                IncludeCoor(coors[i]);
            }
        }
        public void IncludeNode(FeNode node)
        {
            if (node.X > MaxX) MaxX = node.X;
            if (node.X < MinX) MinX = node.X;
            //
            if (node.Y > MaxY) MaxY = node.Y;
            if (node.Y < MinY) MinY = node.Y;
            //
            if (node.Z > MaxZ) MaxZ = node.Z;
            if (node.Z < MinZ) MinZ = node.Z;
        }
        public void IncludeBox(BoundingBox box)
        {
            if (box.MaxX > MaxX) MaxX = box.MaxX;
            if (box.MinX < MinX) MinX = box.MinX;
            //
            if (box.MaxY > MaxY ) MaxY = box.MaxY;
            if (box.MinY < MinY) MinY = box.MinY;
            //
            if (box.MaxZ > MaxZ) MaxZ = box.MaxZ;
            if (box.MinZ < MinZ) MinZ = box.MinZ;
        }
        public void Scale(double scaleFactor)
        {
            double delta = 0.5 * (MaxX - MinX) * (scaleFactor - 1);
            MinX -= delta;
            MaxX += delta;
            //
            delta = 0.5 * (MaxY - MinY) * (scaleFactor - 1);
            MinY -= delta;
            MaxY += delta;
            //
            delta = 0.5 * (MaxZ - MinZ) * (scaleFactor - 1);
            MinZ -= delta;
            MaxZ += delta;
        }
        //
        public bool Intesects(BoundingBox box)
        {
            if (box.MaxX < MinX || box.MinX > MaxX) return false;
            else if (box.MaxY < MinY || box.MinY > MaxY) return false;
            else if (box.MaxZ < MinZ || box.MinZ > MaxZ) return false;
            else return true;
        }
        public bool Intesects(List<BoundingBox> boxes)
        {
            foreach (var box in boxes)
            {
                if (Intesects(box)) return true;
            }
            return false;
        }
        //
        public double[] GetCenter()
        {
            return new double[] { (MinX + MaxX) / 2, (MinY + MaxY) / 2, (MinZ + MaxZ) / 2 };
        }
        public double GetDiagonal()
        {
            return Math.Sqrt(Math.Pow(MaxX - MinX, 2) + Math.Pow(MaxY - MinY, 2) + Math.Pow(MaxZ - MinZ, 2));
        }
        public double GetVolume()
        {
            return (MaxX - MinX) * (MaxY - MinY) * (MaxZ - MinZ);
        }
        //
        public bool IsEqual(BoundingBox boundingBox)
        {
            double diagonal = Math.Pow(MinX - MaxX, 2) + Math.Pow(MinY - MaxY, 2) + Math.Pow(MinZ - MaxZ, 2);
            double bbDiagonal = Math.Pow(boundingBox.MinX - boundingBox.MaxX, 2) + Math.Pow(boundingBox.MinY - boundingBox.MaxY, 2) + Math.Pow(boundingBox.MinZ - boundingBox.MaxZ, 2);

            if (diagonal == 0 && bbDiagonal == 0) return true;
            else if (diagonal != 0 && bbDiagonal != 0) return Math.Abs(diagonal - bbDiagonal) / Math.Max(diagonal, bbDiagonal) < 0.001 ? true : false;
            return false;
        }
        //
        public BoundingBox DeepCopy()
        {
            return new BoundingBox(this);
        }
    }
}
