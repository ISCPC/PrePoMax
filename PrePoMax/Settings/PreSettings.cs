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
        private Color _primaryHighlightColor;
        private Color _secundaryHighlightColor;
        private Color _mouseHighlightColor;
        private Color _constraintSymbolColor;
        private Color _boundaryConditionSymbolColor;
        private Color _loadSymbolColor;
        private Color _frontFaceColor;
        private Color _backFaceColor;
        private int _symbolSize;
        private int _nodeSymbolSize;
        private bool _drawSymbolEdges;


        // Properties                                                                                                               
        public Color PrimaryHighlightColor
        {
            get { return _primaryHighlightColor; }
            set { _primaryHighlightColor = value; }
        }
        public Color SecundaryHighlightColor
        {
            get { return _secundaryHighlightColor; }
            set { _secundaryHighlightColor = value; }
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
        public int SymbolSize
        {
            get { return _symbolSize; }
            set 
            {
                _symbolSize = value;
                if (_symbolSize < 1) _symbolSize = 1;
            }
        }
        public int NodeSymbolSize
        {
            get { return _nodeSymbolSize; }
            set
            {
                _nodeSymbolSize = value;
                if (_nodeSymbolSize < 1) _nodeSymbolSize = 1;
            }
        }
        public bool DrawSymbolEdges
        {
            get { return _drawSymbolEdges; }
            set { _drawSymbolEdges = value; }
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
            _primaryHighlightColor = Color.Red;
            _secundaryHighlightColor = Color.Violet;
            _mouseHighlightColor = Color.Orange;
            _constraintSymbolColor = Color.Yellow;
            _boundaryConditionSymbolColor = Color.Lime;
            _loadSymbolColor = Color.RoyalBlue;
            _frontFaceColor = Color.FromArgb(252, 169, 133);
            _backFaceColor = Color.FromArgb(154, 206, 223);
            _symbolSize = 50;
            _nodeSymbolSize = 3;
            _drawSymbolEdges = true;
        }
      
    }
}
