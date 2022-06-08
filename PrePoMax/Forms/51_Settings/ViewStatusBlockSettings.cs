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
    public class ViewStatusBlockSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private StatusBlockSettings _statusBlockSettings;
        private DynamicCustomTypeDescriptor _dctd = null;
       

        // Properties                                                                                                               
        [CategoryAttribute("Design")]
        [OrderedDisplayName(0, 10, "Background type")]
        [DescriptionAttribute("Select the background type.")]
        public AnnotationBackgroundType BackgroundType
        {
            get { return _statusBlockSettings.BackgroundType; }
            set { _statusBlockSettings.BackgroundType = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(1, 10, "Draw a border rectangle")]
        [DescriptionAttribute("Draw a border rectangle around the status block.")]
        public bool DrawBorder
        {
            get { return _statusBlockSettings.DrawBorder; }
            set { _statusBlockSettings.DrawBorder = value; }
        }


        // Constructors                                                                               
        public ViewStatusBlockSettings(StatusBlockSettings statusBlockSettings)
        {
            _statusBlockSettings = statusBlockSettings;
            _dctd = ProviderInstaller.Install(this);
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(DrawBorder));
        }


        // Methods                                                                               
        public ISettings GetBase()
        {
            return _statusBlockSettings;
        }
        public void Reset()
        {
            _statusBlockSettings.Reset();
        }
    }

}
