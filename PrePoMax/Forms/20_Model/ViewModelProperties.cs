using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeModel;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewModelProperties
    {
        // Variables                                                                                                                
        private string _name;
        private ModelProperties _modelProperties;
        private CustomPropertyDescriptor cpd = null;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [Category("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [Description("Name of the model.")]
        [Id(1, 1)]
        public string Name { get { return _name; } set { _name = value; } }
        //
        [Category("Data")]
        [OrderedDisplayName(1, 10, "Model type")]
        [Description("Model type.")]
        [Id(1, 1)]
        public ModelType ModelType 
        { 
            get { return _modelProperties.ModelType; } 
            set 
            { 
                _modelProperties.ModelType = value;
                //
                UpdateVisibility();
            }
        }
        //
        [Category("Submodel")]
        [OrderedDisplayName(0, 10, "Global results .frd")]
        [Description("Enter the global results file name (.frd) without path.")]
        [Id(1, 2)]
        public string GlobalResultsFileName
        {
            get { return _modelProperties.GlobalResultsFileName; }
            set 
            {
                
                if (value.Contains(" ") || value.Contains(":") || value.Contains("\\") || value.Contains("/") 
                    || value.ToUTF8() != value)
                    throw new Exception("Enter the global results file name (.frd) without path. " + 
                                        "The results file name must not contain any special characters.");
                _modelProperties.GlobalResultsFileName = value; 
            }
        }


        // Constructors                                                                                                             
        public ViewModelProperties(ModelProperties modelProperties)
        {
            _name = "Empty";
            _modelProperties = modelProperties;
            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
            //
            UpdateVisibility();
        }


        // Methods                                                                                                                  
        public ModelProperties GetBase()
        {
            return _modelProperties;
        }
        private void UpdateVisibility()
        {
            cpd = _dctd.GetProperty("GlobalResultsFileName");
            if (_modelProperties.ModelType == ModelType.Submodel) cpd.SetIsBrowsable(true);
            else cpd.SetIsBrowsable(false);
        }

       
       

    }
}
