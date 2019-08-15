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
    public class ViewFeSurface
    {
        // Variables                                                                                                                
        private FeSurface _surface;
        private ItemSetData _itemSetData;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the surface.")]
        public string Name { get { return _surface.Name; } set { _surface.Name = value; } }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Surface type")]
        [DescriptionAttribute("Select the surface type.")]
        public FeSurfaceType SurfaceType { get { return _surface.Type; } set { _surface.Type = value; } }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Create by/from")]
        [DescriptionAttribute("Select the way the surface will be created.")]
        public FeSurfaceCreatedFrom CreateSurfaceFrom
        {
            get { return _surface.CreatedFrom; }
            set
            {
                _surface.CreatedFrom = value;
                SetPropertiesVisibility();
            }
        }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Select items")]
        [DescriptionAttribute("Select the items for the set.")]
        [EditorAttribute(typeof(ItemSetDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ItemSetData ItemSetData
        {
            get { return _itemSetData; }
            set { if (value != _itemSetData) _itemSetData = value; }
        }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the surface creation.")]
        public string NodeSetName { get { return _surface.CreatedFromNodeSetName; } set { _surface.CreatedFromNodeSetName = value; } }

        //[CategoryAttribute("Data")]
        //[OrderedDisplayName(5, 10, "Geom. based")]
        //[DescriptionAttribute("Is surface selection based on geometry?")]
        //public bool GeomBased { get { return _surface.CreationData.IsGeometryBased(); } }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Area")]
        [DescriptionAttribute("Area of the surface.")]
        public double Area { get { return _surface.Area; } }


        // Constructors                                                                                                             
        public ViewFeSurface(FeSurface surface)
        {
            _surface = surface;                                     // 1 command
            _dctd = ProviderInstaller.Install(this);                // 2 command
            SetPropertiesVisibility();                              // 3 command
            _itemSetData = new ItemSetData(_surface.FaceIds);       // 4 command
        }


        // Methods                                                                                                                  
        
        public FeSurface GetBase()
        {
            _surface.FaceIds = _itemSetData.ItemIds;        // this must be here, since _itemSetData is changed as pointer
            return _surface;
        }
        public void PopululateDropDownList(string[] nodeSetNames)
        {
            StandardValueAttribute sva;
            CustomPropertyDescriptor cpd;

            if (nodeSetNames.Length > 0)
            {
                // NodeSetName
                cpd = _dctd.GetProperty("NodeSetName");
                cpd.StatandardValues.Clear();
                cpd.PropertyFlags |= PropertyFlags.ExclusiveStandardValues;

                foreach (var name in nodeSetNames)
                {
                    sva = new StandardValueAttribute(name);
                    cpd.StatandardValues.Add(sva);
                }
            }
            else
            {
                cpd = _dctd.GetProperty("CreateSurfaceFrom");
                cpd.SetIsBrowsable(false);
            }
        }
        private void SetPropertiesVisibility()
        {
            if (_surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                if (_itemSetData != null) _itemSetData.ItemIds = null;
                _dctd.GetProperty("ItemSetData").SetIsBrowsable(true);
                _dctd.GetProperty("NodeSetName").SetIsBrowsable(false);
            }
            else if (_surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
            {
                _dctd.GetProperty("ItemSetData").SetIsBrowsable(false);
                _dctd.GetProperty("NodeSetName").SetIsBrowsable(true);

                if (_surface.CreatedFromNodeSetName == null && _dctd.GetProperty("NodeSetName").StatandardValues.Count > 0)
                {
                    _surface.CreatedFromNodeSetName = _dctd.GetProperty("NodeSetName").StatandardValues.First().ToString();
                }
            }
        }




       

    }
}
