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
        private string _masterName;
        private string _slaveName;
        private HashSet<int> _masterGeometryIds;
        private HashSet<int> _slaveGeometryIds;


        // Properties                                                                                                               
        public string Name { get { return _masterName + "_to_" + _slaveName; } }
        public string MasterName { get { return _masterName; } }
        public string SlaveName { get { return _slaveName; } }
        public HashSet<int> MasterGeometryIds { get { return _masterGeometryIds; } set { _masterGeometryIds = value; } }
        public HashSet<int> SlaveGeometryIds { get { return _slaveGeometryIds; } set { _slaveGeometryIds = value; } }


        // Constructors                                                                                                             
        public MasterSlaveItem(string masterName, string slaveName,
                               HashSet<int> masterGeometryIds, HashSet<int> slaveGeometryIds)
        {
            _masterName = masterName;
            _slaveName = slaveName;
            _masterGeometryIds = masterGeometryIds;
            _slaveGeometryIds = slaveGeometryIds;
        }
        public void SwitchMasterSlave()
        {
            string tmpName = _masterName;
            _masterName = _slaveName;
            _slaveName = tmpName;
            //
            HashSet<int> tmpIds = _masterGeometryIds;
            _masterGeometryIds = _slaveGeometryIds;
            _slaveGeometryIds = tmpIds;
            //

        }
    }
    //
    public class Node<T>
    {
        // Variables                                                                                                                
        private T _data;
        private NodeList<T> _neighbors = null;


        // Propeties                                                                                                                  
        public T Value { get { return _data; } set { _data = value; } }
        public NodeList<T> Neighbors
        {
            get
            {
                if (_neighbors == null) _neighbors = new NodeList<T>();
                return _neighbors;
            }
            set { _neighbors = value; }
        }


        // Constructors                                                                                                             
        public Node()
        { }
        public Node(T data)
            : this(data, null)
        { }
        public Node(T data, NodeList<T> neighbors)
        {
            _data = data;
            _neighbors = neighbors;
        }
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
            {
                if (node.Value.Equals(value)) return node;
            }
            // If we reached here, we didn't find a matching node
            return null;
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
        public void AddNode(Node<T> node)
        {
            // Adds a node to the graph
            _nodeSet.Add(node);
        }
        public void AddDirectedEdge(Node<T> from, Node<T> to)
        {
            from.Neighbors.Add(to);
        }
        public void AddUndirectedEdge(Node<T> from, Node<T> to)
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
            Node<T> nodeToRemove = _nodeSet.FindByValue(value);
            return Remove(nodeToRemove);
        }
        public bool Remove(Node<T> node)
        {
            if (node == null) return false;
            // Remove the node
            _nodeSet.Remove(node);
            // Enumerate through each node in the nodeSet, removing edges to this node
            foreach (Node<T> gnode in _nodeSet)
            {
                int index = gnode.Neighbors.IndexOf(node);
                if (index != -1)
                {
                    // Remove the reference to the node and associated cost
                    gnode.Neighbors.RemoveAt(index);
                }
            }
            //
            return true;
        }
        public bool IsGraphSimple()
        {
            Node<T> currentNode;
            Node<T> parentNode = null;
            Queue<Node<T>> queue = new Queue<Node<T>>();
            Queue<Node<T>> parentQueue = new Queue<Node<T>>();
            HashSet<Node<T>> visitedNodes = new HashSet<Node<T>>();
            //
            queue.Enqueue(_nodeSet.First());
            parentQueue.Enqueue(_nodeSet.First());
            //
            while (queue.Count() > 0)
            {
                currentNode = queue.Dequeue();
                parentNode = parentQueue.Dequeue();
                //
                if (visitedNodes.Add(currentNode))
                {
                    // Add all neighbours to the queue
                    foreach (var neighbour in currentNode.Neighbors)
                    {
                        if (neighbour != parentNode)
                        {
                            queue.Enqueue(neighbour);
                            parentQueue.Enqueue(currentNode);
                        }
                    }
                }
                else return false;
            }
            return true;
        }
        public List<Graph<T>> GetConnectedGraphs()
        {
            Node<T> currentNode;
            HashSet<Node<T>> visitedNodes = new HashSet<Node<T>>();
            Queue<Node<T>> queue = new Queue<Node<T>>();
            NodeList<T> connectedNodes;
            List<Graph<T>> connectedGraphs = new List<Graph<T>>();
            //
            foreach (var node in _nodeSet)
            {
                // Check if the node was already added
                if (!visitedNodes.Contains(node))
                {
                    // Create new set of connected nodes
                    connectedNodes = new NodeList<T>();
                    //
                    queue.Enqueue(node);
                    // Search for connected nodes
                    while (queue.Count() > 0)
                    {
                        currentNode = queue.Dequeue();
                        if (visitedNodes.Add(currentNode))
                        {
                            connectedNodes.Add(currentNode);
                            // Add all neighbour to the queue
                            foreach (var neighbour in currentNode.Neighbors)
                                queue.Enqueue(neighbour);
                        }
                    }
                    //
                    connectedGraphs.Add(new Graph<T>(connectedNodes));
                }
            }
            //
            return connectedGraphs;
        }
    }
    public class NodeData
    {
        // Variables                                                                                                                
        private int _id;
        private string _name;
        private HashSet<int> _itemIds;


        // Properties                                                                                                               
        public int Id { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public HashSet<int> ItemIds { get { return _itemIds; } set { _itemIds = value; } }


        // Constructors                                                                                                             
        public NodeData(int id, string name, HashSet<int> data)
        {
            _id = id;
            _name = name;
            _itemIds = data;
        }
    }
    public class ContactGraph
    {
        // Variables                                                                                                                
        private Graph<NodeData> _graph;


        // Constructors                                                                                                             
        public ContactGraph()
        {
            _graph = new Graph<NodeData>();
        }


        // Methods                                                                                                                  
        public void AddMasterSlaveItems(IEnumerable<MasterSlaveItem> masterSlaveItems, FeMesh mesh)
        {
            HashSet<int> mergedItemIds;
            List<HashSet<int>> itemIdsList = new List<HashSet<int>>();
            List<HashSet<int>> itemIdsListToRemove = new List<HashSet<int>>();
            // Collect item ids
            foreach (var masterSlaveItem in masterSlaveItems)
            {
                // Master side                                                          
                mergedItemIds = new HashSet<int>(masterSlaveItem.MasterGeometryIds);
                // Find intersecting
                foreach (var node in itemIdsList)
                {
                    if (node.Intersect(mergedItemIds).Count() > 0)
                    {
                        mergedItemIds.UnionWith(node);
                        itemIdsListToRemove.Add(node);
                    }
                }
                // Remove merged
                foreach (var node in itemIdsListToRemove) itemIdsList.Remove(node);
                // Add new/merged item
                itemIdsList.Add(mergedItemIds);
                // Slave side                                                           
                itemIdsListToRemove.Clear();
                mergedItemIds = new HashSet<int>(masterSlaveItem.SlaveGeometryIds);
                // Find intersecting
                foreach (var node in itemIdsList)
                {
                    if (node.Intersect(mergedItemIds).Count() > 0)
                    {
                        mergedItemIds.UnionWith(node);
                        itemIdsListToRemove.Add(node);
                    }
                }
                // Remove merged
                foreach (var node in itemIdsListToRemove) itemIdsList.Remove(node);
                // Add new/merged item
                itemIdsList.Add(mergedItemIds);
            }
            // Add items to graph
            int id;
            string name;
            List<string> allNames = new List<string>();
            NodeData nodeData;
            foreach (var itemIds in itemIdsList)
            {
                id = _graph.Nodes.Count() + 1;
                name = GetNameFromItemIds(itemIds, allNames, mesh);
                nodeData = new NodeData(id, name, itemIds);
                //
                _graph.AddNode(new Node<NodeData>(nodeData));
                allNames.Add(name);
            }
            // Add edges to graph
            Node<NodeData> masterNode;
            Node<NodeData> slaveNode;
            foreach (var masterSlaveItem in masterSlaveItems)
            {
                masterNode = null;
                slaveNode = null;
                foreach (Node<NodeData> node in _graph.Nodes)
                {
                    // Find the nodes
                    if (masterNode == null && node.Value.ItemIds.Intersect(masterSlaveItem.MasterGeometryIds).Count() > 0)
                        masterNode = node;
                    else if (slaveNode == null && node.Value.ItemIds.Intersect(masterSlaveItem.SlaveGeometryIds).Count() > 0)
                        slaveNode = node;
                    if (masterNode != null && slaveNode != null) break;
                }
                //
                _graph.AddUndirectedEdge(masterNode, slaveNode);
            }
        }
        
        public List<MasterSlaveItem> GetMasterSlaveItems()
        {
            List<Graph<NodeData>> connectedGraphList = _graph.GetConnectedGraphs();
            List<bool> isSimple = new List<bool>();
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            foreach (var connectedGraph in connectedGraphList)
            {
                if (connectedGraph.IsGraphSimple())
                {
                    isSimple.Add(true);
                    //
                    masterSlaveItems.AddRange(GetMasterSlaveItemsFromSimpleGraph(connectedGraph));
                }
                else
                {
                    isSimple.Add(false);
                }
            }
            return masterSlaveItems;














            //
            List<NodeData> singleConnectedItems = new List<NodeData>();
            do
            {
                singleConnectedItems.Clear();
                //
                foreach (Node<NodeData> node in _graph.Nodes)
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
            HashSet<int> visitedIds = new HashSet<int>();
            HashSet<int> allItemIds = new HashSet<int>();
            Node<NodeData> currentNode;
            Graph<NodeData> isolatedGraph = new Graph<NodeData>();
            Queue<Node<NodeData>> queue = new Queue<Node<NodeData>>();
            //
            if (_graph.Nodes.Count() > 0)
            {
                queue.Enqueue(_graph.Nodes.First());
                while (queue.Count() > 0)
                {
                    currentNode = queue.Dequeue();
                    if (visitedIds.Add(currentNode.Value.Id))
                    {
                        isolatedGraph.AddNode(currentNode);
                        allItemIds.UnionWith(currentNode.Value.ItemIds);
                        foreach (var neighbour in currentNode.Neighbors) queue.Enqueue(neighbour);
                    }
                }
            }
        }
        //
        private static string GetNameFromItemIds(HashSet<int> itemIds, List<string> allNames, FeMesh mesh)
        {
            string name;
            HashSet<int> partIds = new HashSet<int>();
            foreach (var itemId in itemIds)
            {
                partIds.Add(FeMesh.GetPartIdFromGeometryId(itemId));
            }
            if (partIds.Count == 1) name = mesh.GetPartNamesByIds(partIds.ToArray())[0];
            else name = allNames.GetNextNumberedKey("Mixed");
            //
            return name;
        }
        private static List<MasterSlaveItem> GetMasterSlaveItemsFromSimpleGraph(Graph<NodeData> graph)
        {
            Node<NodeData> neighbour;
            List<Node<NodeData>> singleConnectedNodes = new List<Node<NodeData>>();
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            //
            do
            {
                singleConnectedNodes.Clear();
                //
                foreach (Node<NodeData> node in graph.Nodes)
                {
                    if (node.Neighbors.Count() == 1)
                    {
                        neighbour = node.Neighbors[0];
                        masterSlaveItems.Add(new MasterSlaveItem(node.Value.Name, neighbour.Value.Name,
                                                                 node.Value.ItemIds, neighbour.Value.ItemIds));
                        //
                        singleConnectedNodes.Add(node);
                        //
                        break;  // this makes master slave parts more organized for the user
                    }
                }
                //
                foreach (var node in singleConnectedNodes)
                {
                    graph.Remove(node);
                }
            }
            while (graph.Count > 1);
            //
            return masterSlaveItems;
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
                                                         new HashSet<int>() { csp[0].GetGeometryId() },
                                                         new HashSet<int>() { csp[1].GetGeometryId() }));
            }
            //
            ContactGraph contactGraph = new ContactGraph();
            contactGraph.AddMasterSlaveItems(masterSlaveItems, _mesh);
            masterSlaveItems = contactGraph.GetMasterSlaveItems();
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
                                                          new HashSet<int>(), new HashSet<int>());
                    masterSlaveItem.MasterGeometryIds.Add(csp[i].GetGeometryId());
                    masterSlaveItem.SlaveGeometryIds.Add(csp[j].GetGeometryId());
                    partKeyMasterSlaveItems.Add(key, masterSlaveItem);
                }
            }
            //
            ContactGraph contactGraph = new ContactGraph();
            contactGraph.AddMasterSlaveItems(partKeyMasterSlaveItems.Values, _mesh);
            List<MasterSlaveItem> masterSlaveItems = contactGraph.GetMasterSlaveItems();
            //
            return masterSlaveItems;
          }
    }
}
