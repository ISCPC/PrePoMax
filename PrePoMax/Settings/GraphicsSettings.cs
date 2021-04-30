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
    public enum BackgroundType
    {
        [StandardValue("White", Description = "White color background.")]
        White,

        [StandardValue("Solid", Description = "One color background.")]
        Solid,

        [StandardValue("Gradient", DisplayName = "Gradient", Description = "Two color background.")]
        Gradient,
    }

    [Serializable]
    public class GraphicsSettings : ISettings
    {
        // Variables                                                                                                                
        private BackgroundType _backgroundType;
        private Color _topColor;
        private Color _bottomColor;
        private bool _coorSysVisibility;
        private bool _scaleWidgetVisibility;
        private double _ambientComponent;
        private double _diffuseComponent;
        private bool _pointSmoothing;
        private bool _lineSmoothing;
        private double _geometryDeflection;


        // Properties                                                                                                               
        public BackgroundType BackgroundType
        {
            get
            {
                return _backgroundType;
            }
            set
            {
                if (value != _backgroundType)
                {
                    _backgroundType = value;
                    if (_backgroundType == BackgroundType.White) _bottomColor = Color.White;
                    else if (_backgroundType == BackgroundType.Solid) _bottomColor = Color.WhiteSmoke;
                    else if (_backgroundType == BackgroundType.Gradient)
                    {
                        _topColor = Color.Gainsboro;
                        _bottomColor = Color.WhiteSmoke;
                    }
                }
            }
        }
        public Color TopColor { get { return _topColor; } set { _topColor = value; } }
        public Color BottomColor { get { return _bottomColor; } set { _bottomColor = value; } }
        public bool CoorSysVisibility { get { return _coorSysVisibility; } set { _coorSysVisibility = value; } }
        public bool ScaleWidgetVisibility { get { return _scaleWidgetVisibility; } set { _scaleWidgetVisibility = value; } }
        public double AmbientComponent
        {
            get { return _ambientComponent; }
            set
            {
                _ambientComponent = value;
                if (_ambientComponent < 0) _ambientComponent = 0;
                else if (_ambientComponent > 1) _ambientComponent = 1;
            }
        }
        public double DiffuseComponent
        {
            get { return _diffuseComponent; }
            set
            {
                _diffuseComponent = value;
                if (_diffuseComponent < 0) _diffuseComponent = 0;
                else if (_diffuseComponent > 1) _diffuseComponent = 1;
            }
        }
        public bool PointSmoothing { get { return _pointSmoothing; } set { _pointSmoothing = value; } }
        public bool LineSmoothing { get { return _lineSmoothing; } set { _lineSmoothing = value; } }
        public double GeometryDeflection
        {
            get { return _geometryDeflection; }
            set
            {
                _geometryDeflection = value;
                if (_geometryDeflection < 0) _geometryDeflection = 0;
                else if (_geometryDeflection > 0.1) _geometryDeflection = 0.1;
            }
        }


        // Constructors                                                                                                             
        public GraphicsSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _backgroundType = BackgroundType.Gradient;
            _topColor = Color.Gainsboro;
            _bottomColor = Color.WhiteSmoke;
            _coorSysVisibility = true;
            _scaleWidgetVisibility = true;
            _ambientComponent = 0.4;
            _diffuseComponent = 0.6;
            _pointSmoothing = true;
            _lineSmoothing = true;
            _geometryDeflection = 0.01;
        }
    }
}
