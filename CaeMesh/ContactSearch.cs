using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public enum GroupContactPairsByEnum
    {
        None,
        BySurfaceAngle,
        ByParts
    }
    public class ContactSurface
    {
        // Variables                                                                                                                
        private FeMesh _mesh;
        private BasePart _part;
        private int _id;
        private HashSet<int> _nodeIds;
        private BoundingBox _boundingBox;


        // Properties                                                                                                               
        public FeMesh Mesh { get { return _mesh; } set { _mesh = value; } }
        public BasePart Part { get { return _part; } set { _part = value; } }
        public int Id { get { return _id; } set { _id = value; } }
        public HashSet<int> NodeIds { get { return _nodeIds; } set { _nodeIds = value; } }
        public BoundingBox BoundingBox { get { return _boundingBox; } set { _boundingBox = value; } }


        // Constructors                                                                                                             
        public ContactSurface(FeMesh mesh, double[][] _nodes, BasePart part, int id, double boundingBoxOffset)
        {
            _mesh = mesh;
            _part = part;
            _id = id;
            //
            _boundingBox = new BoundingBox();
            _nodeIds = part.Visualization.GetNodeIdsBySurface(_id);
            //foreach (var nodeId in _nodeIds) _boundingBox.IncludeNode(_mesh.Nodes[nodeId]);
            _boundingBox.IncludeFirstCoor(_nodes[_nodeIds.First()]);
            foreach (var nodeId in _nodeIds) _boundingBox.IncludeCoorFast(_nodes[nodeId]);
            _boundingBox.Inflate(boundingBoxOffset);
        }


        // Methods                                                                                                                  
        public int GetGeometryId()
        {
            return _id * 100000 + 3 * 10000 + _part.PartId;
        }
    }
    public class MasterSlaveItem
    {
        // Variables                                                                                                                
        private string _name;
        private List<int> _masterGeometryIds;
        private List<int> _slaveGeometryIds;


        // Properties                                                                                                               
        public string Name { get { return _name; } set { _name = value; } }
        public List<int> MasterGeometryIds { get { return _masterGeometryIds; } set { _masterGeometryIds = value; } }
        public List<int> SlaveGeometryIds { get { return _slaveGeometryIds; } set { _slaveGeometryIds = value; } }


        // Constructors                                                                                                             
        public MasterSlaveItem(string name, List<int> masterGeometryIds, List<int> slaveGeometryIds)
        {
            _name = name;
            _masterGeometryIds = masterGeometryIds;
            _slaveGeometryIds = slaveGeometryIds;
        }
    }
    //
    public class ContactSearch
    {
        // Variables                                                                                                                
        private FeMesh _mesh;
        private double[][] _nodes;
        private BoundingBox[][] _boundingBoxes;
        private bool _includeSelfContact;
        private GroupContactPairsByEnum _groupContactPairsBy;


        // Properties                                                                                                               
        public GroupContactPairsByEnum GroupContactPairsBy
        {
            get { return _groupContactPairsBy; }
            set { _groupContactPairsBy = value; }
        }


        // Constructors                                                                                                             
        public ContactSearch(FeMesh mesh) 
        {
            _mesh = mesh;
            //
            _nodes = new double[mesh.MaxNodeId + 1][];
            foreach (var nodeEntry in mesh.Nodes) _nodes[nodeEntry.Key] = nodeEntry.Value.Coor;            
            //
            _includeSelfContact = false;
            _groupContactPairsBy = GroupContactPairsByEnum.None;
        }


        // Methods                                                                                                                  
        public List<MasterSlaveItem> FindContactPairs(double distance, double angleDeg)
        {
            // Bounding boxes
            int[] cell;
            BoundingBox bb;
            _boundingBoxes = new BoundingBox[_mesh.GetMaxPartId() + 1][];
            foreach (var partEntry in _mesh.Parts)
            {
                _boundingBoxes[partEntry.Value.PartId] = new BoundingBox[partEntry.Value.Visualization.Cells.Length];
                for (int i = 0; i < partEntry.Value.Visualization.Cells.Length; i++)
                {
                    cell = partEntry.Value.Visualization.Cells[i];
                    bb = new BoundingBox();
                    bb.IncludeFirstCoor(_nodes[cell[0]]);
                    bb.IncludeCoorFast(_nodes[cell[1]]);
                    bb.IncludeCoorFast(_nodes[cell[2]]);
                    bb.Inflate(distance * 0.5);
                    //
                    _boundingBoxes[partEntry.Value.PartId][i] = bb;
                }
            }
            //
            int count = 0;
            foreach (var partEntry in _mesh.Parts) count += partEntry.Value.Visualization.FaceAreas.Length;
            ContactSurface[] contactSurfaces = new ContactSurface[count];
            //
            count = 0;
            foreach (var partEntry in _mesh.Parts)
            {
                for (int i = 0; i < partEntry.Value.Visualization.FaceAreas.Length; i++)
                {
                    contactSurfaces[count++] = new ContactSurface(_mesh, _nodes, partEntry.Value, i, distance * 0.5);
                }
            }
            //
            double angleRad = angleDeg * Math.PI / 180;
            List<ContactSurface[]> contactSurfacePairs = new List<ContactSurface[]>();
            //
            for (int i = 0; i < contactSurfaces.Length; i++)
            {
                for (int j = i + 1; j < contactSurfaces.Length; j++)
                {
                    if (CheckSurfaceToSurfaceContact(contactSurfaces[i], contactSurfaces[j], distance, angleRad))
                    {
                        contactSurfacePairs.Add(new ContactSurface[] { contactSurfaces[i], contactSurfaces[j] });
                    }
                }
            }
            //
            if (_groupContactPairsBy == GroupContactPairsByEnum.None) return GroupContactPairsByNone(contactSurfacePairs);
            else if (_groupContactPairsBy == GroupContactPairsByEnum.BySurfaceAngle) throw new NotSupportedException();
            else if (_groupContactPairsBy == GroupContactPairsByEnum.ByParts) return GroupContactPairsByParts(contactSurfacePairs);
            else throw new NotSupportedException();
        }
        private bool CheckSurfaceToSurfaceContact(ContactSurface cs1, ContactSurface cs2, double distance, double angleRad)
        {
            if (!_includeSelfContact && cs1.Part == cs2.Part) return false;
            if (!cs1.BoundingBox.Intersects(cs2.BoundingBox)) return false;
            //            
            double dist;
            double ang;
            int[] cell1;
            int[] cell2;
            double[] p = new double[3];
            double[] q = new double[3];
            double[] a = new double[3];
            double[] b = new double[3];
            double[] n1 = new double[3];
            double[] n2 = new double[3];
            double[][] t1 = new double[3][];
            double[][] t2 = new double[3][];
            BoundingBox bb1;
            BoundingBox bb2;
            BoundingBox[] bbs1 = _boundingBoxes[cs1.Part.PartId];
            BoundingBox[] bbs2 = _boundingBoxes[cs2.Part.PartId];
            BoundingBox intersection = cs1.BoundingBox.GetIntersection(cs2.BoundingBox);
            //
            foreach (int cell1Id in cs1.Part.Visualization.CellIdsByFace[cs1.Id])
            {
                //if (bbs1[cell1Id] == null)
                //{
                //    cell1 = cs1.Part.Visualization.Cells[cell1Id];
                //    bb1 = new BoundingBox();
                //    bb1.IncludeFirstCoor(_nodes[cell1[0]]);
                //    bb1.IncludeCoorFast(_nodes[cell1[1]]);
                //    bb1.IncludeCoorFast(_nodes[cell1[2]]);
                //    bb1.Inflate(distance * 0.5);
                //    bbs1[cell1Id] = bb1;
                //}
                //else
                bb1 = bbs1[cell1Id];
                //
                if (bb1.MaxX < intersection.MinX) continue;
                else if (bb1.MinX > intersection.MaxX) continue;
                else if (bb1.MaxY < intersection.MinY) continue;
                else if (bb1.MinY > intersection.MaxY) continue;
                else if (bb1.MaxZ < intersection.MinZ) continue;
                else if (bb1.MinZ > intersection.MaxZ) continue;
                else
                //if (bb1.Intersects(intersection))
                {
                    foreach (int cell2Id in cs2.Part.Visualization.CellIdsByFace[cs2.Id])
                    {
                        //if (bbs2[cell2Id] == null)
                        //{
                        //    cell2 = cs2.Part.Visualization.Cells[cell2Id];
                        //    bb2 = new BoundingBox();
                        //    bb2.IncludeFirstCoor(_nodes[cell2[0]]);
                        //    bb2.IncludeCoorFast(_nodes[cell2[1]]);
                        //    bb2.IncludeCoorFast(_nodes[cell2[2]]);
                        //    bb2.Inflate(distance * 0.5);
                        //    bbs2[cell2Id] = bb2;
                        //}
                        //else
                        bb2 = bbs2[cell2Id];
                        //
                        if (bb1.MaxX < bb2.MinX) continue;
                        else if (bb1.MinX > bb2.MaxX) continue;
                        else if (bb1.MaxY < bb2.MinY) continue;
                        else if (bb1.MinY > bb2.MaxY) continue;
                        else if (bb1.MaxZ < bb2.MinZ) continue;
                        else if (bb1.MinZ > bb2.MaxZ) continue;
                        else
                        //if (bb1.Intersects(bb2))
                        {
                            cell1 = cs1.Part.Visualization.Cells[cell1Id];
                            t1[0] = _nodes[cell1[0]];
                            t1[1] = _nodes[cell1[1]];
                            t1[2] = _nodes[cell1[2]];
                            //
                            Geometry.VmV(ref a, t1[1], t1[0]);
                            Geometry.VmV(ref b, t1[2], t1[0]);
                            Geometry.VcrossV(ref n1, a, b);
                            Geometry.VxS(ref n1, n1, 1 / Math.Sqrt(Geometry.VdotV(n1, n1)));
                            //
                            cell2 = cs2.Part.Visualization.Cells[cell2Id];
                            t2[0] = _nodes[cell2[0]];
                            t2[1] = _nodes[cell2[1]];
                            t2[2] = _nodes[cell2[2]];
                            //
                            Geometry.VmV(ref a, t2[1], t2[0]);
                            Geometry.VmV(ref b, t2[2], t2[0]);
                            Geometry.VcrossV(ref n2, a, b);
                            Geometry.VxS(ref n2, n2, 1 / Math.Sqrt(Geometry.VdotV(n2, n2)));
                            //
                            ang = Math.Acos(Geometry.VdotV(n1, n2));
                            //
                            if (ang > angleRad)
                            {
                                dist = Geometry.TriDist(ref p, ref q, t1, t2);
                                //
                                if (dist < distance) return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool CheckSurfaceToSurfaceContact1(ContactSurface cs1, ContactSurface cs2, double distance, double angleRad)
        {
            if (!_includeSelfContact && cs1.Part == cs2.Part) return false;
            if (!cs1.BoundingBox.Intersects(cs2.BoundingBox)) return false;
            //
            BoundingBox intersection = cs1.BoundingBox.GetIntersection(cs2.BoundingBox);
            //
            double dist;
            double ang;
            int[] cell1;
            int[] cell2;
            double[] p = new double[3];
            double[] q = new double[3];
            double[] a = new double[3];
            double[] b = new double[3];
            double[] n1 = new double[3];
            double[] n2 = new double[3];
            double[][] t1 = new double[3][];
            double[][] t2 = new double[3][];
            BoundingBox bb1 = new BoundingBox();
            BoundingBox bb2 = new BoundingBox();
            //
            foreach (int cell1Id in cs1.Part.Visualization.CellIdsByFace[cs1.Id])
            {
                cell1 = cs1.Part.Visualization.Cells[cell1Id];
                //t1[0] = cs1.Mesh.Nodes[cell1[0]].Coor;
                //t1[1] = cs1.Mesh.Nodes[cell1[1]].Coor;
                //t1[2] = cs1.Mesh.Nodes[cell1[2]].Coor;
                t1[0] = _nodes[cell1[0]];
                t1[1] = _nodes[cell1[1]];
                t1[2] = _nodes[cell1[2]];
                //
                Geometry.VmV(ref a, t1[1], t1[0]);
                Geometry.VmV(ref b, t1[2], t1[0]);
                Geometry.VcrossV(ref n1, a, b);
                Geometry.VxS(ref n1, n1, 1 / Math.Sqrt(Geometry.VdotV(n1, n1)));
                //
                //bb1.Reset();
                //bb1.IncludeFirstCoor(t1[0]);
                //bb1.IncludeCoorFast(t1[1]);
                //bb1.IncludeCoorFast(t1[2]);
                //bb1.Inflate(distance * 0.5);
                bb1 = _boundingBoxes[cs1.Part.PartId][cell1Id];
                //
                if (bb1.Intersects(intersection))
                {
                    foreach (int cell2Id in cs2.Part.Visualization.CellIdsByFace[cs2.Id])
                    {
                        cell2 = cs2.Part.Visualization.Cells[cell2Id];
                        //t2[0] = cs2.Mesh.Nodes[cell2[0]].Coor;
                        //t2[1] = cs2.Mesh.Nodes[cell2[1]].Coor;
                        //t2[2] = cs2.Mesh.Nodes[cell2[2]].Coor;
                        t2[0] = _nodes[cell2[0]];
                        t2[1] = _nodes[cell2[1]];
                        t2[2] = _nodes[cell2[2]];
                        //
                        //bb2.Reset();
                        //bb2.IncludeFirstCoor(t2[0]);
                        //bb2.IncludeCoorFast(t2[1]);
                        //bb2.IncludeCoorFast(t2[2]);
                        //bb2.Inflate(distance * 0.5);
                        bb2 = _boundingBoxes[cs2.Part.PartId][cell2Id];
                        //
                        if (bb1.Intersects(bb2))
                        {
                            Geometry.VmV(ref a, t2[1], t2[0]);
                            Geometry.VmV(ref b, t2[2], t2[0]);
                            Geometry.VcrossV(ref n2, a, b);
                            Geometry.VxS(ref n2, n2, 1 / Math.Sqrt(Geometry.VdotV(n2, n2)));
                            //
                            ang = Math.Acos(Geometry.VdotV(n1, n2));
                            //
                            if (ang > angleRad)
                            {
                                dist = Geometry.TriDist(ref p, ref q, t1, t2);
                                //
                                if (dist < distance) return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        //
        private List<MasterSlaveItem> GroupContactPairsByNone(List<ContactSurface[]> contactSurfacePairs)
        {
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            //
            foreach (var csp in contactSurfacePairs)
            {
                masterSlaveItems.Add(new MasterSlaveItem(csp[0].Part.Name + "_to_" + csp[1].Part.Name,
                                                         new List<int>() { csp[0].GetGeometryId() },
                                                         new List<int>() { csp[1].GetGeometryId() }));
            }
            //
            return masterSlaveItems;
        }
        private List<MasterSlaveItem> GroupContactPairsByParts(List<ContactSurface[]> contactSurfacePairs)
        {
            int i;
            int j;
            int[] key;
            MasterSlaveItem masterSlaveItem;
            CompareIntArray comparer = new CompareIntArray();
            Dictionary<int[], MasterSlaveItem> masterSlaveItems = new Dictionary<int[], MasterSlaveItem>(comparer);
            //
            foreach (var csp in contactSurfacePairs)
            {
                if (csp[0].Part.PartId < csp[1].Part.PartId)
                {
                    i = 0;
                    j = 1;
                }
                else
                {
                    i = 1;
                    j = 0;
                }
                //
                key = new int[] { csp[i].Part.PartId, csp[j].Part.PartId };
                if (masterSlaveItems.TryGetValue(key, out masterSlaveItem))
                {
                    masterSlaveItem.MasterGeometryIds.Add(csp[i].GetGeometryId());
                    masterSlaveItem.SlaveGeometryIds.Add(csp[j].GetGeometryId());
                }
                else
                {
                    masterSlaveItem = new MasterSlaveItem(csp[0].Part.Name + "_to_" + csp[1].Part.Name,
                                                          new List<int>(), new List<int>());
                    masterSlaveItem.MasterGeometryIds.Add(csp[i].GetGeometryId());
                    masterSlaveItem.SlaveGeometryIds.Add(csp[j].GetGeometryId());
                    masterSlaveItems.Add(key, masterSlaveItem);
                }
            }
            //
            return masterSlaveItems.Values.ToList();
        }
    }
}
