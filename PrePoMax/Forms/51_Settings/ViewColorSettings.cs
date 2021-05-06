using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax.Settings
{
    [Serializable]
    public class ViewColorSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private ColorSettings _colorSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("General")]
        [OrderedDisplayName(0, 10, "Color 1")]
        [DescriptionAttribute("Select the first color.")]
        public Color Color1
        {
            get { return _colorSettings.ColorTable[0]; }
            set { _colorSettings.ColorTable[0] = value; }
        }
        //
        [CategoryAttribute("General")]
        [OrderedDisplayName(1, 10, "Color 2")]
        [DescriptionAttribute("Select the second color.")]
        public Color Color2
        {
            get { return _colorSettings.ColorTable[1]; }
            set { _colorSettings.ColorTable[1] = value; }
        }
        //
        [CategoryAttribute("General")]
        [OrderedDisplayName(2, 10, "Color 3")]
        [DescriptionAttribute("Select the third color.")]
        public Color Color3
        {
            get { return _colorSettings.ColorTable[2]; }
            set { _colorSettings.ColorTable[2] = value; }
        }
        //
        [CategoryAttribute("General")]
        [OrderedDisplayName(3, 10, "Color 4")]
        [DescriptionAttribute("Select the fourth color.")]
        public Color Color4
        {
            get { return _colorSettings.ColorTable[3]; }
            set { _colorSettings.ColorTable[3] = value; }
        }
        //
        [CategoryAttribute("General")]
        [OrderedDisplayName(4, 10, "Color 5")]
        [DescriptionAttribute("Select the fifth color.")]
        public Color Color5
        {
            get { return _colorSettings.ColorTable[4]; }
            set { _colorSettings.ColorTable[4] = value; }
        }
        //
        [CategoryAttribute("General")]
        [OrderedDisplayName(5, 10, "Color 6")]
        [DescriptionAttribute("Select the sixth color.")]
        public Color Color6
        {
            get { return _colorSettings.ColorTable[5]; }
            set { _colorSettings.ColorTable[5] = value; }
        }
        //
        [CategoryAttribute("General")]
        [OrderedDisplayName(6, 10, "Color 7")]
        [DescriptionAttribute("Select the seventh color.")]
        public Color Color7
        {
            get { return _colorSettings.ColorTable[6]; }
            set { _colorSettings.ColorTable[6] = value; }
        }
        //
        [CategoryAttribute("General")]
        [OrderedDisplayName(7, 10, "Color 8")]
        [DescriptionAttribute("Select the eight color.")]
        public Color Color8
        {
            get { return _colorSettings.ColorTable[7]; }
            set { _colorSettings.ColorTable[7] = value; }
        }
        //
        [CategoryAttribute("General")]
        [OrderedDisplayName(8, 10, "Color 9")]
        [DescriptionAttribute("Select the ninth color.")]
        public Color Color9
        {
            get { return _colorSettings.ColorTable[8]; }
            set { _colorSettings.ColorTable[8] = value; }
        }
        //
        [CategoryAttribute("General")]
        [OrderedDisplayName(9, 10, "Color 10")]
        [DescriptionAttribute("Select the tenth color.")]
        public Color Color10
        {
            get { return _colorSettings.ColorTable[9]; }
            set { _colorSettings.ColorTable[9] = value; }
        }
        //
        [CategoryAttribute("Face orientation")]
        [OrderedDisplayName(0, 10, "Front face color")]
        [DescriptionAttribute("Select the front shell face color.")]
        public Color FrontFaceColor
        {
            get { return _colorSettings.FrontFaceColor; }
            set { _colorSettings.FrontFaceColor = value; }
        }
        //
        [CategoryAttribute("Face orientation")]
        [OrderedDisplayName(1, 10, "Back face color")]
        [DescriptionAttribute("Select the back shell face color.")]
        public Color BackFaceColor
        {
            get { return _colorSettings.BackFaceColor; }
            set { _colorSettings.BackFaceColor = value; }
        }


        // Constructors                                                                                                             
        public ViewColorSettings(ColorSettings colorSettings)
        {
            _colorSettings = colorSettings;
            _dctd = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public ISettings GetBase()
        {
            return _colorSettings;
        }

        public void Reset()
        {
            _colorSettings.Reset();
        }
    }

}
