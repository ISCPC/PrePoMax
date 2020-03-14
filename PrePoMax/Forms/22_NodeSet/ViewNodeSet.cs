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
    public class ViewNodeSet
    {
        // Variables                                                                                                                
        private FeNodeSet _nodeSet;
        private ItemSetData _itemSetData;
        private DynamicCustomTypeDescriptor _dctd = null;           // needed for sorting properties


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [DisplayName("Name")]
        [DescriptionAttribute("Name of the node set.")]
        [Id(0, 0)]
        public string Name { get { return _nodeSet.Name; } set { _nodeSet.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [DisplayName("Select items")]
        [DescriptionAttribute("Select the items for the node set.")]
        [EditorAttribute(typeof(ItemSetDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Id(1, 0)]
        public ItemSetData ItemSetData 
        {
            get { return _itemSetData; }
            set { if (value != _itemSetData) _itemSetData = value; }
        }
        //
        [CategoryAttribute("Center of gravity")]
        [DisplayName("X")]
        [DescriptionAttribute("X center of gravity.")]
        [Id(0, 1)]
        public double XCenterOfGravity
        {
            get 
            {
                if (_nodeSet.CenterOfGravity != null && _nodeSet.CenterOfGravity.Length >= 3) return _nodeSet.CenterOfGravity[0];
                else return 0;
            }
        }
        //
        [CategoryAttribute("Center of gravity")]
        [DisplayName("Y")]
        [DescriptionAttribute("Y center of gravity.")]
        [Id(1, 1)]
        public double YCenterOfGravity
        {
            get
            {
                if (_nodeSet.CenterOfGravity != null && _nodeSet.CenterOfGravity.Length >= 3) return _nodeSet.CenterOfGravity[1];
                else return 0;
            }
        }
        //
        [CategoryAttribute("Center of gravity")]
        [DisplayName("Z")]
        [DescriptionAttribute("Z center of gravity.")]
        [Id(2, 1)]
        public double ZCenterOfGravity
        {
            get
            {
                if (_nodeSet.CenterOfGravity != null && _nodeSet.CenterOfGravity.Length >= 3) return _nodeSet.CenterOfGravity[2];
                else return 0;
            }
        }
        //
        [CategoryAttribute("Bounding box center")]
        [DisplayName("X")]
        [DescriptionAttribute("Bounding box X center of gravity.")]
        [Id(0, 2)]
        public double XBoundingBoxCG
        {
            get
            {
                if (_nodeSet.BoundingBox != null && _nodeSet.BoundingBox.Length >= 3) return (_nodeSet.BoundingBox[0][0] + _nodeSet.BoundingBox[0][1]) / 2;
                else return 0;
            }
        }
        //
        [CategoryAttribute("Bounding box center")]
        [DisplayName("Y")]
        [DescriptionAttribute("Bounding box Y center of gravity.")]
        [Id(1, 2)]
        public double YBoundingBoxCG
        {
            get
            {
                if (_nodeSet.BoundingBox != null && _nodeSet.BoundingBox.Length >= 3) return (_nodeSet.BoundingBox[1][0] + _nodeSet.BoundingBox[1][1]) / 2;
                else return 0;
            }
        }
        //
        [CategoryAttribute("Bounding box center")]
        [DisplayName("Z")]
        [DescriptionAttribute("Bounding box Z center of gravity.")]
        [Id(2, 2)]
        public double ZBoundingBoxCG
        {
            get
            {
                if (_nodeSet.BoundingBox != null && _nodeSet.BoundingBox.Length >= 3) return (_nodeSet.BoundingBox[2][0] + _nodeSet.BoundingBox[2][1]) / 2;
                else return 0;
            }
        }


        // Constructors                                                                                                             
        public ViewNodeSet(System.Windows.Forms.Form parentForm, FeNodeSet nodeSet)
        {
            _nodeSet = nodeSet;
            _dctd = ProviderInstaller.Install(this);
            _itemSetData = new ItemSetData(_nodeSet.Labels);
            //
            _itemSetData.ItemIdsChangedEvent += ItemSetData_ItemIdsChangedEvent;
        }


        // Methods                                                                                                                  
        public FeNodeSet GetBase()
        {
            return _nodeSet;
        }
        private void ItemSetData_ItemIdsChangedEvent()
        {
            _nodeSet.Labels = _itemSetData.ItemIds;
        }
    }
}
