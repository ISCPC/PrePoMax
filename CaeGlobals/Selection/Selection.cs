using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    [Serializable]
    public class Selection
    {
        // Variables                                                                                                                
        private List<SelectionNode> _nodes;
        [NonSerialized] private Dictionary<SelectionNode, int[]> _nodeIds;  // for speed optimization: keep current ids; do not copy
        private vtkSelectItem _selectItem;                                  // select node or element


        // Properties                                                                                                               
        public vtkSelectItem SelectItem { get { return _selectItem; } set { _selectItem = value; } }
        public List<SelectionNode> Nodes { get { return _nodes; } set { _nodes = value; } }

        // Constructors                                                                                                             
        public Selection()
        {
            _nodes = new List<SelectionNode>();
            _nodeIds = null;
            _selectItem = vtkSelectItem.None;
        }


        // Methods                                                                                                                  
        public void CopySelectonData(Selection selection)
        {
            _nodes.Clear();
            foreach (var selectionNode in selection.Nodes) _nodes.Add(selectionNode);
            _nodeIds = null;
            _selectItem = selection.SelectItem;
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
        public void RemoveLast()
        {
            if (_nodes != null && _nodes.Count > 0)
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
        }

        public bool IsGeometryBased()
        {
            foreach (var node in _nodes)
            {
                if (!((node is SelectionNodeIds sni && sni.GeometryIds) || 
                      (node is SelectionNodeMouse snm && snm.GeometryIds))) return false;
            }
            return true;
        }

       
    }
}
