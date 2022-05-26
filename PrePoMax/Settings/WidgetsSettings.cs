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
        //
        private bool _showNodeId;
        private bool _showCoordinates;
        //
        private bool _showEdgeSurId;
        private bool _showEdgeSurSize;
        private bool _showEdgeSurMax;
        private bool _showEdgeSurMin;
        private bool _showEdgeSurAvg;


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
        //
        public bool ShowNodeId { get { return _showNodeId; } set { _showNodeId = value; } }
        public bool ShowCoordinates { get { return _showCoordinates; } set { _showCoordinates = value; } }
        //
        public bool ShowEdgeSurId { get { return _showEdgeSurId; } set { _showEdgeSurId = value; } }
        public bool ShowEdgeSurSize { get { return _showEdgeSurSize; } set { _showEdgeSurSize = value; } }
        public bool ShowEdgeSurMax { get { return _showEdgeSurMax; } set { _showEdgeSurMax = value; } }
        public bool ShowEdgeSurMin { get { return _showEdgeSurMin; } set { _showEdgeSurMin = value; } }
        public bool ShowEdgeSurAvg { get { return _showEdgeSurAvg; } set { _showEdgeSurAvg = value; } }


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
            //
            _showEdgeSurId = true;
            _showEdgeSurSize = true;
            _showEdgeSurMax = true;
            _showEdgeSurMin = true;
            _showEdgeSurAvg = true;
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
