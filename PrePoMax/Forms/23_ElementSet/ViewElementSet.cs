using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewElementSet
    {
        // Variables                                                                                                                
        private FeElementSet _elementSet;
        private DynamicCustomTypeDescriptor _dctd = null;           // needed for sorting properties


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the element set.")]
        public string Name { get { return _elementSet.Name; } set { _elementSet.Name = value; } }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Number of elements")]
        [DescriptionAttribute("Number of elements in the element set.")]
        public int NumberOfElements { get { return  _elementSet.Labels == null ? 0 : _elementSet.Labels.Length; } }


        // Constructors                                                                                                             
        public ViewElementSet(FeElementSet elementSet)
        {
            _elementSet = elementSet;
            _dctd = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public FeElementSet GetBase()
        {
            return _elementSet;
        }
    }
}
