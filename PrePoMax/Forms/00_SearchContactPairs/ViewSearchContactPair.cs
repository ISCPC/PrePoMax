using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        // Properties                                                                                                               
        [DisplayName("Name")]
        public string Name { get { return _name; } }
        [DisplayName("Type")]
        public SearchContactPairType Type { get { return _type; } set { _type = value; } }
        [DisplayName("Adjust")]
        public SearchContactPairAdjust Adjust { get { return _adjust; } set { _adjust = value; } }


        // Constructors                                                                                                             
        public ViewSearchContactPair(string name, bool adjust)
        {
            _name = name;
            _type = SearchContactPairType.Tie;
            if (adjust) _adjust = SearchContactPairAdjust.Yes;
            else _adjust = SearchContactPairAdjust.No;
        }
    }
}
