using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using CaeGlobals;

namespace PrePoMax
{
    public class ItemSetData
    {
        // Variables                                                                                                                
        int[] _itemIds;


        // Properties                                                                                                               
        public int[] ItemIds 
        { 
            get { return _itemIds; } 
            set { if (value != _itemIds) _itemIds = value; } 
        }


        // Constructors                                                                                                             
        public ItemSetData()
        {
        }
        public ItemSetData(int[] itemIds)
        {
            _itemIds = itemIds;
        }


        // Methods                                                                                                                  
        public override string ToString()
        {
            if (_itemIds == null || _itemIds.Length == 0) return "Empty";
            else return "Number of items: " + _itemIds.Length;
        }
    }
}
