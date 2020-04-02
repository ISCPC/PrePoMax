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
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the surface.")]
        public string Name { get { return _surface.Name; } set { _surface.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Surface type")]
        [DescriptionAttribute("Select the surface type.")]
        public FeSurfaceType SurfaceType { get { return _surface.Type; } set { _surface.Type = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Area")]
        [DescriptionAttribute("Area of the surface.")]
        public double Area { get { return _surface.Area; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(0, 10, "Region type")]
        [DescriptionAttribute("Select the region type for the creation ot the surface.")]
        public string CreateSurfaceFrom
        {
            get { return _surface.CreatedFrom.ToString(); }
            set
            {
                if (Enum.TryParse(value, out FeSurfaceCreatedFrom createdFrom))
                {
                    _surface.CreatedFrom = createdFrom;
                    SetPropertiesVisibility();
                }
            }
        }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the surface.")]
        public string NodeSetName { get { return _surface.CreatedFromNodeSetName; } set { _surface.CreatedFromNodeSetName = value; } }



        // Constructors                                                                                                             
        public ViewFeSurface(FeSurface surface)
        {
            _surface = surface;                                     // 1 command
            _dctd = ProviderInstaller.Install(this);                // 2 command
            SetPropertiesVisibility();                              // 3 command
        }


        // Methods                                                                                                                  

        public FeSurface GetBase()
        {
            return _surface;
        }
        public void PopululateDropDownList(string[] nodeSetNames)
        {
            StandardValueAttribute sva;
            CustomPropertyDescriptor cpd;
            // Selection
            cpd = _dctd.GetProperty(() => CreateSurfaceFrom);
            cpd.StatandardValues.Clear();
            // Always shoe selection
            cpd.StatandardValues.Add(new StandardValueAttribute(FeSurfaceCreatedFrom.Selection.ToString()));
            //
            if (nodeSetNames.Length > 0)
            {
                // NodeSet
                cpd.StatandardValues.Add(new StandardValueAttribute(FeSurfaceCreatedFrom.NodeSet.ToString()));
                // Add node set names
                cpd = _dctd.GetProperty(() => NodeSetName);
                cpd.StatandardValues.Clear();
                cpd.PropertyFlags |= PropertyFlags.ExclusiveStandardValues;
                //
                foreach (var name in nodeSetNames)
                {
                    sva = new StandardValueAttribute(name);
                    cpd.StatandardValues.Add(sva);
                }
            }
        }
        private void SetPropertiesVisibility()
        {
            if (_surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                _dctd.GetProperty(() => NodeSetName).SetIsBrowsable(false);
            }
            else if (_surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
            {
                _dctd.GetProperty(() => NodeSetName).SetIsBrowsable(true);
                //
                if (_surface.CreatedFromNodeSetName == null && _dctd.GetProperty(() => NodeSetName).StatandardValues.Count > 0)
                {
                    _surface.CreatedFromNodeSetName = _dctd.GetProperty(() => NodeSetName).StatandardValues.First().ToString();
                }
            }
        }






    }
}
