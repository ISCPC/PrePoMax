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
        [Id(2, 1)]
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
        [Description("Global results file name (.frd) without path.")]
        [EditorAttribute(typeof(FrdFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
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
        //
        [Category("Physical constants")]
        [OrderedDisplayName(0, 10, "Absolute zero")]
        [Description("Value of the absolute zero temperature.")]
        [TypeConverter(typeof(StringTemperatureUndefinedConverter))]
        [Id(1, 3)]
        public double AbsoluteZero { get { return _modelProperties.AbsoluteZero; } set { _modelProperties.AbsoluteZero = value; } }
        //
        [Category("Physical constants")]
        [OrderedDisplayName(1, 10, "Stefan-Boltzmann constant")]
        [Description("Value of the Stefan-Boltzmann constant.")]
        [TypeConverter(typeof(StringStefanBoltzmannUndefinedConverter))]
        [Id(2, 3)]
        public double StefanBoltzmann
        {
            get { return _modelProperties.StefanBoltzmann; }
            set { _modelProperties.StefanBoltzmann = value; }
        }
        //
        [Category("Physical constants")]
        [OrderedDisplayName(2, 10, "Gravitational constant")]
        [Description("Value of the Newton gravitational constant.")]
        [TypeConverter(typeof(StringNewtonGravityUndefinedConverter))]
        [Id(3, 3)]
        public double NewtonGravity
        {
            get { return _modelProperties.NewtonGravity; }
            set { _modelProperties.NewtonGravity = value; }
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
            cpd = _dctd.GetProperty(nameof(GlobalResultsFileName));
            if (_modelProperties.ModelType == ModelType.Submodel) cpd.SetIsBrowsable(true);
            else cpd.SetIsBrowsable(false);
        }

       
       

    }
}
