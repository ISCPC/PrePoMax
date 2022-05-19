using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;
using System.Runtime.InteropServices;

namespace PrePoMax
{
    [Serializable]
    public class WidgetsSettings : ISettings
    {
        // Variables                                                                                                                
        private WidgetNumberFormat _numberFormat;
        private int _numberOfSignificantDigits;
        private WidgetBackgroundType _backgroundType;
        private bool _drawBorder;
        private bool _showNodeId;
        private bool _showCoordinates;


        // Properties                                                                                                               
        public WidgetNumberFormat NumberFormat { get { return _numberFormat; } set { _numberFormat = value; } }
        public int NumberOfSignificantDigits
        {
            get { return _numberOfSignificantDigits; }
            set
            {
                _numberOfSignificantDigits = value;
                if (_numberOfSignificantDigits < 2) _numberOfSignificantDigits = 2;
                if (_numberOfSignificantDigits > 8) _numberOfSignificantDigits = 8;
            }
        }
        public WidgetBackgroundType BackgroundType
        {
            get { return _backgroundType; }
            set
            {
                if (value != _backgroundType)
                {
                    _backgroundType = value;
                    if (_backgroundType == WidgetBackgroundType.White) _drawBorder = true;
                }
            }
        }
        public bool DrawBorder { get { return _drawBorder; } set { _drawBorder = value; } }
        public bool ShowCoordinates { get { return _showCoordinates; } set { _showCoordinates = value; } }
        public bool ShowNodeId { get { return _showNodeId; } set { _showNodeId = value; } }


        // Constructors                                                                                                             
        public WidgetsSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _numberFormat = WidgetNumberFormat.General;
            _numberOfSignificantDigits = 4;
            //
            _backgroundType = WidgetBackgroundType.White;
            _drawBorder = true;
            //
            _showNodeId = true;
            _showCoordinates = true;
        }
        public string GetNumberFormat()
        {
            string numberformat;
            if (_numberFormat == WidgetNumberFormat.General)
            {
                numberformat = "G" + _numberOfSignificantDigits.ToString();
            }
            else
            {
                numberformat = "E" + (_numberOfSignificantDigits - 1).ToString();
            }
            return numberformat;
        }
    }
}
