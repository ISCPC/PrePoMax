using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;


namespace vtkControl
{
    class vtkMaxColorBarWidget : vtkMaxTextWidget
    {
        // Variables                                                                                                                
        protected vtkPolyDataMapper2D _colorBarBorderMapper;
        protected vtkActor2D _colorBarBorderActor;
        //
        protected vtkPolyDataMapper2D _colorBarColorsMapper;
        protected vtkActor2D _colorBarColorsActor;
        //
        protected vtkActor2D _textActorFooter;
        protected vtkTextMapper _textMapperFooter;
        //
        protected vtkActor2D _textActorLabel;
        protected vtkTextMapper _textMapperLabel;
        //
        private Color[] _colors;
        private string[] _labels;
        vtkLookupTable _lookupTable;


        // Properties                                                                                                               
        public Color[] Colors {get {return _colors;} }
        public string[] Labels { get { return _labels; } }


        // Constructors                                                                                                             
        public vtkMaxColorBarWidget()
        {
            _backgroundVisibility = false;
            _borderVisibility = false;
            //
            SetBorderColor(0, 0, 0);
            //
            InitializeFooter();
            InitializeLabels();
            InitializeBar();
            //
            SetColorsAndLabels(null, null);
            //
            //_colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Violet };
            //_labels = new string[] { "Red", "Green", "Blue", "Yellow", "Violet" };
            //SetColorsAndLabels(_colors, _labels);
        }


        // Private methods                                                                                                          
        private void InitializeFooter()
        {
            // Footer text                                                                              
            _textMapperFooter = vtkTextMapper.New();
            _textMapperFooter.SetTextProperty(_textMapper.GetTextProperty());
            // Actor
            _textActorFooter = vtkActor2D.New();
            _textActorFooter.SetMapper(_textMapperFooter);
            // Set relative text position
            _textActorFooter.GetPositionCoordinate().SetCoordinateSystemToDisplay();  // set offsets in pixels
            _textActorFooter.GetPositionCoordinate().SetValue(_padding, _padding);
            _textActorFooter.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
        }
        private void InitializeLabels()
        {
            // Bar label                                                                                
            _textMapperLabel = vtkTextMapper.New();
            _textMapperLabel.SetTextProperty(_textMapper.GetTextProperty());
            // Actor
            _textActorLabel = vtkActor2D.New();
            _textActorLabel.SetMapper(_textMapperLabel);
            // Set relative text position
            _textActorLabel.GetPositionCoordinate().SetCoordinateSystemToDisplay();  // set offsets in pixels
            _textActorLabel.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
        }
        private void InitializeBar()
        {
            // Bar border                                                                          
            vtkPolyData colorBarPolyData = vtkPolyData.New();
            colorBarPolyData.SetPoints(vtkPoints.New());
            colorBarPolyData.SetPolys(vtkCellArray.New());
            //
            _colorBarBorderMapper = vtkPolyDataMapper2D.New();
            _colorBarBorderMapper.SetInput(colorBarPolyData);
            //
            vtkProperty2D colorBarProperty = vtkProperty2D.New();
            colorBarProperty.SetColor(0, 0, 0);
            //
            _colorBarBorderActor = vtkActor2D.New();
            _colorBarBorderActor.SetMapper(_colorBarBorderMapper);
            _colorBarBorderActor.SetProperty(colorBarProperty);
            //
            _colorBarBorderActor.GetPositionCoordinate().SetCoordinateSystemToDisplay();
            _colorBarBorderActor.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
            // Bar filled                                                                          
            colorBarPolyData = vtkPolyData.New();
            colorBarPolyData.SetPoints(vtkPoints.New());
            colorBarPolyData.SetPolys(vtkCellArray.New());
            //
            _colorBarColorsMapper = vtkPolyDataMapper2D.New();
            _colorBarColorsMapper.SetInput(colorBarPolyData);
            //
            _colorBarColorsActor = vtkActor2D.New();
            _colorBarColorsActor.SetMapper(_colorBarColorsMapper);
            _colorBarColorsActor.SetProperty(colorBarProperty);
            //
            _colorBarColorsActor.GetPositionCoordinate().SetCoordinateSystemToDisplay();
            _colorBarColorsActor.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
        }
        //
        private void GenerateGeometry()
        {
            // Geometry
            double boxAspectRatio = 3.0 / 4.0;
            //
            string tmp = _textMapperLabel.GetInput();
            _textMapperLabel.SetInput("1");
            int[] sizeOfOne = vtkMaxWidgetTools.GetTextSize(_textMapperLabel, _renderer);
            _textMapperLabel.SetInput("1" + Environment.NewLine + "1");
            int[] sizeOfTwo = vtkMaxWidgetTools.GetTextSize(_textMapperLabel, _renderer);
            _textMapperLabel.SetInput(tmp);
            //
            double lineSpacing = ((double)sizeOfTwo[1] - 2 * sizeOfOne[1]);
            double boxHeight = sizeOfOne[1] + lineSpacing;
            double boxWidth = boxHeight / boxAspectRatio;
            double spacing = boxHeight * 0.5;
            double lineOffset = _textMapperLabel.GetTextProperty().GetLineOffset();
            // Footer
            double[] size1 = GenerateFooter(_padding, _padding);
            size1[1] += spacing;
            // Labels
            double[] size2 = GenerateLabels(_padding + boxWidth + spacing, size1[1]);
            size2[1] += spacing;
            // Bar lines
            double offsetY = lineOffset - 0.3 / 7 * sizeOfOne[1];
            GenerateBarBorders(_padding, offsetY + size1[1], boxWidth, boxHeight);
            GenerateBarColors(_padding, offsetY + size1[1], boxWidth, boxHeight);
            // Text
            double[] size3 = GenerateText(_padding, size2[1]);
            //
            double maxX = size1[0];
            if (size2[0] > maxX) maxX = size2[0];
            if (size3[0] > maxX) maxX = size3[0];
            //
            _size[0] = maxX + _padding;
            _size[1] = size3[1] + _padding;
        }
        private double[] GenerateFooter(double offsetX, double offsetY)
        {
            _textActorFooter.GetPositionCoordinate().SetValue(offsetX, offsetY);
            //
            int[] sizeFooter = vtkMaxWidgetTools.GetTextSize(_textMapperFooter, _renderer);
            return new double[] { offsetX + sizeFooter[0], offsetY + sizeFooter[1] };
        }
        private double[] GenerateLabels(double offsetX, double offsetY)
        {
            string label = "";
            for (int i = 0; i < _labels.Length; i++)
            {
                if (i != 0) label += Environment.NewLine;
                label += _labels[i];
            }
            _textMapperLabel.SetInput(label);
            //
            int[] size = vtkMaxWidgetTools.GetTextSize(_textMapperLabel, _renderer);
            _textActorLabel.GetPositionCoordinate().SetValue(offsetX, offsetY);
            //
            return new double[] { offsetX + size[0], offsetY + size[1] };
        }
        //
        private void _GenerateBarBorders(double offsetX, double offsetY, double boxWidth, double boxHeight, double verticalLineLength)
        {
            vtkPoints colorBarPoints = vtkPoints.New();
            vtkCellArray colorBarLines = vtkCellArray.New();
            //
            int numOfTableColors = _colors.Length;
            //
            colorBarPoints.SetNumberOfPoints(2 * (numOfTableColors + 1) + 2);
            colorBarLines.SetNumberOfCells((numOfTableColors + 1) + 2);
            //
            double h = 0;
            //
            for (int i = 0; i <= numOfTableColors; i++)
            {
                h = i * boxHeight;
                colorBarPoints.SetPoint(2 * i, 0, h, 0);
                colorBarPoints.SetPoint(2 * i + 1, verticalLineLength, h, 0);
                //
                colorBarLines.InsertNextCell(2);
                colorBarLines.InsertCellPoint(2 * i);
                colorBarLines.InsertCellPoint(2 * i + 1);
            }
            //
            colorBarPoints.SetPoint(2 * (numOfTableColors + 1), boxWidth, 0, 0);
            colorBarPoints.SetPoint(2 * (numOfTableColors + 1) + 1, boxWidth, h, 0);
            //
            colorBarLines.InsertNextCell(2);
            colorBarLines.InsertCellPoint(0);
            colorBarLines.InsertCellPoint(2 * numOfTableColors);
            //
            colorBarLines.InsertNextCell(2);
            colorBarLines.InsertCellPoint(2 * (numOfTableColors + 1));
            colorBarLines.InsertCellPoint(2 * (numOfTableColors + 1) + 1);
            //
            vtkPolyData colorBarPoly = _colorBarBorderMapper.GetInput();
            colorBarPoly.SetPoints(colorBarPoints);
            colorBarPoly.SetLines(colorBarLines);
            //
            _colorBarBorderActor.GetPositionCoordinate().SetValue(offsetX, offsetY);
        }
        private void GenerateBarBorders(double offsetX, double offsetY, double boxWidth, double boxHeight)
        {
            vtkPoints colorBarPoints = vtkPoints.New();
            vtkCellArray colorBarLines = vtkCellArray.New();
            //
            int numOfTableColors = _colors.Length;
            //
            colorBarPoints.SetNumberOfPoints(4 * numOfTableColors);
            colorBarLines.SetNumberOfCells(4 * numOfTableColors);
            //
            double h;
            //
            for (int i = 0; i < numOfTableColors; i++)
            {
                h = i * boxHeight;
                colorBarPoints.SetPoint(4 * i, 0, h + 2, 0);
                colorBarPoints.SetPoint(4 * i + 1, boxWidth, h + 2, 0);
                //
                h += boxHeight;
                colorBarPoints.SetPoint(4 * i + 2, 0, h - 2, 0);
                colorBarPoints.SetPoint(4 * i + 3, boxWidth, h - 2, 0);
                //
                colorBarLines.InsertNextCell(2);
                colorBarLines.InsertCellPoint(4 * i);
                colorBarLines.InsertCellPoint(4 * i + 1);
                //
                colorBarLines.InsertNextCell(2);
                colorBarLines.InsertCellPoint(4 * i + 2);
                colorBarLines.InsertCellPoint(4 * i + 3);
                //
                colorBarLines.InsertNextCell(2);
                colorBarLines.InsertCellPoint(4 * i);
                colorBarLines.InsertCellPoint(4 * i + 2);
                //
                colorBarLines.InsertNextCell(2);
                colorBarLines.InsertCellPoint(4 * i + 1);
                colorBarLines.InsertCellPoint(4 * i + 3);
            }
            //
            vtkPolyData colorBarPoly = _colorBarBorderMapper.GetInput();
            colorBarPoly.SetPoints(colorBarPoints);
            colorBarPoly.SetLines(colorBarLines);
            //
            _colorBarBorderActor.GetPositionCoordinate().SetValue(offsetX, offsetY);
        }
        private void GenerateBarColors(double offsetX, double offsetY, double boxWidth, double boxHeight)
        {
            vtkPoints colorBarPoints = vtkPoints.New();
            vtkCellArray colorBarPolygons = vtkCellArray.New();
            vtkIntArray colors = vtkIntArray.New();
            //
            int numOfTableColors = _colors.Length;
            //
            colorBarPoints.SetNumberOfPoints(4 * numOfTableColors);
            colorBarPolygons.SetNumberOfCells(numOfTableColors);
            colors.SetNumberOfValues(numOfTableColors);
            //
            double h;
            //
            for (int i = 0; i < numOfTableColors; i++)
            {
                h = i * boxHeight;
                colorBarPoints.SetPoint(4 * i + 0, 0, h + 2, 0);
                colorBarPoints.SetPoint(4 * i + 1, boxWidth, h + 2, 0);
                //
                h += boxHeight;
                colorBarPoints.SetPoint(4 * i + 2, boxWidth, h - 2, 0);
                colorBarPoints.SetPoint(4 * i + 3, 0, h - 2, 0);
                //
                colorBarPolygons.InsertNextCell(4);
                colorBarPolygons.InsertCellPoint(4 * i + 0);
                colorBarPolygons.InsertCellPoint(4 * i + 1);
                colorBarPolygons.InsertCellPoint(4 * i + 2);
                colorBarPolygons.InsertCellPoint(4 * i + 3);
                // The geometry is built from the bottom up
                colors.SetValue(i, numOfTableColors - 1 - i);
            }
            //
            vtkPolyData colorBarPoly = _colorBarColorsMapper.GetInput();
            colorBarPoly.SetPoints(colorBarPoints);
            colorBarPoly.SetPolys(colorBarPolygons);
            // Set colors
            _colorBarColorsMapper.GetInput().GetCellData().SetScalars(colors);
            //
            double[] range = _lookupTable.GetRange();
            _colorBarColorsMapper.SetScalarRange(range[0], range[1]);
            _colorBarColorsMapper.SetLookupTable(_lookupTable);
            //
            _colorBarColorsActor.GetPositionCoordinate().SetValue(offsetX, offsetY);
        }
        private double[] GenerateText(double offsetX, double offsetY)
        {
            _textActor.GetPositionCoordinate().SetValue(offsetX, offsetY);
            //
            int[] sizeText = vtkMaxWidgetTools.GetTextSize(_textMapper, _renderer);
            return new double[] { offsetX + sizeText[0], offsetY + sizeText[1] };
        }


        // Public methods                                                                                                           
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
                if (_textActorFooter != null) _renderer.AddActor(_textActorFooter);
                if (_textActorLabel != null) _renderer.AddActor(_textActorLabel);
                if (_colorBarColorsActor != null) _renderer.AddActor(_colorBarColorsActor);
                if (_colorBarBorderActor != null) _renderer.AddActor(_colorBarBorderActor);
            }
        }
        public override void VisibilityOff()
        {
            if (_visibility == true)
            {
                base.VisibilityOff();
                //
                if (_textActorFooter != null) _renderer.RemoveActor(_textActorFooter);
                if (_textActorLabel != null) _renderer.RemoveActor(_textActorLabel);
                if (_colorBarBorderActor != null) _renderer.RemoveActor(_colorBarBorderActor);
                if (_colorBarColorsActor != null) _renderer.RemoveActor(_colorBarColorsActor);
            }
        }


        // Public setters                                                                                                           
        public override void SetInteractor(vtkRenderer renderer, vtkRenderWindowInteractor renderWindowInteractor)
        {
            base.SetInteractor(renderer, renderWindowInteractor);
            //
            _renderer.AddActor(_textActorFooter);
            _renderer.AddActor(_textActorLabel);
            _renderer.AddActor(_colorBarColorsActor);
            _renderer.AddActor(_colorBarBorderActor);
        }
        public override void RemoveInteractor()
        {
            _renderer.RemoveActor(_textActorFooter);
            _renderer.RemoveActor(_textActorLabel);
            _renderer.RemoveActor(_colorBarColorsActor);
            _renderer.RemoveActor(_colorBarBorderActor);
            //
            base.RemoveInteractor();
        }
        public void ClearColorsAndLabels()
        {
            SetColorsAndLabels(null, null);
        }
        public void SetColorsAndLabels(Color[] colors, string[] labels)
        {
            if (colors != null) _colors = colors.ToArray();
            else _colors = new Color[0];
            if (labels != null) _labels = labels.ToArray();
            else _labels = new string[0];
            // Replace _ and - for empty char
            for (int i = 0; i < _labels.Length; i++) _labels[i] = _labels[i].Replace('_', ' ').Replace('-', ' ');
            //
            _lookupTable = vtkLookupTable.New();
            _lookupTable.SetNumberOfTableValues(_colors.Length);
            _lookupTable.SetRange(0, _colors.Length);
            // Fill in the colors
            for (int i = 0; i < _colors.Length; i++)
            {
                _lookupTable.SetTableValue(i, colors[i].R / 255.0, colors[i].G / 255.0, colors[i].B / 255.0, 1.0);
            }
            //
            OnSizeChanged();
        }
        public void AddColorsAndLabels(Color[] colors, string[] labels)
        {
            if (colors != null && labels != null)
            {
                List<Color> allColors = new List<Color>(_colors);
                List<string> allLabels = new List<string>(_labels);
                //
                allColors.AddRange(colors);
                allLabels.AddRange(labels);
                //
                SetColorsAndLabels(allColors.ToArray(), allLabels.ToArray());
            }
        }
        //
        public override void SetTextProperty(vtkTextProperty textProperty)
        {
            base.SetTextProperty(textProperty);

            _textMapperFooter.SetTextProperty(textProperty);
            _textMapperLabel.SetTextProperty(textProperty);
        }


        // Public getters                                                                                                           
    }
}
