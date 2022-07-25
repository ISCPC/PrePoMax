using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeResults
{
    public enum InterpolatorEnum
    {
        [StandardValue("ClosestNode", DisplayName = "Closest node")]
        ClosestNode,
        [StandardValue("ClosestPoint", DisplayName = "Closest point")]
        ClosestPoint
    }
    static public class ResultsInterpolators
    {
        public static void InterpolateScalarResults(PartExchangeData source, PartExchangeData target, InterpolatorEnum interpolator,
                                                    out float[] distances, out float[] values)
        {
            BoundingBox sourceBox = ComputeAllNodesBoundingBox(source);
            double l = sourceBox.GetDiagonal() / 128;
            //
            int nx = (int)Math.Ceiling(sourceBox.GetXSize() / l);
            int ny = (int)Math.Ceiling(sourceBox.GetYSize() / l);
            int nz = (int)Math.Ceiling(sourceBox.GetZSize() / l);
            int nxy = nx * ny;
            double deltaX = sourceBox.GetXSize() / nx;
            double deltaY = sourceBox.GetYSize() / ny;
            double deltaZ = sourceBox.GetZSize() / nz;
            //
            BoundingBox[] cellBoxes = ComputeCellBoundingBoxes(source);
            BoundingBox[] regionBoxes = SplitCellBoxesToRegions(cellBoxes, sourceBox, nx, ny, nz);
            Triangle[] triangles = TriangularCellsToTriangles(source);
            //
            int i;
            int j;
            int k;
            int mini;
            int maxi;
            int minj;
            int maxj;
            int mink;
            int maxk;
            double[] sourceCoor;
            BoundingBox bb;
            int num;
            int delta;
            double d;
            double boxD;
            double minD;
            Vec3D sourcePoint;
            Vec3D closestPoint;
            Vec3D bestPoint = new Vec3D();
            Triangle triangle;
            Triangle bestTriangle = null;
            bool closer;
            distances = new float[target.Nodes.Coor.Length];
            values = new float[target.Nodes.Coor.Length];
            //
            for (int nId = 0; nId < target.Nodes.Coor.Length; nId++)
            {
                sourceCoor = target.Nodes.Coor[nId];
                sourcePoint = new Vec3D(sourceCoor);
                i = (int)Math.Floor((sourceCoor[0] - sourceBox.MinX) / deltaX);
                j = (int)Math.Floor((sourceCoor[1] - sourceBox.MinY) / deltaY);
                k = (int)Math.Floor((sourceCoor[2] - sourceBox.MinZ) / deltaZ);
                if (i < 0) i = 0;
                else if (i >= nx) i = nx - 1;
                if (j < 0) j = 0;
                else if (j >= ny) j = ny - 1;
                if (k < 0) k = 0;
                else if (k >= nz) k = nz - 1;
                bb = regionBoxes[k * nxy + j * nx + i];
                //
                mini = i;
                maxi = i;
                minj = j;
                maxj = j;
                mink = k;
                maxk = k;
                delta = 0;
                num = ((Dictionary<int, BoundingBox>)bb.Tag).Count;
                // Add next layer of regions
                while (num == 0 || delta < 1)
                {
                    delta++;
                    mini = i - delta;
                    maxi = i + delta;
                    minj = j - delta;
                    maxj = j + delta;
                    mink = k - delta;
                    maxk = k + delta;
                    if (mini < 0) mini = 0;
                    if (maxi >= nx) maxi = nx - 1;
                    if (minj < 0) minj = 0;
                    if (maxj >= ny) maxj = ny - 1;
                    if (mink < 0) mink = 0;
                    if (maxk >= nz) maxk = nz - 1;
                    for (int kk = mink; kk <= maxk; kk++)
                    {
                        for (int jj = minj; jj <= maxj; jj++)
                        {
                            for (int ii = mini; ii <= maxi; ii++)
                            {
                                bb = regionBoxes[kk * nxy + jj * nx + ii];
                                num += ((Dictionary<int, BoundingBox>)bb.Tag).Count;
                            }
                        }
                    }
                }
                //
                minD = double.MaxValue;
                //
                for (int kk = mink; kk <= maxk; kk++)
                {
                    for (int jj = minj; jj <= maxj; jj++)
                    {
                        for (int ii = mini; ii <= maxi; ii++)
                        {
                            bb = regionBoxes[kk * nxy + jj * nx + ii];
                            if (((Dictionary<int, BoundingBox>)bb.Tag).Count == 0) continue;
                            //
                            foreach (var entry in (Dictionary<int, BoundingBox>)bb.Tag)
                            {
                                triangle = triangles[entry.Key];
                                boxD = entry.Value.MaxOutsideDistance2(sourceCoor);
                                if (boxD < minD)
                                {
                                    if (interpolator == InterpolatorEnum.ClosestNode)
                                        closer = triangle.GetClosestNodeTo(sourcePoint, minD, out closestPoint);
                                    else if (interpolator == InterpolatorEnum.ClosestPoint)
                                        closer = triangle.GetClosestPointTo(sourcePoint, minD, out closestPoint);
                                    else throw new NotSupportedException();
                                    //
                                    if (closer)
                                    {
                                        d = (closestPoint - sourcePoint).Len2;
                                        //
                                        if (d < minD)
                                        {
                                            minD = d;
                                            bestTriangle = triangle;
                                            bestPoint.X = closestPoint.X;
                                            bestPoint.Y = closestPoint.Y;
                                            bestPoint.Z = closestPoint.Z;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //
                distances[nId] = (float)Math.Sqrt(minD);
                values[nId] = (float)bestTriangle.InterpolateAt(bestPoint);
            }
        }
        //
        private static BoundingBox ComputeAllNodesBoundingBox(PartExchangeData pData)
        {
            BoundingBox bb = new BoundingBox();
            bb.IncludeFirstCoor(pData.Nodes.Coor[0]);
            for (int i = 0; i < pData.Nodes.Coor.Length; i++) bb.IncludeCoorFast(pData.Nodes.Coor[i]);
            return bb;
        }
        private static BoundingBox[] ComputeCellBoundingBoxes(PartExchangeData pData)
        {
            int[] cell;
            BoundingBox bb;
            BoundingBox[] bBoxes = new BoundingBox[pData.Cells.Ids.Length];
            //
            for (int i = 0; i < pData.Cells.CellNodeIds.Length; i++)
            {
                cell = pData.Cells.CellNodeIds[i];
                bb = new BoundingBox();
                bb.IncludeFirstCoor(pData.Nodes.Coor[cell[0]]);
                bb.IncludeCoorFast(pData.Nodes.Coor[cell[1]]);
                bb.IncludeCoorFast(pData.Nodes.Coor[cell[2]]);
                if (cell.Length == 4 || cell.Length == 8) bb.IncludeCoorFast(pData.Nodes.Coor[cell[3]]);
                //
                bBoxes[i] = bb;
            }
            //
            return bBoxes;
        }
        private static BoundingBox[] SplitCellBoxesToRegions(BoundingBox[] cellBoxes, BoundingBox cellBoxesBox, 
                                                             int nx, int ny, int nz)
        {
            int nxy = nx * ny;
            double deltaX = cellBoxesBox.GetXSize() / nx;
            double deltaY = cellBoxesBox.GetYSize() / ny;
            double deltaZ = cellBoxesBox.GetZSize() / nz;
            //
            BoundingBox bb;
            BoundingBox[] regions = new BoundingBox[nxy * nz];
            for (int k = 0; k < nz; k++)
            {
                for (int j = 0; j < ny; j++)
                {
                    for (int i = 0; i < nx; i++)
                    {
                        bb = new BoundingBox();
                        bb.MinX = cellBoxesBox.MinX + i * deltaX;
                        bb.MaxX = bb.MinX + deltaX;
                        bb.MinY = cellBoxesBox.MinY + j * deltaY;
                        bb.MaxY = bb.MinY + deltaY;
                        bb.MinZ = cellBoxesBox.MinZ + k * deltaZ;
                        bb.MaxZ = bb.MinZ + deltaZ;
                        bb.Tag = new Dictionary<int, BoundingBox>();
                        regions[k * nxy + j * nx + i] = bb;
                    }
                }
            }
            //
            int count = 0;
            int mini;
            int maxi;
            int minj;
            int maxj;
            int mink;
            int maxk;
            // If cell box max value is on the border of the region division, the cell will be a member of both space regions
            foreach (var cellBox in cellBoxes)
            {
                mini = (int)Math.Floor((cellBox.MinX - cellBoxesBox.MinX) / deltaX);
                maxi = (int)Math.Floor((cellBox.MaxX - cellBoxesBox.MinX) / deltaX);
                if (maxi == nx) maxi--;
                //
                minj = (int)Math.Floor((cellBox.MinY - cellBoxesBox.MinY) / deltaY);
                maxj = (int)Math.Floor((cellBox.MaxY - cellBoxesBox.MinY) / deltaY);
                if (maxj == ny) maxj--;
                //
                mink = (int)Math.Floor((cellBox.MinZ - cellBoxesBox.MinZ) / deltaZ);
                maxk = (int)Math.Floor((cellBox.MaxZ - cellBoxesBox.MinZ) / deltaZ);
                if (maxk == nz) maxk--;
                //
                for (int k = mink; k <= maxk; k++)
                {
                    for (int j = minj; j <= maxj; j++)
                    {
                        for (int i = mini; i <= maxi; i++)
                        {
                            bb = regions[k * nxy + j * nx + i];
                            ((Dictionary<int, BoundingBox>)bb.Tag).Add(count, cellBox);
                        }
                    }
                }
                //
                count++;
            }
            //
            return regions;
        }
        private static Triangle[] TriangularCellsToTriangles(PartExchangeData pData)
        {
            int[] cell;
            Triangle[] triangles = new Triangle[pData.Cells.CellNodeIds.Length];
            //
            for (int i = 0; i < triangles.Length; i++)
            {
                cell = pData.Cells.CellNodeIds[i];
                if (cell.Length != 3) throw new NotSupportedException();
                triangles[i] = new Triangle(pData.Nodes.Coor[cell[0]],
                                            pData.Nodes.Coor[cell[1]],
                                            pData.Nodes.Coor[cell[2]],
                                            pData.Nodes.Values[cell[0]],
                                            pData.Nodes.Values[cell[1]],
                                            pData.Nodes.Values[cell[2]]);
            }
            //
            return triangles;
        }
        private static Triangle[] TriangularCellsToTrianglesParallel(PartExchangeData pData)
        {
            // This is slower
            Triangle[] triangles = new Triangle[pData.Cells.CellNodeIds.Length];
            //
            Parallel.For(0, triangles.Length, i =>
            {
                int[] cell = pData.Cells.CellNodeIds[i];
                if (cell.Length != 3) throw new NotSupportedException();
                triangles[i] = new Triangle(pData.Nodes.Coor[cell[0]],
                                            pData.Nodes.Coor[cell[1]],
                                            pData.Nodes.Coor[cell[2]],
                                            pData.Nodes.Values[cell[0]],
                                            pData.Nodes.Values[cell[1]],
                                            pData.Nodes.Values[cell[2]]);
            });
            //
            return triangles;
        }
        //
        public static void ClosestPointToShouldWork()
        {
            var r = new Random(0);
            double next() => r.NextDouble() * 5 - 1;
            var t = new Triangle(new Vec3D(0, 0, 0), new Vec3D(3.5, 2, 0), new Vec3D(3, 0.0, 0), 0, 0, 0);
            //
            var hash = new Vec3D(0, 0, 0);
            for (int i = 0; i < 800; i++)
            {
                var pt = new Vec3D(next(), next(), 0);
                var pc = t.ClosestPointTo(pt);
                hash += pc;
            }
            // Test the hash
            // If it doesn't match then eyeball the visualization
            // and see what has gone wrong

            //hash.ShouldBeApproximately(new Vector3(1496.28118561104, 618.196568578824, 0), 1e-5);

        }
    }
}
