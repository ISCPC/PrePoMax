using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeResults
{
    static public class ResultsInterpolators
    {
        public static void InterpolateScalarResults(PartExchangeData source, PartExchangeData target)
        {
            BoundingBox sourceBox = ComputeAllNodesBoundingBox(source);
            double l = sourceBox.GetDiagonal() / 64;
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
            double[] coor;
            BoundingBox bb;
            int num;
            int delta;
            double min;
            double value;
            double dist;
            double[] center;
            int[] cell;
            target.Nodes.Values = new float[target.Nodes.Coor.Length];
            //
            for (int nId = 0; nId < target.Nodes.Coor.Length; nId++)
            { 
                coor = target.Nodes.Coor[nId];
                i = (int)Math.Floor((coor[0] - sourceBox.MinX) / deltaX);
                j = (int)Math.Floor((coor[1] - sourceBox.MinY) / deltaY);
                k = (int)Math.Floor((coor[2] - sourceBox.MinZ) / deltaZ);
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
                while (num == 0)
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
                    if (maxj >= nx) maxj = ny - 1;
                    if (mink < 0) mink = 0;
                    if (maxk >= nx) maxk = nz - 1;
                    for (int kk = mink; kk < maxk; kk++)
                    {
                        for (int jj = minj; jj < maxj; jj++)
                        {
                            for (int ii = mini; ii < maxi; ii++)
                            {
                                bb = regionBoxes[kk * nxy + jj * nx + ii];
                                num += ((Dictionary<int, BoundingBox>)bb.Tag).Count;
                            }
                        }
                    }
                }
                //
                min = double.MaxValue;
                value = -1;
                for (int kk = mink; kk <= maxk; kk++)
                {
                    for (int jj = minj; jj <= maxj; jj++)
                    {
                        for (int ii = mini; ii <= maxi; ii++)
                        {
                            bb = regionBoxes[kk * nxy + jj * nx + ii];
                            
                            if (((Dictionary<int, BoundingBox>)bb.Tag).Count == 0) continue;

                            foreach (var entry in (Dictionary<int, BoundingBox>)bb.Tag)
                            {
                                cell = source.Cells.CellNodeIds[entry.Key];
                                foreach (var nodeId in cell)
                                {
                                    center = source.Nodes.Coor[nodeId];

                                    //center = bb.GetCenter();

                                    dist = Math.Sqrt(Math.Pow(coor[0] - center[0], 2) +
                                                     Math.Pow(coor[1] - center[1], 2) +
                                                     Math.Pow(coor[2] - center[2], 2));

                                    if (dist < min)
                                    {
                                        min = dist;
                                        value = source.Nodes.Values[nodeId];
                                    }
                                }
                            }

                            
                        }
                    }
                }

                target.Nodes.Values[nId] = (float)value;
            }
        }



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
    }
}
