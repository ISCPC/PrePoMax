using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
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
}
