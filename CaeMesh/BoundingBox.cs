using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class BoundingBox
    {
        public double MinX;
        public double MinY;
        public double MinZ;

        public double MaxX;
        public double MaxY;
        public double MaxZ;

        public BoundingBox()
        {
            Reset();
        }
        public BoundingBox(BoundingBox box)
        {
            MinX = box.MinX;
            MinY = box.MinY;
            MinZ = box.MinZ;

            MaxX = box.MaxX;
            MaxY = box.MaxY;
            MaxZ = box.MaxZ;
        }

        public void Reset()
        {
            MinX = double.MaxValue;
            MinY = double.MaxValue;
            MinZ = double.MaxValue;
            MaxX = -double.MaxValue;
            MaxY = -double.MaxValue;
            MaxZ = -double.MaxValue;
        }
        public void SetMin(FeNode node)
        {
            MinX = node.X;
            MinY = node.Y;
            MinZ = node.Z;
        }
        public void SetMax(FeNode node)
        {
            MaxX = node.X;
            MaxY = node.Y;
            MaxZ = node.Z;
        }
        public void CheckNode(FeNode node)
        {
            if (node.X > MaxX) MaxX = node.X;
            if (node.X < MinX) MinX = node.X;

            if (node.Y > MaxY) MaxY = node.Y;
            if (node.Y < MinY) MinY = node.Y;

            if (node.Z > MaxZ) MaxZ = node.Z;
            if (node.Z < MinZ) MinZ = node.Z;
        }
        public void CheckBox(BoundingBox box)
        {
            if (box.MaxX > MaxX) MaxX = box.MaxX;
            if (box.MinX < MinX) MinX = box.MinX;

            if (box.MaxY > MaxY ) MaxY = box.MaxY;
            if (box.MinY < MinY) MinY = box.MinY;

            if (box.MaxZ > MaxZ) MaxZ = box.MaxZ;
            if (box.MinZ < MinZ) MinZ = box.MinZ;
        }
        public double GetDiagonal()
        {
            return Math.Sqrt(Math.Pow(MaxX - MinX, 2) + Math.Pow(MaxY - MinY, 2) + Math.Pow(MaxZ - MinZ, 2));
        }

        public bool IsEqual(BoundingBox boundingBox)
        {
            double diagonal = Math.Pow(MinX - MaxX, 2) + Math.Pow(MinY - MaxY, 2) + Math.Pow(MinZ - MaxZ, 2);
            double bbDiagonal = Math.Pow(boundingBox.MinX - boundingBox.MaxX, 2) + Math.Pow(boundingBox.MinY - boundingBox.MaxY, 2) + Math.Pow(boundingBox.MinZ - boundingBox.MaxZ, 2);

            if (diagonal == 0 && bbDiagonal == 0) return true;
            else if (diagonal != 0 && bbDiagonal != 0) return Math.Abs(diagonal - bbDiagonal) / Math.Max(diagonal, bbDiagonal) < 0.001 ? true : false;
            return false;
        }

        public BoundingBox DeepCopy()
        {
            return new BoundingBox(this);
        }
    }
}
