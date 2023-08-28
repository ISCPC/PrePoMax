using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace UserControls
{
    [Serializable]
    public class ViewSolidSection : ViewSection
    {
        // Variables                                                                                                                
        private CaeModel.SolidSection _solidSection;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Thickness")]
        [DescriptionAttribute("Set the thickness in the case of 2D plain strain/stress state.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        public string Thickness
        {
            get { return _solidSection.Thickness.Equation; }
            set { _solidSection.Thickness.Equation = value; }
        }

        //[CategoryAttribute("Data")]
        //[OrderedDisplayName(4, 10, "Type")]
        //[DescriptionAttribute("The type of the solid section.")]
        //public CaeModel.SolidSectionType Type
        //{
        //    get { return _solidSection.Type; }
        //}


        // Constructors                                                                                                             
        public ViewSolidSection(CaeModel.SolidSection solidSection)
        {
            _solidSection = solidSection;
            SetBase(solidSection);
        }


        // Methods



    }

}
