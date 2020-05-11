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
    public class ViewGeneralSettings : ViewSettings, IReset
    {
        // Variables                                                                                                                
        private GeneralSettings _generalSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("General")]
        [OrderedDisplayName(0, 10, "Open last file")]
        [DescriptionAttribute("When the program starts open the last file Saved/Opened.")]
        public bool OpenLastFile { get { return _generalSettings.OpenLastFile; } set { _generalSettings.OpenLastFile = value; } }

        [CategoryAttribute("General")]
        [OrderedDisplayName(1, 10, "Last file name")]
        [DescriptionAttribute("The name of the last file Saved/Opened.")]
        [ReadOnly(true)]
        public string LastFileName { get { return _generalSettings.LastFileName; } set { _generalSettings.LastFileName = value; } }

        [CategoryAttribute("General")]
        [OrderedDisplayName(2, 10, "Save results in .pmx files")]
        [DescriptionAttribute("Save the results in the PrePoMax .pmx file.")]
        public bool SaveResultsInPmx { get { return _generalSettings.SaveResultsInPmx; } set { _generalSettings.SaveResultsInPmx = value; } }


        // Constructors                                                                                                             
        public ViewGeneralSettings(GeneralSettings generalSettings)
        {
            _generalSettings = generalSettings;
            _dctd = ProviderInstaller.Install(this);
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(OpenLastFile));
            _dctd.RenameBooleanPropertyToYesNo(nameof(SaveResultsInPmx));
        }

        // Methods                                                                                                                  
        public override ISettings GetBase()
        {
            return _generalSettings;
        }
        public void Reset()
        {
            _generalSettings.Reset();
        }
    }

}
