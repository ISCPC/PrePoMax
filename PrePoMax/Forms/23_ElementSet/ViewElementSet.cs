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
        private ItemSetData _itemSetData;
        private DynamicCustomTypeDescriptor _dctd = null;           // needed for sorting properties


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the element set.")]
        public string Name { get { return _elementSet.Name; } set { _elementSet.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Select items")]
        [DescriptionAttribute("Select the items for the element set.")]
        [EditorAttribute(typeof(ItemSetDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ItemSetData ItemSetData 
        {
            get { return _itemSetData; }
            set { if (value != _itemSetData) _itemSetData = value; }
        }


        // Constructors                                                                                                             
        public ViewElementSet(System.Windows.Forms.Form parentForm, FeElementSet elementSet)
        {
            _elementSet = elementSet;
            _dctd = ProviderInstaller.Install(this);
            _itemSetData = new ItemSetData(_elementSet.Labels);
            //
            _itemSetData.ItemIdsChangedEvent += ItemSetData_ItemIdsChangedEvent;
        }


        // Methods                                                                                                                  
        public FeElementSet GetBase()
        {
            return _elementSet;
        }
        private void ItemSetData_ItemIdsChangedEvent()
        {
            _elementSet.Labels = _itemSetData.ItemIds;
        }
    }
}
