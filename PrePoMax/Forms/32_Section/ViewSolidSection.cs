using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public class ViewSolidSection : ViewSection
    {
        // Variables                                                                                                                
        private CaeModel.SolidSection _solidSection;
        

        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Thickness")]
        [DescriptionAttribute("Set the thickness in the case of 2D plain strain/stress state.")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double Thickness { get { return _solidSection.Thickness; } set { _solidSection.Thickness = value; } }


        // Constructors                                                                                                             
        public ViewSolidSection(CaeModel.SolidSection solidSection)
        {
            _solidSection = solidSection;
            SetBase(_solidSection);
            //
            DynamicCustomTypeDescriptor.GetProperty(nameof(Thickness)).SetIsBrowsable(solidSection.TwoD);
        }


        // Methods                                                                                                                  


    }

}
