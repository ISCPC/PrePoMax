using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CaeGlobals
{
    [Serializable]
    public class SelectionNodeIds : SelectionNode, ISerializable
    {
        // Variables                                                                                                                
        private bool _selectAll;            //ISerializable
        private int[] _itemIds;             //ISerializable
        private bool _geometryIds;          //ISerializable


        // Properties                                                                                                               
        public bool SelectAll { get { return _selectAll; } set { _selectAll = value; } }
        public int[] ItemIds { get { return _itemIds; } set { _itemIds = value; } }
        public bool GeometryIds { get { return _geometryIds; } set { _geometryIds = value; } }


        // Constructors                                                                                                             
        public SelectionNodeIds(vtkSelectOperation selectOpreation, bool selectAll, int[] itemIds = null, bool geometryIds = false)
            : base(selectOpreation)
        {
            _selectAll = selectAll;
            if (itemIds != null) _itemIds = itemIds.ToArray(); // copy
            else _itemIds = null;
            _geometryIds = geometryIds;
        }
        public SelectionNodeIds(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_selectAll":
                        _selectAll = (bool)entry.Value; break;
                    case "_itemIds":
                        _itemIds = (int[])entry.Value; break;
                    case "_geometryIds":
                        _geometryIds = (bool)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public bool Equals(SelectionNodeIds selectionNode)
        {
            if (_selectAll == selectionNode.SelectAll)
            {
                if (_itemIds == null && selectionNode.ItemIds == null)
                    return true;

                if (_itemIds != null && selectionNode.ItemIds != null && _itemIds.Length == selectionNode.ItemIds.Length)
                {
                    bool equal = true;
                    for (int i = 0; i < _itemIds.Length; i++)
                    {
                        if (_itemIds[i] != selectionNode.ItemIds[i])
                        {
                            equal = false;
                            break;
                        }
                    }
                    if (equal) return true;
                }
            }
            return false;
        }

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_selectAll", _selectAll, typeof(bool));
            info.AddValue("_itemIds", _itemIds, typeof(int[]));
            info.AddValue("_geometryIds", _geometryIds, typeof(bool));
        }
    }
}
