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
        private GeometryType _geometryType;
        private HashSet<int> _nodeIds;
        private BoundingBox _boundingBox;
        private bool _internal;


        // Properties                                                                                                               
        public FeMesh Mesh { get { return _mesh; } set { _mesh = value; } }
        public BasePart Part { get { return _part; } set { _part = value; } }
        public int Id { get { return _id; } set { _id = value; } }
        public GeometryType GeometryType { get { return _geometryType; } set { _geometryType = value; } }
        public HashSet<int> NodeIds { get { return _nodeIds; } set { _nodeIds = value; } }
        public BoundingBox BoundingBox { get { return _boundingBox; } set { _boundingBox = value; } }
        public bool Internal { get { return _internal; } set { _internal = value; } }


        // Constructors                                                                                                             
        public ContactSurface(FeMesh mesh, double[][] _nodes, BasePart part, int id, GeometryType geometryType,
                              double boundingBoxOffset)
        {
            _mesh = mesh;
            _part = part;
            _id = id;
            _geometryType = geometryType;
            //
            if (_geometryType == GeometryType.ShellEdgeSurface) _nodeIds = part.Visualization.GetNodeIdsByEdge(_id);
            else _nodeIds = part.Visualization.GetNodeIdsBySurface(_id);
            //
            _boundingBox = new BoundingBox();
            _boundingBox.IncludeFirstCoor(_nodes[_nodeIds.First()]);
            foreach (var nodeId in _nodeIds) _boundingBox.IncludeCoorFast(_nodes[nodeId]);
            _boundingBox.Inflate(boundingBoxOffset);
            //
            _internal = false;
        }
        public ContactSurface(ContactSurface contactSurface)
        {
            _mesh = contactSurface._mesh;
            _part = contactSurface._part;
            _id = contactSurface._id;
            _geometryType = contactSurface._geometryType;
            _nodeIds = new HashSet<int>(contactSurface._nodeIds);
            _boundingBox = new BoundingBox(contactSurface._boundingBox);
            _internal = contactSurface._internal;
        }

        // Methods                                                                                                                  
        public int[] GetCell(int cellId)
        {
            if (_geometryType == GeometryType.ShellEdgeSurface)
            {
                return _part.Visualization.EdgeCells[cellId];
            }
            else
            {
                int[] cell = _part.Visualization.Cells[cellId];
                //
                if (_geometryType == GeometryType.ShellBackSurface)
                {
                    if (cell.Length == 3) return new int[] { cell[0], cell[2], cell[1] };
                    else if (cell.Length == 4) return new int[] { cell[0], cell[3], cell[2], cell[1] };
                    else if (cell.Length == 6) return new int[] { cell[0], cell[2], cell[1], cell[5], cell[4], cell[3] };
                    else if (cell.Length == 8) return new int[] { cell[0], cell[3], cell[2], cell[1],
                                                                  cell[7], cell[6], cell[5], cell[4] };
                    else throw new NotSupportedException();
                }
                else return cell;
            }
        }        
        public double[] GetCellNormal(int cellId)
        {
            int[] cell = _part.Visualization.Cells[cellId];
            double[] n1 = _mesh.Nodes[cell[0]].Coor;
            double[] n2 = _mesh.Nodes[cell[1]].Coor;
            double[] n3 = _mesh.Nodes[cell[2]].Coor;
            double[] a = new double[3];
            double[] b = new double[3];
            double[] n = new double[3];
            Geometry.VmV(ref a, n2, n1);
            Geometry.VmV(ref b, n3, n1);
            Geometry.VcrossV(ref n, a, b);
            Geometry.Vnorm(ref n, n);
            return n;
        }
        public void GetEdgeCellNormal(int edgeCellId, ref double[] cellNormal, ref double[] edgeCellNormal)
        {
            int baseCellId = _part.Visualization.GetEdgeCellBaseCellIdSlow(edgeCellId);
            //
            int[] cell = _part.Visualization.Cells[baseCellId];
            int[] edgeCell = _part.Visualization.EdgeCells[edgeCellId];
            int n1Id;
            int numNodes;
            if (cell.Length == 3 || cell.Length == 6) numNodes = 3;
            else numNodes = 4;
            // Find the first edge node
            if (edgeCell[0] == cell[0]) n1Id = 0;
            else if (edgeCell[0] == cell[1]) n1Id = 1;
            else if (edgeCell[0] == cell[2]) n1Id = 2;
            else if (numNodes == 4 && edgeCell[0] == cell[3]) n1Id = 3;
            else throw new NotSupportedException();
            // Find the second node
            int delta;
            if (edgeCell[1] == cell[(n1Id + 1) % numNodes]) delta = 1;
            else delta = -1;
            //
            double[] n1 = _mesh.Nodes[cell[n1Id]].Coor;
            double[] n2 = _mesh.Nodes[cell[(numNodes + n1Id + 1 * delta) % numNodes]].Coor;
            double[] n3 = _mesh.Nodes[cell[(numNodes + n1Id + 2 * delta) % numNodes]].Coor;
            double[] a = new double[3];
            double[] b = new double[3];
            double[] n = new double[3];
            Geometry.VmV(ref a, n2, n1);
            Geometry.VmV(ref b, n3, n1);
            Geometry.VcrossV(ref n, a, b);
            Geometry.Vnorm(ref cellNormal, n);
            //
            Geometry.VcrossV(ref edgeCellNormal, a, n);
            Geometry.Vnorm(ref edgeCellNormal, edgeCellNormal);
        }
        public int GetGeometryId()
        {
            return _id * 100000 + (int)_geometryType * 10000 + _part.PartId;
        }
    }
    public class MasterSlaveItem
    {
        // Variables                                                                                                                
        private string _masterName;
        private string _slaveName;
        private HashSet<int> _masterGeometryIds;
        private HashSet<int> _slaveGeometryIds;
        private bool _unresolved;


        // Properties                                                                                                               
        public string Name
        {
            get
            {
                if (_unresolved) return _masterName;
                else return _masterName + "_to_" + _slaveName;
            }
        }
        public string MasterName { get { return _masterName; } }
        public string SlaveName { get { return _slaveName; } }
        public HashSet<int> MasterGeometryIds { get { return _masterGeometryIds; } set { _masterGeometryIds = value; } }
        public HashSet<int> SlaveGeometryIds { get { return _slaveGeometryIds; } set { _slaveGeometryIds = value; } }
        public bool Unresolved { get { return _unresolved; } set { _unresolved = value; } }
        public string GeometryType
        {
            get
            {
                string name = GetGeometryTypeName(_masterGeometryIds);
                if (name.Length > 0) name += "-";
                name += GetGeometryTypeName(_slaveGeometryIds);
                return name;
            }
        }


        // Constructors                                                                                                             
        public MasterSlaveItem(string masterName, string slaveName,
                               HashSet<int> masterGeometryIds, HashSet<int> slaveGeometryIds)
        {
            _masterName = masterName;
            _slaveName = slaveName;
            _masterGeometryIds = masterGeometryIds;
            _slaveGeometryIds = slaveGeometryIds;
            _unresolved = false;
        }
        public void SwapMasterSlave()
        {
            string tmpName = _masterName;
            _masterName = _slaveName;
            _slaveName = tmpName;
            //
            HashSet<int> tmpGeometryIds = _masterGeometryIds;
            _masterGeometryIds = _slaveGeometryIds;
            _slaveGeometryIds = tmpGeometryIds;
        }
        private string GetGeometryTypeName(HashSet<int> geometryIds)
        {
            if (geometryIds == null) return "";
            //
            int typeId;
            string name = "";
            HashSet<int> typeIds = new HashSet<int>();
            //
            foreach (int geometryId in geometryIds) typeIds.Add(FeMesh.GetItemTypePartIdsFromGeometryId(geometryId)[1]);
            //
            if (typeIds.Count == 0) { }
            else if (typeIds.Count == 1)
            {
                typeId = typeIds.First();
                if (typeId == (int)CaeMesh.GeometryType.SolidSurface ||
                    typeId == (int)CaeMesh.GeometryType.ShellFrontSurface ||
                    typeId == (int)CaeMesh.GeometryType.ShellBackSurface) name += "Surface";
                else if (typeId == (int)CaeMesh.GeometryType.ShellEdgeSurface) name += "Edge";
                else throw new NotSupportedException();
            }
            else name += "Mixed";
            //
            return name;
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
        {
            _nodeSet = new NodeList<T>();
        }
        public Graph(Graph<T> graph)
            : this(graph.Nodes)
        { }
        public Graph(NodeList<T> nodeSet)
        {
            _nodeSet = new NodeList<T>();
            //
            if (nodeSet != null)
            {
                Node<T> newNode;
                Dictionary<Node<T>, Node<T>> oldNewNode = new Dictionary<Node<T>, Node<T>>();
                // Create new nodes
                foreach (var oldNode in nodeSet)
                {
                    newNode = new Node<T>(oldNode.Value);
                    AddNode(newNode);
                    oldNewNode.Add(oldNode, newNode);
                }
                // Add connections
                foreach (var oldNode in nodeSet)
                {
                    foreach (var neighbour in oldNode.Neighbors)
                    {
                        AddDirectedEdge(oldNewNode[oldNode], oldNewNode[neighbour]);
                    }
                }
            }
        }
        

        // Methods                                                                                                                  
        public void AddNode(Node<T> node)
        {
            // Adds a node to the graph
            _nodeSet.Add(node);
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
        // Edges
        public void AddDirectedEdge(Node<T> from, Node<T> to)
        {
            from.Neighbors.Add(to);
        }
        public void AddUndirectedEdge(Node<T> from, Node<T> to)
        {
            from.Neighbors.Add(to);
            to.Neighbors.Add(from);
        }
        public void RemoveDirectedEdge(Node<T> from, Node<T> to)
        {
            from.Neighbors.Remove(to);
        }
        public void RemoveUndirectedEdge(Node<T> from, Node<T> to)
        {
            from.Neighbors.Remove(to);
            to.Neighbors.Remove(from);
        }
        //
        public bool IsGraphWithoutCycles()
        {
            Node<T> currentNode;
            Node<T> parentNode;
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
                // Check for cycles
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
        public bool IsGraphWithOneCycle()
        {
            Graph<T> graphCopy = new Graph<T>(_nodeSet);
            List<T> singleConnectedItems = new List<T>();
            // Remove all open branches
            do
            {
                singleConnectedItems.Clear();
                //
                foreach (Node<T> node in graphCopy.Nodes)
                {
                    if (node.Neighbors.Count() <= 1) singleConnectedItems.Add(node.Value);
                }
                //
                foreach (var item in singleConnectedItems)
                {
                    graphCopy.Remove(item);
                }
            }
            while (singleConnectedItems.Count > 0);
            // Check for a single cycle
            foreach (Node<T> node in graphCopy.Nodes)
            {
                if (node.Neighbors.Count() > 2) return false;
            }
            return true;
        }
        public List<Graph<T>> GeConnectedSubgraphs()
        {
            Node<T> currentNode;
            HashSet<Node<T>> visitedNodes = new HashSet<Node<T>>();
            Queue<Node<T>> queue = new Queue<Node<T>>();
            NodeList<T> connectedNodes;
            List<Graph<T>> connectedSubgraphs = new List<Graph<T>>();
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
                    connectedSubgraphs.Add(new Graph<T>(connectedNodes));
                }
            }
            //
            return connectedSubgraphs;
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
        //
        public List<MasterSlaveItem> GetMasterSlaveItems(bool checkUnresolved)
        {
            List<Graph<NodeData>> connectedSubgraphList = _graph.GeConnectedSubgraphs();
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            //
            foreach (var connectedGraph in connectedSubgraphList)
            {
                if (connectedGraph.IsGraphWithoutCycles())
                {
                    masterSlaveItems.AddRange(GetMasterSlaveItemsFromGraphWithoutCycles(connectedGraph));
                }
                else if (connectedGraph.IsGraphWithOneCycle())
                {
                    masterSlaveItems.AddRange(GetMasterSlaveItemsFromGraphWithOneCycle(connectedGraph));
                }
                else
                {
                    masterSlaveItems.AddRange(GetMasterSlaveItemsFromGraphWithMultipleCycles(connectedGraph, checkUnresolved));
                }
            }
            return masterSlaveItems;
        }
        //
        public static string GetNameFromItemIds(HashSet<int> itemIds, List<string> allNames, FeMesh mesh)
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
        private static List<MasterSlaveItem> GetMasterSlaveItemsFromGraphWithoutCycles(Graph<NodeData> graph)
        {
            Graph<NodeData> reducedGraph;
            return GetMasterSlaveItemsFromGraphWithoutCycles(graph, out reducedGraph);
        }
        private static List<MasterSlaveItem> GetMasterSlaveItemsFromGraphWithoutCycles(Graph<NodeData> graph,
                                                                                       out Graph<NodeData> reducedGraph)
        {
            Node<NodeData> neighbour;
            List<Node<NodeData>> singleConnectedNodes = new List<Node<NodeData>>();
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            reducedGraph = new Graph<NodeData>(graph);
            //
            do
            {
                singleConnectedNodes.Clear();
                //
                foreach (Node<NodeData> node in reducedGraph.Nodes)
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
                    reducedGraph.Remove(node);
                }
            }
            while (singleConnectedNodes.Count > 0);
            //
            return masterSlaveItems;
        }
        private static List<MasterSlaveItem> GetMasterSlaveItemsFromGraphWithOneCycle(Graph<NodeData> graph)
        {
            Graph<NodeData> reducedGraph;
            List<MasterSlaveItem> masterSlaveItems = GetMasterSlaveItemsFromGraphWithoutCycles(graph, out reducedGraph);
            //
            Node<NodeData> currentNode;
            Node<NodeData> parentNode;
            Queue<Node<NodeData>> queue = new Queue<Node<NodeData>>();
            Queue<Node<NodeData>> parentQueue = new Queue<Node<NodeData>>();
            HashSet<Node<NodeData>> visitedNodes = new HashSet<Node<NodeData>>();
            //
            queue.Enqueue(reducedGraph.Nodes.First());
            parentQueue.Enqueue(reducedGraph.Nodes.First());
            //
            while (queue.Count() > 0)
            {
                currentNode = queue.Dequeue();
                parentNode = parentQueue.Dequeue();
                // Check for cycles
                if (visitedNodes.Add(currentNode))
                {
                    // Add all neighbours to the queue
                    foreach (var neighbour in currentNode.Neighbors)
                    {
                        if (neighbour != parentNode)
                        {
                            masterSlaveItems.Add(new MasterSlaveItem(currentNode.Value.Name, neighbour.Value.Name,
                                                                     currentNode.Value.ItemIds, neighbour.Value.ItemIds));
                            //
                            queue.Enqueue(neighbour);
                            parentQueue.Enqueue(currentNode);
                            break;  // add only the first neighbor and continue
                        }
                    }
                }
            }
            //
            return masterSlaveItems;
        }
        private static List<MasterSlaveItem> GetMasterSlaveItemsFromGraphWithMultipleCycles(Graph<NodeData> graph,
                                                                                            bool checkUnresolved)
        {
            string prefix = "";
            HashSet<int> ids = new HashSet<int>();
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            // Create an unresolved master slave item that is shown to the user as a single surface
            if (checkUnresolved)
            {
                MasterSlaveItem masterSlaveItem;
                foreach (var node in graph.Nodes) ids.UnionWith(node.Value.ItemIds);
                masterSlaveItem = new MasterSlaveItem("Unresolved", "", ids, null);
                masterSlaveItem.Unresolved = true;
                masterSlaveItems.Add(masterSlaveItem);
                //
                prefix = "Unresolved_";
            }
            // Create master slave items
            Graph<NodeData> reducedGraph;
            masterSlaveItems.AddRange(GetMasterSlaveItemsFromGraphWithoutCycles(graph, out reducedGraph));
            //
            foreach (var node in reducedGraph.Nodes)
            {
                foreach (var neighbor in node.Neighbors)
                {
                    masterSlaveItems.Add(new MasterSlaveItem(prefix + node.Value.Name, neighbor.Value.Name,
                                                             node.Value.ItemIds, neighbor.Value.ItemIds));
                    reducedGraph.RemoveDirectedEdge(neighbor, node);
                }
                node.Neighbors.Clear();
            }
            //
            return masterSlaveItems;
        }
    }
    //
    public class ContactSearch
    {
        // Variables                                                                                                                
        private FeMesh _mesh;
        private FeMesh _geometry;
        private double[][] _nodes;
        private BoundingBox[][] _cellBoundingBoxes;
        private BoundingBox[][] _cellEdgeBoundingBoxes;
        private bool _includeSelfContact;
        private GroupContactPairsByEnum _groupContactPairsBy;


        // Properties                                                                                                               
        public GroupContactPairsByEnum GroupContactPairsBy
        {
            get { return _groupContactPairsBy; }
            set { _groupContactPairsBy = value; }
        }


        // Constructors                                                                                                             
        public ContactSearch(FeMesh mesh, FeMesh geometry)
        {
            _mesh = mesh;
            _geometry = geometry;
            //
            if (_mesh != null && _mesh.Nodes != null)
            {
                _nodes = new double[_mesh.MaxNodeId + 1][];
                foreach (var nodeEntry in _mesh.Nodes) _nodes[nodeEntry.Key] = nodeEntry.Value.Coor;
            }
            //
            _includeSelfContact = false;
            _groupContactPairsBy = GroupContactPairsByEnum.None;
        }


        // Methods                                                                                                                  
        public List<MasterSlaveItem> FindContactPairs(double distance, double angleDeg, bool checkUnresolved)
        {
            if (_mesh == null) return null;
            // Bounding boxes for each cell
            int[] cell;
            BoundingBox bb;
            _cellBoundingBoxes = new BoundingBox[_mesh.GetMaxPartId() + 1][];
            _cellEdgeBoundingBoxes = new BoundingBox[_mesh.GetMaxPartId() + 1][];
            //
            foreach (var partEntry in _mesh.Parts)
            {
                _cellBoundingBoxes[partEntry.Value.PartId] = new BoundingBox[partEntry.Value.Visualization.Cells.Length];
                for (int i = 0; i < partEntry.Value.Visualization.Cells.Length; i++)
                {
                    cell = partEntry.Value.Visualization.Cells[i];
                    bb = new BoundingBox();
                    bb.IncludeFirstCoor(_nodes[cell[0]]);
                    bb.IncludeCoorFast(_nodes[cell[1]]);
                    bb.IncludeCoorFast(_nodes[cell[2]]);
                    if (cell.Length == 4 || cell.Length == 8) bb.IncludeCoorFast(_nodes[cell[3]]);
                    bb.Inflate(distance * 0.5);
                    //
                    _cellBoundingBoxes[partEntry.Value.PartId][i] = bb;
                }
                // Shell edge faces
                if (partEntry.Value.PartType == PartType.Shell)
                {
                    _cellEdgeBoundingBoxes[partEntry.Value.PartId]
                        = new BoundingBox[partEntry.Value.Visualization.EdgeCells.Length];
                    for (int i = 0; i < partEntry.Value.Visualization.EdgeCells.Length; i++)
                    {
                        cell = partEntry.Value.Visualization.EdgeCells[i];
                        bb = new BoundingBox();
                        bb.IncludeFirstCoor(_nodes[cell[0]]);
                        bb.IncludeCoorFast(_nodes[cell[1]]);
                        if (cell.Length == 3) bb.IncludeCoorFast(_nodes[cell[2]]);
                        bb.Inflate(distance * 0.5);
                        //
                        _cellEdgeBoundingBoxes[partEntry.Value.PartId][i] = bb;
                    }
                }
            }
            // Find all surfaces of the assembly
            int count = 0;
            foreach (var partEntry in _mesh.Parts)
            {
                if (partEntry.Value.PartType == PartType.Solid) count += partEntry.Value.Visualization.FaceCount;
                else if (partEntry.Value.PartType == PartType.Shell)
                {
                    count += 2 * partEntry.Value.Visualization.FaceCount;
                    count += partEntry.Value.Visualization.EdgeCount;
                }
            }
            ContactSurface[] contactSurfaces = new ContactSurface[count];
            //
            count = 0;
            HashSet<int> freeEdgeIds;
            foreach (var partEntry in _mesh.Parts)
            {
                if (partEntry.Value.PartType == PartType.Solid)
                {
                    // Solid faces
                    for (int i = 0; i < partEntry.Value.Visualization.FaceCount; i++)
                    {
                        contactSurfaces[count++] = new ContactSurface(_mesh, _nodes, partEntry.Value, i,
                                                                      GeometryType.SolidSurface, distance * 0.5);
                    }
                }
                else if (partEntry.Value.PartType == PartType.Shell)
                {
                    // Sehll faces
                    for (int i = 0; i < partEntry.Value.Visualization.FaceCount; i++)
                    {
                        contactSurfaces[count++] = new ContactSurface(_mesh, _nodes, partEntry.Value, i,
                                                                      GeometryType.ShellFrontSurface, distance * 0.5);
                        contactSurfaces[count] = new ContactSurface(contactSurfaces[count - 1]);    // copy
                        contactSurfaces[count].GeometryType = GeometryType.ShellBackSurface;
                        count++;
                    }
                    // Shell edge faces
                    freeEdgeIds = partEntry.Value.Visualization.GetFreeEdgeIds();
                    for (int i = 0; i < partEntry.Value.Visualization.EdgeCount; i++)
                    {
                        contactSurfaces[count] = new ContactSurface(_mesh, _nodes, partEntry.Value, i,
                                                                    GeometryType.ShellEdgeSurface, distance * 0.5);
                        // Fnd internal edges on shells
                        if (!freeEdgeIds.Contains(i)) contactSurfaces[count].Internal = true;
                        count++;
                    }
                }
            }
            // Find internal surfaces on compound parts
            for (int i = 0; i < contactSurfaces.Length; i++)
            {                
                for (int j = i + 1; j < contactSurfaces.Length; j++)
                {
                    if (contactSurfaces[i].GeometryType == contactSurfaces[j].GeometryType && // skip front and back shell
                        contactSurfaces[i].NodeIds.Count() == contactSurfaces[j].NodeIds.Count() &&
                        contactSurfaces[i].BoundingBox.Intersects(contactSurfaces[j].BoundingBox) &&
                        contactSurfaces[i].NodeIds.Union(contactSurfaces[j].NodeIds).Count() == contactSurfaces[i].NodeIds.Count)
                    {
                        contactSurfaces[i].Internal = true;
                        contactSurfaces[j].Internal = true;
                    }
                }
            }
            // Find all surface pairs in contact
            double angleRad = angleDeg * Math.PI / 180;
            List<ContactSurface[]> contactSurfacePairs = new List<ContactSurface[]>();
            //
            for (int i = 0; i < contactSurfaces.Length; i++)
            {
                for (int j = i + 1; j < contactSurfaces.Length; j++)
                {
                    if (CheckSurfaceToSurfaceDistance(contactSurfaces[i], contactSurfaces[j], distance, angleRad))
                    {
                        contactSurfacePairs.Add(new ContactSurface[] { contactSurfaces[i], contactSurfaces[j] });
                    }
                }
            }
            // Group surface pairs
            if (_groupContactPairsBy == GroupContactPairsByEnum.None)
                return GroupContactPairsByNone(contactSurfacePairs, checkUnresolved);
            else if (_groupContactPairsBy == GroupContactPairsByEnum.BySurfaceAngle)
                throw new NotSupportedException();
            else if (_groupContactPairsBy == GroupContactPairsByEnum.ByParts)
                return GroupContactPairsByParts(contactSurfacePairs, checkUnresolved);
            else throw new NotSupportedException();
        }
        private bool CheckSurfaceToSurfaceDistance(ContactSurface cs1, ContactSurface cs2, double distance, double angleRad)
        {
            if (cs1.Internal || cs2.Internal) return false;
            if (!_includeSelfContact && cs1.Part == cs2.Part) return false;
            if (!cs1.BoundingBox.Intersects(cs2.BoundingBox)) return false;
            //
            BoundingBox bb1;
            BoundingBox bb2;
            BoundingBox[] bbs1;
            BoundingBox[] bbs2;
            BoundingBox intersection = cs1.BoundingBox.GetIntersection(cs2.BoundingBox);
            //
            int[] cell1;
            int[] cell2;
            int[] cell1Ids;
            int[] cell2Ids;
            double[] cellNorm = new double[3];
            double[] edgeCellNorm = new double[3];
            double[][] t1 = new double[3][];
            double[][] t2 = new double[3][];
            t1[2] = new double[3];
            t2[2] = new double[3];
            //
            bool edgeSurface1 = cs1.GeometryType == GeometryType.ShellEdgeSurface;
            bool edgeSurface2 = cs2.GeometryType == GeometryType.ShellEdgeSurface;
            // Use face cell ids or edge cell ids
            if (edgeSurface1)
            {
                bbs1 = _cellEdgeBoundingBoxes[cs1.Part.PartId];
                cell1Ids = cs1.Part.Visualization.EdgeCellIdsByEdge[cs1.Id];
            }
            else
            {
                bbs1 = _cellBoundingBoxes[cs1.Part.PartId];
                cell1Ids = cs1.Part.Visualization.CellIdsByFace[cs1.Id];
            }
            //
            foreach (int cell1Id in cell1Ids)
            {
                bb1 = bbs1[cell1Id];
                //
                if (bb1.MaxX < intersection.MinX) continue;
                else if (bb1.MinX > intersection.MaxX) continue;
                else if (bb1.MaxY < intersection.MinY) continue;
                else if (bb1.MinY > intersection.MaxY) continue;
                else if (bb1.MaxZ < intersection.MinZ) continue;
                else if (bb1.MinZ > intersection.MaxZ) continue;
                else
                {
                    // Use face cell ids or edge cell ids
                    if (edgeSurface2)
                    {
                        bbs2 = _cellEdgeBoundingBoxes[cs2.Part.PartId];
                        cell2Ids = cs2.Part.Visualization.EdgeCellIdsByEdge[cs2.Id];
                    }
                    else
                    {
                        bbs2 = _cellBoundingBoxes[cs2.Part.PartId];
                        cell2Ids = cs2.Part.Visualization.CellIdsByFace[cs2.Id];
                    }
                    //
                    foreach (int cell2Id in cell2Ids)
                    {
                        bb2 = bbs2[cell2Id];
                        //
                        if (bb1.MaxX < bb2.MinX) continue;
                        else if (bb1.MinX > bb2.MaxX) continue;
                        else if (bb1.MaxY < bb2.MinY) continue;
                        else if (bb1.MinY > bb2.MaxY) continue;
                        else if (bb1.MaxZ < bb2.MinZ) continue;
                        else if (bb1.MinZ > bb2.MaxZ) continue;
                        else
                        {
                            cell1 = cs1.GetCell(cell1Id);   // reverse cell ids for shell back faces or get shell edge cell
                            cell2 = cs2.GetCell(cell2Id);   // reverse cell ids for shell back faces or get shell edge cell
                            // Cell 1 triangle 1
                            t1[0] = _nodes[cell1[0]];
                            t1[1] = _nodes[cell1[1]];
                            if (!edgeSurface1) t1[2] = _nodes[cell1[2]];
                            // Cell 2 triangle 1
                            t2[0] = _nodes[cell2[0]];
                            t2[1] = _nodes[cell2[1]];
                            if (!edgeSurface2) t2[2] = _nodes[cell2[2]];
                            // 1. triangle is edge segment
                            if (edgeSurface1)
                            {
                                cs1.GetEdgeCellNormal(cell1Id, ref cellNorm, ref edgeCellNorm);
                                Geometry.VpVxS(ref t1[2], t1[1], cellNorm, 0.001 * distance);
                            }
                            // 2. triangle is edge segment
                            if (edgeSurface2)
                            {
                                cs2.GetEdgeCellNormal(cell2Id, ref cellNorm, ref edgeCellNorm);
                                Geometry.VpVxS(ref t2[2], t2[1], cellNorm, 0.001 * distance);
                            }
                            if (CheckTriangleToTriangleDistance(t1, t2, distance, angleRad)) return true;
                            // Cell 2 is a rectangle
                            if (cell2.Length == 4 || cell2.Length == 8)
                            {
                                // Cell 2 triangle 2
                                t2[0] = _nodes[cell2[0]];
                                t2[1] = _nodes[cell2[2]];
                                t2[2] = _nodes[cell2[3]];
                                //
                                if (CheckTriangleToTriangleDistance(t1, t2, distance, angleRad)) return true;
                            }
                            // Cell 1 is a rectangle
                            if (cell1.Length == 4 || cell1.Length == 8)
                            {
                                // Cell 1 triangle 2
                                t1[0] = _nodes[cell1[0]];
                                t1[1] = _nodes[cell1[2]];
                                t1[2] = _nodes[cell1[3]];
                                //
                                if (CheckTriangleToTriangleDistance(t1, t2, distance, angleRad)) return true;
                                // Cell 2 triangle 1 again
                                if (cell2.Length == 4 || cell2.Length == 8)
                                {
                                    t2[0] = _nodes[cell2[0]];
                                    t2[1] = _nodes[cell2[1]];
                                    t2[2] = _nodes[cell2[2]];
                                    //
                                    if (CheckTriangleToTriangleDistance(t1, t2, distance, angleRad)) return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool CheckTriangleToTriangleDistance(double[][] t1, double[][] t2, double distance, double angleRad)
        {
            double dist;
            double ang;
            //
            double[] a = new double[3];
            double[] b = new double[3];
            double[] n1 = new double[3];
            double[] n2 = new double[3];
            // Triangle 1
            Geometry.VmV(ref a, t1[1], t1[0]);
            Geometry.VmV(ref b, t1[2], t1[0]);
            Geometry.VcrossV(ref n1, a, b);
            Geometry.VxS(ref n1, n1, 1 / Math.Sqrt(Geometry.VdotV(n1, n1)));
            // Triangle 2
            Geometry.VmV(ref a, t2[1], t2[0]);
            Geometry.VmV(ref b, t2[2], t2[0]);
            Geometry.VcrossV(ref n2, a, b);
            Geometry.VxS(ref n2, n2, 1 / Math.Sqrt(Geometry.VdotV(n2, n2)));
            //
            // 180° - the normals point in the opposite directions
            ang = Math.PI - Math.Acos(Geometry.VdotV(n1, n2));
            if (ang < angleRad)
            {
                // Closest points on triangles t1 and t2 are p adn q, respectively
                double[] p = new double[3];
                double[] q = new double[3];
                double[] pq = new double[3];
                //
                dist = Geometry.TriDist(ref p, ref q, t1, t2);
                //
                if (dist < distance)
                {
                    // Check the orientraion of the triangle normals
                    if (dist > 0)
                    {
                        double ang1;
                        double ang2;
                        Geometry.VmV(ref pq, q, p);         // vector from p to q
                        Geometry.Vnorm(ref pq, pq);
                        ang1 = Geometry.VdotV(pq, n1);
                        if (ang1 < 0) return false;         // negative value means n1 poits away from q
                        ang2 = Geometry.VdotV(pq, n2);
                        if (ang2 > 0) return false;         // positive value means n2 poits away from p
                        // Check that the closest triangle points are one above the other
                        // The angle between the closest direction vector and normals must be small < 5° = 0.995
                        if (ang1 < 0.995 && ang2 > -0.995) return false;
                    }
                    return true;
                }
            }
            return false;
        }
        //
        private List<MasterSlaveItem> GroupContactPairsByNone(List<ContactSurface[]> contactSurfacePairs, bool checkUnresolved)
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
            masterSlaveItems = contactGraph.GetMasterSlaveItems(checkUnresolved);
            //
            return masterSlaveItems;
        }
        private List<MasterSlaveItem> GroupContactPairsByParts(List<ContactSurface[]> contactSurfacePairs, bool checkUnresolved)
        {
            int i;
            int j;
            int[] key;
            MasterSlaveItem masterSlaveItem;
            CompareIntArray comparer = new CompareIntArray();
            Dictionary<int[], MasterSlaveItem> partKeyMasterSlaveItems = new Dictionary<int[], MasterSlaveItem>(comparer);
            //
            Dictionary<int, int> partIds = GetPartIdsMergedByCompounds();
            //
            foreach (var csp in contactSurfacePairs)
            {
                if (partIds[csp[0].Part.PartId] < partIds[csp[1].Part.PartId])
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
                key = new int[] { partIds[csp[i].Part.PartId], partIds[csp[j].Part.PartId] };
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
            List<MasterSlaveItem> masterSlaveItems = contactGraph.GetMasterSlaveItems(checkUnresolved);
            //
            return masterSlaveItems;
          }
        private Dictionary<int,int> GetPartIdsMergedByCompounds()
        {
            int maxPartId = 0;
            Dictionary<int, int> partIds = new Dictionary<int, int>();
            foreach (var partEntry in _mesh.Parts)
            {
                partIds.Add(partEntry.Value.PartId, partEntry.Value.PartId);
                if (partEntry.Value.PartId > maxPartId) maxPartId = partEntry.Value.PartId;
            }
            //
            BasePart part;
            foreach (var geometryPartEntry in _geometry.Parts)
            {
                if (geometryPartEntry.Value is CompoundGeometryPart cgp)
                {
                    foreach (string subPartName in cgp.SubPartNames)
                    {
                        if (_mesh.Parts.TryGetValue(subPartName, out part)) partIds[part.PartId] = maxPartId;
                    }
                    maxPartId++;
                }
            }
            //
            return partIds;
        }


    }
}
