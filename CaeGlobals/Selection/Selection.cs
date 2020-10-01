using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CaeGlobals
{
    [Serializable]
    public class Selection : ISerializable
    {
        // Variables                                                                                                                
        private List<SelectionNode> _nodes;                 //ISerializable
        private vtkSelectItem _selectItem;                  //ISerializable
        private int _currentView;                           //ISerializable
        private int _maxNumberOfIds;                        //ISerializable

        // Temporary storage for speed optimization: keep current ids; do not copy
        [NonSerialized] private Dictionary<SelectionNode, int[]> _nodeIds;
        [NonSerialized] private bool _limitSelectionToFirstPart;


        // Properties                                                                                                               
        public vtkSelectItem SelectItem { get { return _selectItem; } set { _selectItem = value; } }
        public List<SelectionNode> Nodes { get { return _nodes; } set { _nodes = value; } }
        public bool LimitSelectionToFirstPart 
        {
            get { return _limitSelectionToFirstPart; }
            set { _limitSelectionToFirstPart = value; } 
        }
        public int CurrentView 
        { 
            get { return _currentView; } 
            set { _currentView = value; } 
        }
        public int MaxNumberOfIds { get { return _maxNumberOfIds; } set { _maxNumberOfIds = value; } }


        // Constructors                                                                                                             
        public Selection()
        {
            _nodes = new List<SelectionNode>();
            _nodeIds = null;
            _selectItem = vtkSelectItem.None;
            _limitSelectionToFirstPart = false;
            _currentView = -1;
            _maxNumberOfIds = -1;
        }
        public Selection(SerializationInfo info, StreamingContext context)
        {
            _currentView = -1;              // Compatibility for version v0.5.2
            _maxNumberOfIds = -1;           // Compatibility for version v0.8.0
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_nodes":
                        _nodes = (List<SelectionNode>)entry.Value; 
                        break;
                    case "_selectItem":
                        _selectItem = (vtkSelectItem)entry.Value; break;
                    case "_currentView":
                        _currentView = (int)entry.Value; break;
                    case "_maxNumberOfIds":
                        _maxNumberOfIds = (int)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public void CopySelectonData(Selection selection)
        {
            _nodes.Clear();
            foreach (var selectionNode in selection.Nodes) _nodes.Add(selectionNode);
            _nodeIds = null;
            _selectItem = selection._selectItem;
            _currentView = selection._currentView;
        }
        public void Add(SelectionNode node, int[] ids)
        {
            _nodes.Add(node);
            if (_nodeIds == null) _nodeIds = new Dictionary<SelectionNode, int[]>();
            _nodeIds.Add(node, ids);
        }
        public void Add(SelectionNodeIds node)
        {
            Add(node, node.ItemIds);
        }
        public bool TryGetNodeIds(SelectionNode node, out int[] ids)
        {
            ids = null;
            return _nodeIds == null ? false : _nodeIds.TryGetValue(node, out ids);
        }
        public void RemoveFirst()
        {
            if (_nodes.Count > 0)
            {
                SelectionNode node = _nodes.First();
                if (_nodeIds != null) _nodeIds.Remove(node);
                _nodes.Remove(node);
            }
        }
        public void RemoveLast()
        {
            if (_nodes.Count > 0)
            {
                SelectionNode node = _nodes.Last();
                if (_nodeIds != null) _nodeIds.Remove(node);
                _nodes.Remove(node);
            }
        }
        public void Clear()
        {
            _nodes.Clear();
            _nodeIds = null;
            //_selectItem = vtkSelectItem.None; - must not be used!!!
            //_currentView = -1;                - must not be used!!!
        }
        //
        public bool IsGeometryBased()
        {
            foreach (var node in _nodes)
            {
                if (!((node is SelectionNodeIds sni && sni.GeometryIds) || 
                      (node is SelectionNodeMouse snm && snm.IsGeometryBased))) return false;
            }
            return true;
        }
        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            info.AddValue("_nodes", _nodes, typeof(List<SelectionNode>));
            info.AddValue("_selectItem", _selectItem, typeof(vtkSelectItem));
            info.AddValue("_currentView", _currentView, typeof(int));
            info.AddValue("_maxNumberOfIds", _maxNumberOfIds, typeof(int));
        }

    }
}
