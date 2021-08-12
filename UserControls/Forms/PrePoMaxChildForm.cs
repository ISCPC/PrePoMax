using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserControls
{
    public class PrePoMaxChildForm : ChildMouseWheelManagedForm
    {
        // Variables                                                                                                                
        protected bool _propertyItemChanged;


        // Methods                                                                                                                  
        protected void CheckMissingValueRef(ref string[] allvalues, string currentValue, Action<string> valueSetter)
        {
            if (allvalues.Length == 0 || (allvalues.Length > 0 && !allvalues.Contains(currentValue)))
            {
                string value = "Missing";
                if (allvalues.Length > 0) value = allvalues[0];

                if (currentValue != null && currentValue != value)
                {
                    ShowProperyMissingBox(currentValue, value);
                    valueSetter(value);                    
                    _propertyItemChanged = true;
                }

                if (allvalues.Length == 0) allvalues = new string[] { value };
            }
        }
        protected void CheckMissingValue(string[] allvalues, string currentValue, Action<string> valueSetter)
        {
            if (allvalues.Length == 0 || (allvalues.Length > 0 && !allvalues.Contains(currentValue)))
            {
                string value = "Missing";
                if (allvalues.Length > 0) value = allvalues[0];

                if (currentValue != null && currentValue != value)
                {
                    ShowProperyMissingBox(currentValue, value);
                    valueSetter(value);
                    Array.Resize(ref allvalues, 1);
                    allvalues[0] = value;
                    _propertyItemChanged = true;
                }
            }
        }

        private void ShowProperyMissingBox(string missingValue, string newValue)
        {
            CaeGlobals.MessageBoxes.ShowError("The property value '" + missingValue + "' no longer exists." + Environment.NewLine
                                              + "The value was changed to '" + newValue + "'.");
        }        
    }
}
