using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vtkControl;

namespace PrePoMax
{
    [Serializable]
    public class Selection
    {
        // Variables                                                                                                                
        private vtkSelectItem _selectItem;      // select node or element
        private List<SelectionNode> _nodes;


        // Properties                                                                                                               
        public List<SelectionNode> Nodes { get { return _nodes; } set { _nodes = value; } }
        public vtkSelectItem SelectItem { get { return _selectItem; } set { _selectItem = value; } }


        // Constructors                                                                                                             
        public Selection()
        {
            _nodes = new List<SelectionNode>();
            _selectItem = vtkSelectItem.Node;
        }


        // Methods                                                                                                                  
        public void CopySelectonData(Selection selection)
        {
            _nodes.Clear();
            foreach (var selectionNode in selection.Nodes)
            {
                _nodes.Add(selectionNode);
            }
            _selectItem = selection.SelectItem;
        }
        public void Add(SelectionNode node)
        {
            _nodes.Add(node);
        }
        public void RemoveLast()
        {
            if (_nodes != null && _nodes.Count > 0) _nodes.RemoveAt(_nodes.Count - 1);
        }
        public void Clear()
        {
            _nodes.Clear();
        }

       
    }
}
