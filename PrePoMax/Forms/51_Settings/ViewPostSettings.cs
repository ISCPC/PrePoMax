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
    public class ViewPostSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private PostSettings _postSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Deformation                                                          
        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(0, 10, "Deformation scale factor")]
        [DescriptionAttribute("Select the deformation scale factor type.")]
        [Id(1, 1)]
        public DeformationScaleFactorType DeformationScaleFactorType
        {
            get { return _postSettings.DeformationScaleFactorType; }
            set
            {
                _postSettings.DeformationScaleFactorType = value;
                //
                if (value == DeformationScaleFactorType.Automatic)
                    _dctd.GetProperty(nameof(DeformationScaleFactorValue)).SetIsBrowsable(false);
                else if (value == DeformationScaleFactorType.TrueScale)
                    _dctd.GetProperty(nameof(DeformationScaleFactorValue)).SetIsBrowsable(false);
                else if (value == DeformationScaleFactorType.Off)
                    _dctd.GetProperty(nameof(DeformationScaleFactorValue)).SetIsBrowsable(false);
                else if (value == DeformationScaleFactorType.UserDefined)
                    _dctd.GetProperty(nameof(DeformationScaleFactorValue)).SetIsBrowsable(true);
                else throw new NotSupportedException();
            }
        }
        //
        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(1, 10, "Value")]
        [DescriptionAttribute("Select the deformation scale factor value.")]
        [Id(2, 1)]
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
        [Id(3, 1)]
        public bool DrawUndeformedModel
        {
            get { return _postSettings.DrawUndeformedModel; }
            set
            {
                _postSettings.DrawUndeformedModel = value;
                //
                _dctd.GetProperty(nameof(UndeformedModelColor)).SetIsBrowsable(_postSettings.DrawUndeformedModel);
            }
        }
        //
        [CategoryAttribute("Deformation")]
        [OrderedDisplayName(3, 10, "Undeformed model color")]
        [DescriptionAttribute("Set the color of the undeformed model.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(4, 1)]
        public System.Drawing.Color UndeformedModelColor
        {
            get { return _postSettings.UndeformedModelColor; } 
            set { _postSettings.UndeformedModelColor = value; }
        }
        // Limit values                                                         
        [CategoryAttribute("Limit values")]
        [OrderedDisplayName(0, 10, "Show max value location")]
        [DescriptionAttribute("Show max value location.")]
        [Id(1, 2)]
        public bool ShowMaxValueLocation
        {
            get { return _postSettings.ShowMaxValueLocation; }
            set { _postSettings.ShowMaxValueLocation = value; }
        }
        //
        [CategoryAttribute("Limit values")]
        [OrderedDisplayName(1, 10, "Show min value location")]
        [DescriptionAttribute("Show min value location.")]
        [Id(2, 2)]
        public bool ShowMinValueLocation
        {
            get { return _postSettings.ShowMinValueLocation; }
            set { _postSettings.ShowMinValueLocation = value; }
        }
        // History output
        [CategoryAttribute("History output")]
        [OrderedDisplayName(0, 10, "Max history output entries")]
        [DescriptionAttribute("Enter the maximum number of the history outputs entries to show.")]
        [Id(1, 3)]
        public int MaxHistoryEntriesToShow
        {
            get { return _postSettings.MaxHistoryEntriesToShow; }
            set { _postSettings.MaxHistoryEntriesToShow = value; }
        }


        // Constructors                                                                                                             
        public ViewPostSettings(PostSettings postSettings)
        {
            _postSettings = postSettings;
            _dctd = ProviderInstaller.Install(this);
            //
            DeformationScaleFactorType = _postSettings.DeformationScaleFactorType;  // add this also to Reset()
            DrawUndeformedModel = _postSettings.DrawUndeformedModel;                // add this also to Reset()
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowMinValueLocation));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowMaxValueLocation));
            _dctd.RenameBooleanPropertyToYesNo(nameof(DrawUndeformedModel));
        }


        // Methods                                                                                                                  
        public ISettings GetBase()
        {
            return _postSettings;
        }
        public void Reset()
        {
            _postSettings.Reset();
            //
            DeformationScaleFactorType = _postSettings.DeformationScaleFactorType;
            DrawUndeformedModel = _postSettings.DrawUndeformedModel;
        }
    }

}
