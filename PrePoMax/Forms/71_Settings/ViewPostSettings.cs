using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Settings
{
    [Serializable]
    public class ViewPostSettings : ViewSettings, IReset
    {
        // Variables                                                                                                                
        private PostSettings _postSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Deformation                                                          
        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(0, 10, "Deformation scale factor")]
        [DescriptionAttribute("Select the deformation scale factor type.")]
        public DeformationScaleFactorType DeformationScaleFactorType
        {
            get { return _postSettings.DeformationScaleFactorType; }
            set
            {
                _postSettings.DeformationScaleFactorType = value;

                if (value == PrePoMax.DeformationScaleFactorType.Automatic)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("DeformationScaleFactorValue");
                    cpd.SetIsBrowsable(false);
                }
                else if (value == PrePoMax.DeformationScaleFactorType.TrueScale)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("DeformationScaleFactorValue");
                    cpd.SetIsBrowsable(false);
                }
                else if (value == PrePoMax.DeformationScaleFactorType.Off)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("DeformationScaleFactorValue");
                    cpd.SetIsBrowsable(false);
                }
                else if (value == PrePoMax.DeformationScaleFactorType.UserDefined)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty("DeformationScaleFactorValue");
                    cpd.SetIsBrowsable(true);
                }
                else throw new NotSupportedException();
            }
        }
        //
        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(1, 10, "Value")]
        [DescriptionAttribute("Select the deformation scale factor value.")]
        public double DeformationScaleFactorValue 
        { 
            get { return _postSettings.DeformationScaleFactorValue; } 
            set 
            {
                _postSettings.DeformationScaleFactorValue = value;
                if (_postSettings.DeformationScaleFactorValue < 0) _postSettings.DeformationScaleFactorValue = 0;
            } 
        }
        //
        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(2, 10, "Draw undeformed model")]
        [DescriptionAttribute("Draw undeformed model.")]
        public bool DrawUndeformedModel
        {
            get { return _postSettings.DrawUndeformedModel; }
            set
            {
                _postSettings.DrawUndeformedModel = value;

                CustomPropertyDescriptor cpd;
                cpd = _dctd.GetProperty("UndeformedModelColor");
                cpd.SetIsBrowsable(_postSettings.DrawUndeformedModel);
            }
        }
        //
        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(3, 10, "Undeformed model color")]
        [DescriptionAttribute("Set the color of the undeformed model.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        public System.Drawing.Color UndeformedModelColor
        {
            get { return _postSettings.UndeformedModelColor; } 
            set { _postSettings.UndeformedModelColor = value; }
        }
        // Limit values                                                         
        [CategoryAttribute("Limit values")]
        [OrderedDisplayName(0, 10, "Show max value location")]
        [DescriptionAttribute("Show max value location.")]
        public bool ShowMaxValueLocation
        {
            get { return _postSettings.ShowMaxValueLocation; }
            set { _postSettings.ShowMaxValueLocation = value; }
        }
        //
        [CategoryAttribute("Limit values")]
        [OrderedDisplayName(1, 10, "Show min value location")]
        [DescriptionAttribute("Show min value location.")]
        public bool ShowMinValueLocation
        {
            get { return _postSettings.ShowMinValueLocation; }
            set { _postSettings.ShowMinValueLocation = value; }
        }


        // Constructors                                                                                                             
        public ViewPostSettings(PostSettings postSettings)
        {
            _postSettings = postSettings;
            _dctd = ProviderInstaller.Install(this);
            //
            DeformationScaleFactorType = _postSettings.DeformationScaleFactorType;  // add this also to Reset()
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowMinValueLocation));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowMaxValueLocation));
            _dctd.RenameBooleanPropertyToYesNo(nameof(DrawUndeformedModel));
        }


        // Methods                                                                                                                  
        public override ISettings GetBase()
        {
            return _postSettings;
        }
        public void Reset()
        {
            _postSettings.Reset();
            //
            DeformationScaleFactorType = _postSettings.DeformationScaleFactorType;
        }
    }

}
