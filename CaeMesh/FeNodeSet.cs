using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class FeNodeSet : FeGroup
    {
        // Variables                                                                                                                
        private Selection _creationData;
        private int[] _creationIds;
        private double[] _centerOfGravity;
        private double[][] _boundingBox;

        // Properties                                                                                                               
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public double[] CenterOfGravity { get { return _centerOfGravity; } set { _centerOfGravity = value; } }
        public double[][] BoundingBox { get { return _boundingBox; } set { _boundingBox = value; } }


        // Constructors                                                                                                             
        public FeNodeSet(string name, int[] labels) 
            : base(name, labels)
        {
            _creationData = null;
            _creationIds = null;
            _centerOfGravity = null;
            _boundingBox = null;
        }
        public FeNodeSet(string name, int[] labels, double[] centerOfGravity)
            : base(name, labels)
        {
            _creationData = null;
            _creationIds = null;
            _centerOfGravity = centerOfGravity;
            _boundingBox = null;
        }


        // Methods                                                                                                                  
        public int[] GetMaxNRandomNodeIds(int n)
        {
            if (Labels.Length < n) return Labels;
            else
            {
                List<int> notSelectedNodes = new List<int>(Labels);
                int[] selectedNodes = new int[n];
                Random rand = new Random();
                int id;
                for (int i = 0; i < n; i++)
                {
                    id = rand.Next(0, notSelectedNodes.Count - 1);
                    selectedNodes[i] = notSelectedNodes[id];
                    notSelectedNodes.RemoveAt(id);
                }
                return selectedNodes.ToArray();
            }
        }
    }
}
