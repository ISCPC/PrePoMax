using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
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
}
