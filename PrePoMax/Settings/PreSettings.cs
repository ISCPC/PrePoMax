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
    public class PreSettings : ISettings
    {
        // Variables                                                                                                                
        private Color _highlightColor;
        private Color _mouseHighlightColor;
        private Color _constraintSymbolColor;
        private Color _boundaryConditionSymbolColor;
        private Color _loadSymbolColor;
        private int _symbolSize;
        private int _nodeSymbolSize;
        private bool _drawSilhouettes;


        // Properties                                                                                                               
        public Color HighlightColor
        {
            get { return _highlightColor; }
            set { _highlightColor = value; }
        }
        public Color MouseHighlightColor
        {
            get { return _mouseHighlightColor; }
            set { _mouseHighlightColor = value; }
        }
        public Color ConstraintSymbolColor
        {
            get { return _constraintSymbolColor; }
            set { _constraintSymbolColor = value; }
        }
        public Color BoundaryConditionSymbolColor
        {
            get { return _boundaryConditionSymbolColor; }
            set { _boundaryConditionSymbolColor = value; }
        }
        public Color LoadSymbolColor
        {
            get { return _loadSymbolColor; }
            set { _loadSymbolColor = value; }
        }
        public int SymbolSize
        {
            get { return _symbolSize; }
            set { _symbolSize = value; }
        }
        public int NodeSymbolSize
        {
            get { return _nodeSymbolSize; }
            set { _nodeSymbolSize = value; }
        }
        public bool DrawSilhouettes
        {
            get { return _drawSilhouettes; }
            set { _drawSilhouettes = value; }
        }


        // Constructors                                                                                                             
        public PreSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _highlightColor = Color.Red;
            _mouseHighlightColor = Color.Orange;
            _constraintSymbolColor = Color.Yellow;
            _boundaryConditionSymbolColor = Color.Lime;
            _loadSymbolColor = Color.RoyalBlue;
            _symbolSize = 50;
            _nodeSymbolSize = 3;
            _drawSilhouettes = true;
        }
      
    }
}
