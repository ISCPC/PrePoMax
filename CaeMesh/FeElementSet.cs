using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class FeElementSet : FeGroup
    {
        // Variables                                                                                                                
        private Selection _creationData;
        private string[] _elementSetNames;


        // Properties                                                                                                               
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }


        // Constructors                                                                                                             
        public FeElementSet(string name, int[] labels) 
            : base(name, labels)
        {
            _creationData = null;
            _elementSetNames = null;
        }
        public FeElementSet(string name, string[] elementSetNames)
            : base(name, null)
        {
            _creationData = null;
            _elementSetNames = elementSetNames.ToArray();
        }

        public FeElementSet(FeElementSet elementSet)
           : base(elementSet.Name, elementSet.Labels)
        {
            _creationData = elementSet.CreationData.DeepClone();
            _elementSetNames = elementSet._elementSetNames.ToArray();
            //
            _active = elementSet.Active;
            _visible = elementSet.Visible;
            _valid = elementSet.Valid;
            _internal = elementSet.Internal;
        }


        // Methods                                                                                                                  
        public int[] GetMaxNRandomElementIds(int n)
        {
            if (Labels.Length < n) return Labels;
            else
            {
                List<int> notSelectedElements = new List<int>(Labels);
                int[] selectedElements = new int[n];
                Random rand = new Random();
                int id;
                for (int i = 0; i < n; i++)
                {
                    id = rand.Next(0, notSelectedElements.Count - 1);
                    selectedElements[i] = notSelectedElements[id];
                    notSelectedElements.RemoveAt(id);
                }
                return selectedElements.ToArray();
            }
        }
    }
}
