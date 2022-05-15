using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace vtkControl
{
    class vtkMaxScaleWidget : vtkMaxBorderWidget
    {
        // Variables                                                                                                                
        private double _width;
        private double _barWidth;
        private double _barWidthInModelUnits;
        private double _barHeight;
        private int _numOffields;
        private int _padding;
        private string _labelFormat;
        private string _unit;

        protected vtkTextMapper[] _scaleTextMappers;
        protected vtkActor2D[] _scaleTextActors;

        protected vtkPolyDataMapper2D _scaleBorderMapper;
        protected vtkActor2D _scaleBorderActor;

        protected vtkPolyDataMapper2D _scaleBackgroundMapper;
        protected vtkActor2D _scaleBackgroundActor;


        // Constructors                                                                                                             
        public vtkMaxScaleWidget()
        {
            _width = 300;
            //
            _barWidth = _width;
            _barHeight = 10;
            _numOffields = 5;
            _padding = 5;
            _labelFormat = "G4";
            _unit = "";
            //
            InitializeLabels();
            InitializeBackground();
            InitializeBorder();
        }


        // Private methods                                                                                                          
        private void InitializeLabels()
        {
            // Text property
            vtkTextProperty textProperty = vtkTextProperty.New();
            //
            _scaleTextMappers = new vtkTextMapper[_numOffields + 1];
            _scaleTextActors = new vtkActor2D[_scaleTextMappers.Length];
            //
            for (int i = 0; i < _scaleTextMappers.Length; i++)
            {
                // Mapper
                _scaleTextMappers[i] = vtkTextMapper.New();
                _scaleTextMappers[i].SetTextProperty(textProperty);
                // Actor
                _scaleTextActors[i] = vtkActor2D.New();
                _scaleTextActors[i].SetMapper(_scaleTextMappers[i]);
                // Set relative text position
                _scaleTextActors[i].GetPositionCoordinate().SetCoordinateSystemToDisplay();  // set offsets in pixels
                _scaleTextActors[i].GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
                _scaleTextActors[i].GetPositionCoordinate().SetValue(0, 0);
            }
        }
        private void InitializeBackground()
        {
            // Bar border                                                                          
            vtkPolyData backgroundPolyData = vtkPolyData.New();
            backgroundPolyData.SetPoints(vtkPoints.New());
            backgroundPolyData.SetPolys(vtkCellArray.New());
            //
            _scaleBackgroundMapper = vtkPolyDataMapper2D.New();
            _scaleBackgroundMapper.SetInput(backgroundPolyData);
            //
            vtkProperty2D backgroundProperty = vtkProperty2D.New();
            backgroundProperty.SetColor(0, 0, 0);
            //
            _scaleBackgroundActor = vtkActor2D.New();
            _scaleBackgroundActor.SetMapper(_scaleBackgroundMapper);
            _scaleBackgroundActor.SetProperty(backgroundProperty);
            //
            _scaleBackgroundActor.GetPositionCoordinate().SetCoordinateSystemToDisplay();  // set offsets in pixels
            _scaleBackgroundActor.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
        }
        private void InitializeBorder()
        {
            // Bar border                                                                          
            vtkPolyData borderPolyData = vtkPolyData.New();
            borderPolyData.SetPoints(vtkPoints.New());
            borderPolyData.SetPolys(vtkCellArray.New());
            //
            _scaleBorderMapper = vtkPolyDataMapper2D.New();
            _scaleBorderMapper.SetInput(borderPolyData);
            //
            vtkProperty2D borderProperty = vtkProperty2D.New();
            borderProperty.SetColor(0, 0, 0);
            //
            _scaleBorderActor = vtkActor2D.New();
            _scaleBorderActor.SetMapper(_scaleBorderMapper);
            _scaleBorderActor.SetProperty(borderProperty);
            //
            _scaleBorderActor.GetPositionCoordinate().SetCoordinateSystemToDisplay();  // set offsets in pixels
            _scaleBorderActor.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
        }
        
        private void GenerateGeometry()
        {
            UpdateSize();
            //
            GenerateLableValues();       // must be before Background
            GenerateBackground();       // must be before Borders
            GenerateBorders();
            UpdateOutterBorder();
            PositionLabels();
        }
        private void UpdateSize()
        {
            // Get world projection from display position
            _renderer.SetDisplayPoint(0, 0, 0);
            _renderer.DisplayToWorld();
            double[] first = _renderer.GetWorldPoint();
            //
            _renderer.SetDisplayPoint(0, _width, 0);
            _renderer.DisplayToWorld();
            double[] second = _renderer.GetWorldPoint();
            //
            double len = Math.Sqrt(Math.Pow(first[0] - second[0], 2)
                                   + Math.Pow(first[1] - second[1], 2)
                                   + Math.Pow(first[2] - second[2], 2));
            //
            if (!double.IsNaN(len) && len != 0)
            {
                //System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + "   Len: " + len);
                double prevWidth = _barWidth;
                double p = Math.Floor(Math.Log10(len));
                len /= Math.Pow(10, p);
                int numOfIntervals = 5;
                double ratio = Math.Round(numOfIntervals * len) / numOfIntervals / len;
                //
                _barWidth = _width * ratio;
                _barWidthInModelUnits = Math.Round(numOfIntervals * len) / numOfIntervals * Math.Pow(10, p);
                //
                _position[0] -= (_barWidth - prevWidth) / 2;
            }
        }
        private void GenerateLableValues()
        {
            double maxValue = _barWidthInModelUnits;
            double dx = maxValue / _numOffields;
            //
            for (int i = 0; i < _scaleTextMappers.Length; i++)
            {
                _scaleTextMappers[i].SetInput((i * dx).ToString(_labelFormat));
            }
        }
        private void GenerateBackground()
        {
            // Text size
            int[] sizeOfFirst = vtkMaxWidgetTools.GetTextSize(_scaleTextMappers[0], _renderer);
            // Number of vertical lines
            int n = _numOffields + 1;
            //
            vtkPoints backgroundPoints = vtkPoints.New();
            vtkCellArray backgroundPolygon = vtkCellArray.New();
            //
            backgroundPoints.SetNumberOfPoints(2 * n);
            //
            double dx = _barWidth / _numOffields;
            double dy = _barHeight;
            double x0 = sizeOfFirst[0] / 2 + _padding;
            double y0 = sizeOfFirst[1] + 2 * _padding;
            //
            for (int i = 0; i <= _numOffields; i++)
            {
                backgroundPoints.SetPoint(2 * i, x0 + i * dx, y0, 0);
                backgroundPoints.SetPoint(2 * i + 1, x0 + i * dx, y0 + dy, 0);
            }
            //
            for (int i = 0; i < _numOffields; i+=2)
            {
                backgroundPolygon.InsertNextCell(4);
                backgroundPolygon.InsertCellPoint(2 * i);
                backgroundPolygon.InsertCellPoint(2 * (i + 1));
                backgroundPolygon.InsertCellPoint(2 * (i + 1) + 1);
                backgroundPolygon.InsertCellPoint(2 * i + 1);
            }
            //
            vtkPolyData backgroundPoly = _scaleBackgroundMapper.GetInput();
            backgroundPoly.SetPoints(backgroundPoints);
            backgroundPoly.SetPolys(backgroundPolygon);
        }
        private void GenerateBorders()
        {
            vtkCellArray borderLines = vtkCellArray.New();
            // Vertical lines
            for (int i = 0; i <= _numOffields; i++)
            {
                borderLines.InsertNextCell(2);
                borderLines.InsertCellPoint(2 * i);
                borderLines.InsertCellPoint(2 * i + 1);
            }
            // Horizontal lines
            borderLines.InsertNextCell(2);
            borderLines.InsertCellPoint(0);
            borderLines.InsertCellPoint(2 * _numOffields);
            //
            borderLines.InsertNextCell(2);
            borderLines.InsertCellPoint(1);
            borderLines.InsertCellPoint(2 * _numOffields + 1);
            // PolyData
            vtkPolyData borderPoly = _scaleBorderMapper.GetInput();
            borderPoly.SetPoints(_scaleBackgroundMapper.GetInput().GetPoints());
            borderPoly.SetLines(borderLines);
        }
        private void PositionLabels()
        {
            vtkPoints points = _scaleBackgroundMapper.GetInput().GetPoints();
            //
            int[] size;
            double[] point;
            for (int i = 0; i < _scaleTextMappers.Length; i++)
            {
                point = points.GetPoint(2 * i);
                size = vtkMaxWidgetTools.GetTextSize(_scaleTextMappers[i], _renderer);
                //
                _scaleTextActors[i].GetPositionCoordinate().SetValue(point[0] - size[0] / 2, _padding);
            }
            //
            string lastLabel = _scaleTextMappers[_scaleTextMappers.Length - 1].GetInput();
            _scaleTextMappers[_scaleTextMappers.Length - 1].SetInput(lastLabel + " " + _unit);
        }
        private void UpdateOutterBorder()
        {
            // Text size
            int[] sizeOfFirst = vtkMaxWidgetTools.GetTextSize(_scaleTextMappers[0], _renderer);
            int[] sizeOfLast = vtkMaxWidgetTools.GetTextSize(_scaleTextMappers[_scaleTextMappers.Length - 1], _renderer);
            //
            double outterWidth = _padding + sizeOfFirst[0] / 2 + _barWidth + sizeOfLast[0] / 2 + _padding;
            double outterHeight = _padding + sizeOfLast[1] + _padding + _barHeight + _padding + sizeOfFirst[1] + _padding;
            //
            _size[0] = outterWidth;
            _size[1] = outterHeight;
        }
       

        // Public methods                                                                                                           
        public void SetUnit(string unit)
        {
            _unit = unit;
            GenerateGeometry();
        }
        public override void OnRenderWindowModified()
        {
            if (_visibility)
            {
                base.OnRenderWindowModified();
                OnSizeChanged();
            }
        }
        public override void CameraModified()
        {
            if (_visibility)
            {
                base.CameraModified();
                OnSizeChanged();
                //System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + "   CameraModified");
            }
        }
        public override void OnSizeChanged()
        {
            GenerateGeometry();
            OnMovedOrSizeChanged();
        }
        public override void VisibilityOn()
        {
            if (_visibility == false)
            {
                base.VisibilityOn();
                //
                if (_scaleBackgroundActor != null) _renderer.AddActor(_scaleBackgroundActor);
                if (_scaleBorderActor != null) _renderer.AddActor(_scaleBorderActor);
                //
                for (int i = 0; i < _scaleTextActors.Length; i++)
                {
                    if (_scaleTextActors[i] != null) _renderer.AddActor(_scaleTextActors[i]);
                }
            }
        }
        public override void VisibilityOff()
        {
            if (_visibility == true)
            {
                base.VisibilityOff();
                //
                if (_scaleBackgroundActor != null) _renderer.RemoveActor(_scaleBackgroundActor);
                if (_scaleBorderActor != null) _renderer.RemoveActor(_scaleBorderActor);
                //
                for (int i = 0; i < _scaleTextActors.Length; i++)
                {
                    if (_scaleTextActors[i] != null) _renderer.RemoveActor(_scaleTextActors[i]);
                }
            }
        }

        
        // Public setters                                                                                                           
        public override void SetInteractor(vtkRenderer renderer, vtkRenderWindowInteractor renderWindowInteractor)
        {
            base.SetInteractor(renderer, renderWindowInteractor);
            //
            _renderer.AddActor(_scaleBackgroundActor);
            _renderer.AddActor(_scaleBorderActor);
            for (int i = 0; i < _scaleTextActors.Length; i++)
            {
                _renderer.AddActor(_scaleTextActors[i]);
            }
            //
            OnSizeChanged();
        }
        public override void RemoveInteractor()
        {
            _renderer.RemoveActor(_scaleBackgroundActor);
            _renderer.RemoveActor(_scaleBorderActor);
            for (int i = 0; i < _scaleTextActors.Length; i++)
            {
                _renderer.RemoveActor(_scaleTextActors[i]);
            }
            //
            base.RemoveInteractor();
        }
        public void SetWidth(int width)
        {
            _width = width;
            OnSizeChanged();
        }
        public void SetLabelFormat(string labelFormat)
        {
            if (_labelFormat != labelFormat)
            {
                _labelFormat = labelFormat;
                OnSizeChanged();
            }
        }
        public void SetTextProperty(vtkTextProperty textProperty)
        {
            for (int i = 0; i < _scaleTextMappers.Length; i++)
            {
                _scaleTextMappers[i].SetTextProperty(textProperty);
            }
        }
        public void SetPadding(int padding)
        {
            if (padding != _padding)
            {
                _padding = padding;
                OnSizeChanged();
            }
        }


        // Public getters                                                                                                           
        public string GetLabelFormat()
        {
            return _labelFormat;
        }
    }
}
