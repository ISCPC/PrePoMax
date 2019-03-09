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
    public abstract class ViewStep
    {
        // Variables                                                                                                                
        protected DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the step.")]
        public abstract string Name { get; set; }

        [Browsable(false)]
        public abstract CaeModel.Step Base { get; set; }

        // Constructors                                                                                                             


        // Methods
        protected void RenameTrueFalseForBooleanToOnOff(string propertyName)
        {
            RenameTrueFalseForBooleanProperty(propertyName, "On", "Off");
        }
        protected void RenameTrueFalseForBooleanToYesNo(string propertyName)
        {
            RenameTrueFalseForBooleanProperty(propertyName, "Yes", "No");
        }
        protected void RenameTrueFalseForBooleanProperty(string propertyName, string trueName, string falseName)
        {
            CustomPropertyDescriptor cpd = _dctd.GetProperty(propertyName);

            foreach (StandardValueAttribute sva in cpd.StatandardValues)
            {
                if ((bool)sva.Value == true) sva.DisplayName = trueName;
                else sva.DisplayName = falseName;
            }
        }
        public abstract void UpdateFieldView();
    
     
    }
}
