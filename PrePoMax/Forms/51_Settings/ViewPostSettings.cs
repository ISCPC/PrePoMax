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


        // Undeformed model                                                     
        [CategoryAttribute("Undeformed model")]
        [OrderedDisplayName(0, 10, "Draw undeformed model")]
        [DescriptionAttribute("Draw undeformed model.")]
        [Id(1, 1)]
        public UndeformedModelTypeEnum UndeformedModelType
        {
            get { return _postSettings.UndeformedModelType; }
            set
            {
                _postSettings.UndeformedModelType = value;
                //
                _dctd.GetProperty(nameof(UndeformedModelColor)).SetIsBrowsable(value != UndeformedModelTypeEnum.None);
            }
        }
        //
        [CategoryAttribute("Undeformed model")]
        [OrderedDisplayName(1, 10, "Undeformed model color")]
        [DescriptionAttribute("Set the color of the undeformed model.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(2, 1)]
        public System.Drawing.Color UndeformedModelColor
        {
            get { return _postSettings.UndeformedModelColor; } 
            set { _postSettings.UndeformedModelColor = value; }
        }
        // Limit values                                                         
        [CategoryAttribute("Limit values")]
        [OrderedDisplayName(0, 10, "Show min value and location")]
        [DescriptionAttribute("Show min value and location.")]
        [Id(1, 2)]
        public bool ShowMinValueLocation
        {
            get { return _postSettings.ShowMinValueLocation; }
            set { _postSettings.ShowMinValueLocation = value; }
        }
        //
        [CategoryAttribute("Limit values")]
        [OrderedDisplayName(1, 10, "Show max value and location")]
        [DescriptionAttribute("Show max value and location.")]
        [Id(2, 2)]
        public bool ShowMaxValueLocation
        {
            get { return _postSettings.ShowMaxValueLocation; }
            set { _postSettings.ShowMaxValueLocation = value; }
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
            UndeformedModelType = _postSettings.UndeformedModelType;                // add this also to Reset()
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowMinValueLocation));
            _dctd.RenameBooleanPropertyToYesNo(nameof(ShowMaxValueLocation));
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
            UndeformedModelType = _postSettings.UndeformedModelType;
        }
    }

}
