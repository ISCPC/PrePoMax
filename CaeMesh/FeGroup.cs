using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class FeGroup : NamedClass
    {
        // Variables                                                                                                                
        protected int[] _labels;

        // Properties                                                                                                               
        [Browsable(false)]
        public int[] Labels { get { return _labels; } set { _labels = value; } }

        [CategoryAttribute("Data"),
        DisplayName("Count"),
        DescriptionAttribute("Number of items.")]
        public int Count { get { return Labels.Length; } }


        // Constructors                                                                                                             
        public FeGroup(string name, int[] labels) 
            : base(name)
        {
            _labels = labels;
        }
        public FeGroup(FeGroup group)
            :base(group) // NamedClass
        {
            _labels = group.Labels.ToArray();
        }


        // Methods                                                                                                                  
    }
}
