using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public class Node<T> : IComparable<Node<T>> where T : IComparable<T>
    {
        // Variables                                                                                                                
        private T _data;
        private NodeList<T> _neighbours = null;


        // Propeties                                                                                                                  
        public T Value { get { return _data; } set { _data = value; } }
        public NodeList<T> Neighbours
        {
            get
            {
                if (_neighbours == null) _neighbours = new NodeList<T>();
                return _neighbours;
            }
            set { _neighbours = value; }
        }


        // Constructors                                                                                                             
        public Node()
        { }
        public Node(T data)
            : this(data, null)
        { }
        public Node(T data, NodeList<T> neighbours)
        {
            _data = data;
            _neighbours = neighbours;
        }
        public int CompareTo(Node<T> other)
        {
            int n1 = _neighbours == null ? 0 : _neighbours.Count;
            int n2 = other.Neighbours == null ? 0 : other.Neighbours.Count;
            //
            if (n1 < n2)
                return -1;
            else if (n1 > n2)
                return 1;
            else
                return _data.CompareTo(other._data);
        }
    }
}
