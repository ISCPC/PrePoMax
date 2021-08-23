﻿using System;
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
        private string _masterName;
        private string _slaveName;
        private int _masterPartId;
        private int _slavePartId;
        private HashSet<int> _masterGeometryIds;
        private HashSet<int> _slaveGeometryIds;


        // Properties                                                                                                               
        public string Name { get { return _masterName + "_to_" + _slaveName; } }
        public string MasterName { get { return _masterName; } }
        public string SlaveName { get { return _slaveName; } }
        public int MasterPartId { get { return _masterPartId; } }
        public int SlavePartId { get { return _slavePartId; } }
        public HashSet<int> MasterGeometryIds { get { return _masterGeometryIds; } set { _masterGeometryIds = value; } }
        public HashSet<int> SlaveGeometryIds { get { return _slaveGeometryIds; } set { _slaveGeometryIds = value; } }


        // Constructors                                                                                                             
        public MasterSlaveItem(string masterName, string slaveName, int masterPartId, int slavePartId,
                               HashSet<int> masterGeometryIds, HashSet<int> slaveGeometryIds)
        {
            _masterName = masterName;
            _slaveName = slaveName;
            _masterPartId = masterPartId;
            _slavePartId = slavePartId;
            _masterGeometryIds = masterGeometryIds;
            _slaveGeometryIds = slaveGeometryIds;
        }
        public void SwitchMasterSlave()
        {
            string tmpName = _masterName;
            _masterName = _slaveName;
            _slaveName = tmpName;
            //
            int tmpId = _masterPartId;
            _masterPartId = _slavePartId;
            _slavePartId = tmpId;
            //
            HashSet<int> tmpIds = _masterGeometryIds;
            _masterGeometryIds = _slaveGeometryIds;
            _slaveGeometryIds = tmpIds;
            //

        }
    }
    public class PartRegion
    {
        // Variables                                                                                                                
        private string _partName;
        private int _partId;
        private HashSet<int> _surfaceIds;
        private HashSet<MasterSlaveItem> _masterSlaveItems;
        private HashSet<MasterSlaveItem> _independentMasterSlaveItems;


        // Properties                                                                                                               
        public string PartName { get { return _partName; } }
        public int PartId { get { return _partId; } }
        public HashSet<int> SurfaceIds { get { return _surfaceIds; } }
        public HashSet<MasterSlaveItem> MasterSlaveItems { get { return _masterSlaveItems; } }


        // Constructors                                                                                                             
        public PartRegion(string partName, int partId, MasterSlaveItem masterSlaveItem)
        {
            _partName = partName;
            _partId = partId;
            //
            if (partId == masterSlaveItem.MasterPartId) _surfaceIds = new HashSet<int>(masterSlaveItem.MasterGeometryIds);
            else _surfaceIds = new HashSet<int>(masterSlaveItem.SlaveGeometryIds);
            //
            _masterSlaveItems = new HashSet<MasterSlaveItem>() { masterSlaveItem };
            _independentMasterSlaveItems = new HashSet<MasterSlaveItem>();
        }


        // Methods                                                                                                                  
        public bool DoesMerge(PartRegion partRegion)
        {
            if (_partId == partRegion.PartId && _surfaceIds.Intersect(partRegion.SurfaceIds).Count() > 0) return true;
            else return false;
        }
        public bool Merge(PartRegion partRegion)
        {
            if (_partId == partRegion.PartId && _surfaceIds.Intersect(partRegion.SurfaceIds).Count() > 0)
            {
                _surfaceIds.UnionWith(partRegion.SurfaceIds);
                _masterSlaveItems.UnionWith(partRegion.MasterSlaveItems);
                return true;
            }
            else return false;
        }
        public void MakeMasterSlaveItemsIndependent(HashSet<MasterSlaveItem> independentMasterSleveItems)
        {
            foreach (var independentMasterSleveItem in independentMasterSleveItems)
            {
                if (_masterSlaveItems.Contains(independentMasterSleveItem))
                    _independentMasterSlaveItems.Add(independentMasterSleveItem);
            }
            // Remove independent master slave items
            foreach (var independentMasterSleveItem in _independentMasterSlaveItems)
            {
                _masterSlaveItems.Remove(independentMasterSleveItem);
            }
        }
    }
    public class PartRegionCollection
    {
        // Variables                                                                                                                
        private HashSet<PartRegion> _partRegions;


        // Constructors                                                                                                             
        public PartRegionCollection()
        {
            _partRegions = new HashSet<PartRegion>();
        }


        // Methods                                                                                                                  
        public void Add(PartRegion partRegion)
        {
            List<PartRegion> regionsToRemove = new List<PartRegion>();
            // Merge connected part regions
            foreach (var existingPartRegion in _partRegions)
            {
                if (partRegion.Merge(existingPartRegion)) regionsToRemove.Add(existingPartRegion);
            }
            // Remove merged part regions
            foreach (var regionToRemove in regionsToRemove)
            {
                _partRegions.Remove(regionToRemove);
            }
            // Add new/merged part region
            _partRegions.Add(partRegion);
        }
        public void SwitchMasterSlave()
        {
            int loopCount = 0;
            int switchCount = 1;
            List<PartRegion> allPartRegions = new List<PartRegion>();
            List<PartRegion> largePartRegions = new List<PartRegion>(_partRegions);
            HashSet<MasterSlaveItem> independentMasterSlaveItems = new HashSet<MasterSlaveItem>();
            do
            {
                allPartRegions.Clear();
                allPartRegions.AddRange(largePartRegions);
                largePartRegions.Clear();
                independentMasterSlaveItems.Clear();
                // Find independent master slave items
                foreach (var partRegion in allPartRegions)
                {
                    if (partRegion.MasterSlaveItems.Count <= 1) independentMasterSlaveItems.UnionWith(partRegion.MasterSlaveItems);
                }
                // Make master slave items independent
                foreach (var partRegion in allPartRegions)
                {
                    partRegion.MakeMasterSlaveItemsIndependent(independentMasterSlaveItems);
                }
                // Find multi-connected part regions
                foreach (var partRegion in allPartRegions)
                {
                    if (partRegion.MasterSlaveItems.Count > 1) largePartRegions.Add(partRegion);
                }
                //
                loopCount++;
            }
            while (allPartRegions.Count() != largePartRegions.Count() && loopCount < 10000);



            //
            loopCount = 0;
            int masterCount;
            while (switchCount > 0 && loopCount < 100000)
            {
                switchCount = 0;
                foreach (var partRegion in largePartRegions)
                {
                    if (partRegion.MasterSlaveItems.Count > 1)
                    {
                        masterCount = 0;
                        foreach (var masterSlaveItem in partRegion.MasterSlaveItems)
                        {
                            if (masterSlaveItem.MasterPartId == partRegion.PartId)
                            {
                                masterCount++;
                                if (masterCount > 1)
                                {
                                    masterSlaveItem.SwitchMasterSlave();
                                    switchCount++;
                                }
                            }
                        }
                    }
                }
                loopCount++;
            }
        }
    }
    //
    public class Node<T>
    {
        // Variables                                                                                                                
        private T _data;
        private NodeList<T> _neighbors = null;
        
        
        // Constructors                                                                                                             
        public Node()
        { }
        public Node(T data)
            : this(data, null)
        { }
        public Node(T data, NodeList<T> neighbors)
        {
            this._data = data;
            this._neighbors = neighbors;
        }


        // Methods                                                                                                                  
        public T Value { get { return _data; } set { _data = value; } }
        protected NodeList<T> Neighbors { get { return _neighbors; } set { _neighbors = value; } }
    }
    public class NodeList<T> : System.Collections.ObjectModel.Collection<Node<T>>
    {
        // Constructors                                                                                                             
        public NodeList()
            : base()
        { }


        // Methods                                                                                                                  
        public NodeList(int initialSize)
        {
            // Add the specified number of items
            for (int i = 0; i < initialSize; i++) base.Items.Add(default(Node<T>));
        }
        public Node<T> FindByValue(T value)
        {
            // Search the list for the value
            foreach (Node<T> node in Items)
                if (node.Value.Equals(value))
                    return node;

            // If we reached here, we didn't find a matching node
            return null;
        }
    }
    public class GraphNode<T> : Node<T>
    {
        // Variables                                                                                                                
        private int _id;


        // Properties                                                                                                               
        public int Id { get { return _id; } }


        // Constructors                                                                                                             
        public GraphNode()
            : base()
        { }
        public GraphNode(T value)
            : base(value)
        { }
        public GraphNode(T value, int id)
            : base(value)
        {
            _id = id;
        }
        public GraphNode(T value, NodeList<T> neighbors)
            : base(value, neighbors)
        { }


        // Methods                                                                                                                  
        new public NodeList<T> Neighbors
        {
            get
            {
                if (base.Neighbors == null)
                    base.Neighbors = new NodeList<T>();
                //
                return base.Neighbors;
            }
        }
    }
    public class Graph<T>
    {
        // Variables                                                                                                                
        private NodeList<T> _nodeSet;


        // Properties                                                                                                               
        public NodeList<T> Nodes { get { return _nodeSet; } }
        public int Count { get { return _nodeSet.Count; } }


        // Constructors                                                                                                             
        public Graph()
            : this(null)
        { }
        public Graph(NodeList<T> nodeSet)
        {
            if (nodeSet == null) this._nodeSet = new NodeList<T>();
            else this._nodeSet = nodeSet;
        }


        // Methods                                                                                                                  
        public void AddNode(GraphNode<T> node)
        {
            // Adds a node to the graph
            _nodeSet.Add(node);
        }
        public void AddNode(T value)
        {
            // Adds a node to the graph
            _nodeSet.Add(new GraphNode<T>(value, _nodeSet.Count() + 1));
        }
        public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to)
        {
            from.Neighbors.Add(to);
        }
        public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to)
        {
            from.Neighbors.Add(to);
            to.Neighbors.Add(from);
        }
        public bool Contains(T value)
        {
            return _nodeSet.FindByValue(value) != null;
        }
        public bool Remove(T value)
        {
            // First remove the node from the nodeset
            GraphNode<T> nodeToRemove = (GraphNode<T>)_nodeSet.FindByValue(value);
            if (nodeToRemove == null)
                // Node wasn't found
                return false; 
            // Otherwise, the node was found
            _nodeSet.Remove(nodeToRemove);
            // Enumerate through each node in the nodeSet, removing edges to this node
            foreach (GraphNode<T> gnode in _nodeSet)
            {
                int index = gnode.Neighbors.IndexOf(nodeToRemove);
                if (index != -1)
                {
                    // Remove the reference to the node and associated cost
                    gnode.Neighbors.RemoveAt(index);
                }
            }
            //
            return true;
        }
    }
    public class ContactGraph
    {
        // Variables                                                                                                                
        private Graph<HashSet<int>> _graph;


        // Constructors                                                                                                             
        public ContactGraph()
        {
            _graph = new Graph<HashSet<int>>();
        }


        // Methods                                                                                                                  
        public void AddMasterSlaveItems(IEnumerable<MasterSlaveItem> masterSlaveItems)
        {
            HashSet<int> mergedNode;
            List<HashSet<int>> nodes = new List<HashSet<int>>();
            List<HashSet<int>> nodesToRemove = new List<HashSet<int>>();
            // Collect geometry ids into nodes
            foreach (var masterSlaveItem in masterSlaveItems)
            {
                // Master side                                                          
                mergedNode = new HashSet<int>(masterSlaveItem.MasterGeometryIds);
                // Find intersecting
                foreach (var node in nodes)
                {
                    if (node.Intersect(mergedNode).Count() > 0)
                    {
                        mergedNode.UnionWith(node);
                        nodesToRemove.Add(node);
                    }
                }
                // Remove merged
                foreach (var node in nodesToRemove) nodes.Remove(node);
                // Add new/merged node
                nodes.Add(mergedNode);
                // Slave side                                                           
                nodesToRemove.Clear();
                mergedNode = new HashSet<int>(masterSlaveItem.SlaveGeometryIds);
                // Find intersecting
                foreach (var node in nodes)
                {
                    if (node.Intersect(mergedNode).Count() > 0)
                    {
                        mergedNode.UnionWith(node);
                        nodesToRemove.Add(node);
                    }
                }
                // Remove merged
                foreach (var node in nodesToRemove) nodes.Remove(node);
                // Add new/merged node
                nodes.Add(mergedNode);
            }
            // Add nodes to graph
            foreach (var node in nodes) _graph.AddNode(node);
            //
            GraphNode<HashSet<int>> masterNode;
            GraphNode<HashSet<int>> slaveNode;
            foreach (var masterSlaveItem in masterSlaveItems)
            {
                masterNode = null;
                slaveNode = null;
                foreach (GraphNode<HashSet<int>> graphNode in _graph.Nodes)
                {
                    if (masterNode == null && graphNode.Value.Intersect(masterSlaveItem.MasterGeometryIds).Count() > 0)
                        masterNode = graphNode;
                    else if (slaveNode == null && graphNode.Value.Intersect(masterSlaveItem.SlaveGeometryIds).Count() > 0)
                        slaveNode = graphNode;
                    if (masterNode != null && slaveNode != null) break;
                }
                //
                _graph.AddUndirectedEdge(masterNode, slaveNode);
            }
        }
        public void Go()
        {
            List<HashSet<int>> singleConnectedItems = new List<HashSet<int>>();
            do
            {
                singleConnectedItems.Clear();
                //
                foreach (GraphNode<HashSet<int>> node in _graph.Nodes)
                {
                    if (node.Neighbors.Count() <= 1) singleConnectedItems.Add(node.Value);
                }
                //
                foreach (var item in singleConnectedItems)
                {
                    _graph.Remove(item);
                }
            }
            while (singleConnectedItems.Count > 0);
            //
            HashSet<int> visited = new HashSet<int>();
            HashSet<int> allValues = new HashSet<int>();
            GraphNode<HashSet<int>> currentNode;
            Graph<HashSet<int>> isolatedGraph = new Graph<HashSet<int>>();
            Queue<GraphNode<HashSet<int>>> queue = new Queue<GraphNode<HashSet<int>>>();
            //
            if (_graph.Nodes.Count() > 0)
            {
                queue.Enqueue((GraphNode<HashSet<int>>)_graph.Nodes.First());
                while (queue.Count() > 0)
                {
                    currentNode = queue.Dequeue();
                    if (visited.Add(currentNode.Id))
                    {
                        isolatedGraph.AddNode(currentNode);
                        allValues.UnionWith(currentNode.Value);
                        foreach (var neighbour in currentNode.Neighbors) queue.Enqueue((GraphNode<HashSet<int>>)neighbour);
                    }
                }
            }
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
                masterSlaveItems.Add(new MasterSlaveItem(csp[0].Part.Name, csp[1].Part.Name,
                                                         csp[0].Part.PartId, csp[1].Part.PartId,
                                                         new HashSet<int>() { csp[0].GetGeometryId() },
                                                         new HashSet<int>() { csp[1].GetGeometryId() }));
            }
            //
            ContactGraph contactGraph = new ContactGraph();
            contactGraph.AddMasterSlaveItems(masterSlaveItems);
            contactGraph.Go();
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
            Dictionary<int[], MasterSlaveItem> partKeyMasterSlaveItems = new Dictionary<int[], MasterSlaveItem>(comparer);
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
                if (partKeyMasterSlaveItems.TryGetValue(key, out masterSlaveItem))
                {
                    masterSlaveItem.MasterGeometryIds.Add(csp[i].GetGeometryId());
                    masterSlaveItem.SlaveGeometryIds.Add(csp[j].GetGeometryId());
                }
                else
                {
                    masterSlaveItem = new MasterSlaveItem(csp[i].Part.Name, csp[j].Part.Name,
                                                          csp[i].Part.PartId, csp[j].Part.PartId,
                                                          new HashSet<int>(), new HashSet<int>());
                    masterSlaveItem.MasterGeometryIds.Add(csp[i].GetGeometryId());
                    masterSlaveItem.SlaveGeometryIds.Add(csp[j].GetGeometryId());
                    partKeyMasterSlaveItems.Add(key, masterSlaveItem);
                }
            }
            //
            ContactGraph contactGraph = new ContactGraph();
            contactGraph.AddMasterSlaveItems(partKeyMasterSlaveItems.Values);
            contactGraph.Go();
            //
            PartRegionCollection partRegionCollection = new PartRegionCollection();
            foreach (var entry in partKeyMasterSlaveItems)
            {
                partRegionCollection.Add(new PartRegion(entry.Value.MasterName, entry.Value.MasterPartId, entry.Value));
                partRegionCollection.Add(new PartRegion(entry.Value.SlaveName, entry.Value.SlavePartId, entry.Value));
            }
            partRegionCollection.SwitchMasterSlave();

            return partKeyMasterSlaveItems.Values.ToList();
        }
    }
}
