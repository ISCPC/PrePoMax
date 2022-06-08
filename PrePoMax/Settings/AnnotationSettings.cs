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
    public class AnnotationSettings : ISettings
    {
        // Variables                                                                                                                
        private AnnotationNumberFormat _numberFormat;
        private int _numberOfSignificantDigits;
        private AnnotationBackgroundType _backgroundType;
        private bool _drawBorder;
        //
        private bool _showNodeId;
        private bool _showCoordinates;
        //
        private bool _showEdgeSurId;
        private bool _showEdgeSurSize;
        private bool _showEdgeSurMax;
        private bool _showEdgeSurMin;
        private bool _showEdgeSurSum;
        //
        private bool _showPartName;
        private bool _showPartId;
        private bool _showPartType;
        private bool _showPartNumberOfElements;
        private bool _showPartNumberOfNodes;


        // Properties                                                                                                               
        public AnnotationNumberFormat NumberFormat { get { return _numberFormat; } set { _numberFormat = value; } }
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
        public AnnotationBackgroundType BackgroundType
        {
            get { return _backgroundType; }
            set
            {
                if (value != _backgroundType)
                {
                    _backgroundType = value;
                    if (_backgroundType == AnnotationBackgroundType.White) _drawBorder = true;
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
        public bool ShowEdgeSurSum { get { return _showEdgeSurSum; } set { _showEdgeSurSum = value; } }
        //
        public bool ShowPartName { get { return _showPartName; } set { _showPartName = value; } }
        public bool ShowPartId { get { return _showPartId; } set { _showPartId = value; } }
        public bool ShowPartType { get { return _showPartType; } set { _showPartType = value; } }
        public bool ShowPartNumberOfElements
        {
            get { return _showPartNumberOfElements; }
            set { _showPartNumberOfElements = value; }
        }
        public bool ShowPartNumberOfNodes { get { return _showPartNumberOfNodes; } set { _showPartNumberOfNodes = value; } }


        // Constructors                                                                                                             
        public AnnotationSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _numberFormat = AnnotationNumberFormat.General;
            _numberOfSignificantDigits = 4;
            //
            _backgroundType = AnnotationBackgroundType.White;
            _drawBorder = true;
            //
            _showNodeId = true;
            _showCoordinates = true;
            //
            _showEdgeSurId = true;
            _showEdgeSurSize = true;
            _showEdgeSurMax = true;
            _showEdgeSurMin = true;
            _showEdgeSurSum = true;
            //
            _showPartName = true;
            _showPartId = true;
            _showPartType = true;
            _showPartNumberOfElements = true;
            _showPartNumberOfNodes = true;
        }
        public string GetNumberFormat()
        {
            string numberformat;
            if (_numberFormat == AnnotationNumberFormat.General)
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
