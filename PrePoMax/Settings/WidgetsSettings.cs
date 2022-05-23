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
        private bool _showEdgeId;
        private bool _showEdgeLength;
        private bool _showEdgeMax;
        private bool _showEdgeMin;
        private bool _showEdgeAvg;


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
        public bool ShowEdgeId { get { return _showEdgeId; } set { _showEdgeId = value; } }
        public bool ShowEdgeLength { get { return _showEdgeLength; } set { _showEdgeLength = value; } }
        public bool ShowEdgeMax { get { return _showEdgeMax; } set { _showEdgeMax = value; } }
        public bool ShowEdgeMin { get { return _showEdgeMin; } set { _showEdgeMin = value; } }
        public bool ShowEdgeAvg { get { return _showEdgeAvg; } set { _showEdgeAvg = value; } }


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
            _showEdgeId = true;
            _showEdgeLength = true;
            _showEdgeMax = true;
            _showEdgeMin = true;
            _showEdgeAvg = true;
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
