using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;


namespace PrePoMax.Forms
{
    [Serializable]
    public class MaterialLibraryItem : NamedClass
    {
        // Variables                                                                                                                
        private bool _expanded;
        private List<MaterialLibraryItem> _items;
        private CaeModel.Material _tag;


        // Properties                                                                                                               
        public bool Expanded { get { return _expanded; } set { _expanded = value; } }
        public List<MaterialLibraryItem> Items { get { return _items; } set { _items = value; } }
        public CaeModel.Material Tag { get { return _tag; } set { _tag = value; } }


        // Constructors                                                                                                             
        public MaterialLibraryItem()
        {
            _expanded = false;
            _items = new List<MaterialLibraryItem>();
            _tag = null;
        }
        public MaterialLibraryItem(string name)
            : base(name)
        {
            _expanded = false;
            _items = new List<MaterialLibraryItem>();
            _tag = null;
        }


        // Methods                                                                                                                  
    }
}
