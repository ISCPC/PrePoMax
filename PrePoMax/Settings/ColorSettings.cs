using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax
{
    [Serializable]
    public class ColorSettings : ISettings, Settings.IReset
    {
        // Variables                                                                                                                
        private Color[] _colorTable;
        private Color _frontFaceColor;
        private Color _backFaceColor;

        // Properties                                                                                                               
        public Color[] ColorTable
        {
            get { return _colorTable; }
            set { _colorTable = value; }
        }
        public Color FrontFaceColor
        {
            get { return _frontFaceColor; }
            set { _frontFaceColor = value; }
        }
        public Color BackFaceColor
        {
            get { return _backFaceColor; }
            set { _backFaceColor = value; }
        }


        // Constructors                                                                                                             
        public ColorSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _colorTable = new Color[]
            {
                Color.Beige,
                Color.SeaGreen,
                Color.LightGray,
                Color.SteelBlue,
                Color.BurlyWood,
                Color.LightSteelBlue,
                Color.Gray,
                Color.DarkSalmon,
                Color.DarkKhaki,
                Color.NavajoWhite
            };
            //
            _frontFaceColor = Color.Coral;
            _backFaceColor = Color.PowderBlue;
        }

}
}
