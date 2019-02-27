using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vtkControl;

namespace PrePoMax
{
    [Serializable]
    public class SelectionNodeIds : SelectionNode
    {
        // Variables                                                                                                                
        private bool _selectAll;
        private int[] _itemIds;


        // Properties                                                                                                               
        public bool SelectAll { get { return _selectAll; } set { _selectAll = value; } }
        public int[] ItemIds { get { return _itemIds; } set { _itemIds = value; } }


        // Constructors                                                                                                             
        public SelectionNodeIds(vtkSelectOperation selectOpreation, bool selectAll, int[] itemIds = null)
            : base(selectOpreation)
        {
            _selectAll = selectAll;
            if (itemIds != null) _itemIds = itemIds.ToArray(); // copy
            else _itemIds = null;
        }


        // Methods                                                                                                                  
    }
}
