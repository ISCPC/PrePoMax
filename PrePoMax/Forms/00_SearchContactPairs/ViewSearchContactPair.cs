using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using DynamicTypeDescriptor;


namespace PrePoMax.Forms
{
    [Serializable]
    public enum SearchContactPairType
    {
        Tie,
        Contact
    }
    [Serializable]
    public enum SearchContactPairAdjust
    {
        Yes,
        No
    }

    [Serializable]
    public class ViewSearchContactPair
    {
        // Variables                                                                                                                
        private string _name;
        private SearchContactPairType _type;
        private SearchContactPairAdjust _adjust;
        private double _adjustmentSize;
        private bool _multiView;
        private object _tag;
        private DynamicCustomTypeDescriptor _dctd = null;           // needed for sorting properties


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the contact pair.")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (CaeGlobals.NamedClass.CheckName(value)) _name = value;
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Type")]
        [DescriptionAttribute("Type of the contact pair.")]
        public SearchContactPairType Type { get { return _type; } set { _type = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Adjust")]
        [DescriptionAttribute("Set adjust to No to prevent the projection of the slave nodes on the master surface.")]
        public SearchContactPairAdjust Adjust { get { return _adjust; } set { _adjust = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Adjustment size")]
        [DescriptionAttribute("Set the adjustment size inside which the slave nodes will be projected.")]
        [TypeConverter(typeof(StringLengthDefaultConverter))]
        public double AdjustSize { get { return _adjustmentSize; } set { _adjustmentSize = value; } }
        //
        [Browsable(false)]
        public bool MultiView
        {
            get { return _multiView; }
            set
            {
                _multiView = value;
                _dctd.GetProperty(nameof(Name)).SetIsBrowsable(!_multiView);
            }
        }
        //
        [Browsable(false)]
        public object Tag { get { return _tag; } set { _tag = value; } }


        // Constructors                                                                                                             
        public ViewSearchContactPair(string name, bool adjust, double adjustmentSize)
        {
            _name = name;
            _type = SearchContactPairType.Tie;
            if (adjust) _adjust = SearchContactPairAdjust.Yes;
            else _adjust = SearchContactPairAdjust.No;
            _adjustmentSize = adjustmentSize;
            //
            _dctd = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public void SetProperty(string propertyName, ViewSearchContactPair source)
        {
            Type type = typeof(ViewSearchContactPair);
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var property in propertyInfos)
            {
                if (property.Name == propertyName)
                {
                    var o = property.GetValue(source);
                    var p = this.GetType().GetProperty(property.Name);
                    if (p != null)
                    {
                        Type t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                        object safeValue = (o == null) ? null : Convert.ChangeType(o, t);
                        p.SetValue(this, safeValue);
                    }
                }
            }
        }
    }
}
