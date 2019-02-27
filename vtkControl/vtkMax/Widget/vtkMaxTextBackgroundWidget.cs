using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace vtkControl
{
    class vtkMaxTextBackgroundWidget : vtkMaxTextWidget
    {
        // Variables                                                                                                                
        protected vtkPolyDataMapper2D _backgroundMapper;
        protected vtkActor2D _backgroundActor;

        protected vtkPolyDataMapper2D _borderMapper;
        protected vtkActor2D _borderActor;

        protected bool _backgroundVisibility;
        protected bool _borderVisibility;
        

        // Constructors                                                                                                             
        public vtkMaxTextBackgroundWidget()
        {
            _backgroundVisibility = true;
            _borderVisibility = true;

            vtkPoints backgroundPoints = vtkPoints.New();
            backgroundPoints.SetNumberOfPoints(4);
            backgroundPoints.SetPoint(0, 0.0, 0.0, 0.0);
            backgroundPoints.SetPoint(1, 10.5, 0.0, 0.0);
            backgroundPoints.SetPoint(2, 10.5, 10.5, 0.0);
            backgroundPoints.SetPoint(3, 0.0, 10.5, 0.0);

            vtkCellArray backgroundPolygon = vtkCellArray.New();
            backgroundPolygon.InsertNextCell(4);
            backgroundPolygon.InsertCellPoint(0);
            backgroundPolygon.InsertCellPoint(1);
            backgroundPolygon.InsertCellPoint(2);
            backgroundPolygon.InsertCellPoint(3);

            vtkPolyData backgroundPolyData = vtkPolyData.New();
            backgroundPolyData.SetPoints(backgroundPoints);
            backgroundPolyData.SetPolys(backgroundPolygon);

            _backgroundMapper = vtkPolyDataMapper2D.New();
            _backgroundMapper.SetInput(backgroundPolyData);

            _backgroundActor = vtkActor2D.New();
            _backgroundActor.SetMapper(_backgroundMapper);
            _backgroundActor.VisibilityOff();

            _backgroundActor.GetPositionCoordinate().SetReferenceCoordinate(_borderRepresentation.GetPositionCoordinate());

            vtkCellArray borderPolygon = vtkCellArray.New();
            borderPolygon.InsertNextCell(5);
            borderPolygon.InsertCellPoint(0);
            borderPolygon.InsertCellPoint(1);
            borderPolygon.InsertCellPoint(2);
            borderPolygon.InsertCellPoint(3);
            borderPolygon.InsertCellPoint(0);

            vtkPolyData borderPolyData = vtkPolyData.New();
            borderPolyData.SetPoints(backgroundPoints);
            borderPolyData.SetLines(borderPolygon);

            _borderMapper = vtkPolyDataMapper2D.New();
            _borderMapper.SetInput(borderPolyData);

            _borderActor = vtkActor2D.New();
            _borderActor.SetMapper(_borderMapper);
            _borderActor.GetProperty().SetColor(0, 0, 0);
            _borderActor.VisibilityOff();

            _borderActor.GetPositionCoordinate().SetReferenceCoordinate(_borderRepresentation.GetPositionCoordinate());
        }


        // Private methods                                                                                                          
        public override void OnSizeChanged()
        {
            base.OnSizeChanged();

            double[] borderSize = _borderRepresentation.GetPosition2();

            vtkPoints backgroundPoints = _backgroundMapper.GetInput().GetPoints();

            backgroundPoints.SetPoint(1, borderSize[0], 0.0, 0.0);
            backgroundPoints.SetPoint(2, borderSize[0], borderSize[1], 0.0);
            backgroundPoints.SetPoint(3, 0.0, borderSize[1], 0.0);
            backgroundPoints.Modified();
        }


        // Public methods                                                                                                           
        public static new vtkMaxTextBackgroundWidget New()
        {
            return new vtkMaxTextBackgroundWidget();
        }
        public override void VisibilityOn()
        {
            base.VisibilityOn();
            if (_backgroundActor != null && _backgroundVisibility) _backgroundActor.VisibilityOn();
            if (_borderActor != null && _borderVisibility) _borderActor.VisibilityOn();
        }
        public override void VisibilityOff()
        {
            base.VisibilityOff();
            if (_backgroundActor != null) _backgroundActor.VisibilityOff();
            if (_borderActor != null) _borderActor.VisibilityOff();
        }
        public void BackgroundVisibilityOff()
        {
            _backgroundVisibility = false;
            if (_backgroundActor.GetVisibility() == 1) _backgroundActor.VisibilityOff();
        }
        public void BackgroundVisibilityOn()
        {
            _backgroundVisibility = true;
            if (_backgroundActor.GetVisibility() == 0) _backgroundActor.VisibilityOn();
        }

        public void BorderVisibilityOff()
        {
            _borderVisibility = false;
            if (_borderActor.GetVisibility() == 1) _borderActor.VisibilityOff();
        }
        public void BorderVisibilityOn()
        {
            _borderVisibility = true;
            if (_borderActor.GetVisibility() == 0) _borderActor.VisibilityOn();
        }


        // Public setters                                                                                                           
        public override void SetRenderer(vtkRenderer renderer)
        {
            if (_renderer != null && _backgroundActor != null) _renderer.RemoveActor(_backgroundActor);
            if (_renderer != null && _borderActor != null) _renderer.RemoveActor(_borderActor);

            renderer.AddActor(_backgroundActor);
            renderer.AddActor(_borderActor);

            base.SetRenderer(renderer);
        }
        public void SetBorderProperty(vtkProperty2D borderProperty)
        {
            _borderActor.SetProperty(borderProperty);
        }
        public void SetBackgroundProperty(vtkProperty2D backgroundProperty)
        {
            _backgroundActor.SetProperty(backgroundProperty);
        }


        // Public getters                                                                                                           
        public vtkProperty2D GetBackgroundProperty()
        {
            return _backgroundActor.GetProperty();
        }
    }
}
