using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace vtkControl
{
    class vtkMaxCaptionWidget : vtkMaxTextBackgroundWidget
    {
        // Variables                                                                                                                
        private bool _anchorAlreadySet;
        vtkCoordinate _worldAnchorPoint;
        vtkCoordinate _worldPositionPoint;

        vtkPolyData _leaderPolyData;
        vtkPolyDataMapper2D _leaderMapper2D;
        vtkActor2D _leaderActor2D;

        vtkPolyData _headPolyData;
        vtkGlyph3D _headGlyph;
        vtkActor2D _headActor2D;

        protected bool _enabled;

        // Variables                                                                                                                
        public bool Visibility
        {
            get { return base.GetVisibility() == 1 ? true : false; }
            set
            {
                if (value) VisibilityOn();
                else VisibilityOff();
            }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }


        // Constructors                                                                                                             
        public vtkMaxCaptionWidget()
        {
            _enabled = false;
            _anchorAlreadySet = false;
            _scaleBySectors = false;

            vtkPoints pts;
            vtkDoubleArray vecs;

            _worldAnchorPoint = vtkCoordinate.New();
            _worldAnchorPoint.SetCoordinateSystemToWorld();
            _worldAnchorPoint.SetValue(0, 0, 0);

            _worldPositionPoint = vtkCoordinate.New();
            _worldPositionPoint.SetCoordinateSystemToWorld();

            // This is the leader (line) from the attachment point to the caption
            _leaderPolyData = vtkPolyData.New();
            pts = vtkPoints.New();
            pts.SetNumberOfPoints(2);
            pts.SetPoint(0, 0, 0, 0);
            pts.SetPoint(0, 100, 100, 0);
            _leaderPolyData.SetPoints(pts);
            vtkCellArray leader = vtkCellArray.New();
            leader.InsertNextCell(2);
            leader.InsertCellPoint(0);
            leader.InsertCellPoint(1); //at the attachment point
            _leaderPolyData.SetLines(leader);

            // mapper/actor combination
            _leaderMapper2D = vtkPolyDataMapper2D.New();
            _leaderMapper2D.SetInput(_leaderPolyData);

            _leaderActor2D = vtkActor2D.New();
            _leaderActor2D.SetMapper(_leaderMapper2D);
            _leaderActor2D.GetProperty().SetColor(0, 0, 0);

            // Arrow
            vtkArrowSource arrowSource = vtkArrowSource.New();
            arrowSource.SetTipLength(1);
            arrowSource.SetTipRadius(0.2);
            arrowSource.SetShaftRadius(0.3);

            vtkTransform transform = vtkTransform.New();
            transform.Translate(-1, 0, 0);

            // Transform the polydata
            vtkTransformPolyDataFilter translatedArrow = vtkTransformPolyDataFilter.New();
            translatedArrow.SetTransform(transform);
            translatedArrow.SetInputConnection(arrowSource.GetOutputPort());

            // This is for glyphing the head of the leader: A single point with a vector for glyph orientation
            _headPolyData = vtkPolyData.New();
            pts = vtkPoints.New();
            pts.SetNumberOfPoints(1);
            pts.SetPoint(0, 0, 0, 0);
            _headPolyData.SetPoints(pts);
            vecs = vtkDoubleArray.New();
            vecs.SetNumberOfComponents(3);
            vecs.SetNumberOfTuples(1);
            vecs.SetTuple3(0, 1, 1, 1);
            _headPolyData.GetPointData().SetVectors(vecs);

            _headGlyph = vtkGlyph3D.New();
            _headGlyph.SetSourceConnection(translatedArrow.GetOutputPort());
            _headGlyph.SetInput(_headPolyData);
            _headGlyph.SetScaleModeToDataScalingOff();
            _headGlyph.SetScaleFactor(20);
            _headGlyph.Update();

            // Mapper/actor combination
            vtkPolyDataMapper2D arrowMapper2D = vtkPolyDataMapper2D.New();
            arrowMapper2D.SetInput(_headGlyph.GetOutput());

            _headActor2D = vtkActor2D.New();
            _headActor2D.SetMapper(arrowMapper2D);
            _headActor2D.GetProperty().SetColor(0, 0, 0);

            // Set initial position out of view
            _borderRepresentation.GetPositionCoordinate().SetValue(-1, -1);
        }


        // Private methods                                                                                                          
        private void RecomputeLeader()
        {
            if (_positionInPixels == null) return;      // if borederRepresentation is changed before _positionInPixels is initialized

            double[] end = RecomputeLeaderEnd();        // this is arrow head
            double[] start = RecomputeLeaderStart();    // this is the start of the arrow
            RecomputeHead(start, end);
        }
        private double[] RecomputeLeaderStart()
        {
            vtkPoints pts = _leaderPolyData.GetPoints();

            double[] head = pts.GetPoint(1);
            double[] size = _borderRepresentation.GetPosition2Coordinate().GetValue();
            double[] start;
            if (head[0] < _positionInPixels[0])                     // head is left from border
                start = new double[] { _positionInPixels[0], _positionInPixels[1] + size[1] / 2 };
            else if (head[0] > _positionInPixels[0] + size[0])      // head is right from border
                start = new double[] { _positionInPixels[0] + size[0], _positionInPixels[1] + size[1] / 2 };
            else if (head[1] < _positionInPixels[1])                // head is below the border
                start = new double[] { _positionInPixels[0] + size[0] / 2, _positionInPixels[1] };
            else                                                    // head is over the border
                start = new double[] { _positionInPixels[0] + size[0] / 2, _positionInPixels[1] + size[1] };

            pts.SetPoint(0, start[0], start[1], 0);

            return start;
        }
        private double[] RecomputeLeaderEnd()
        {
            vtkPoints pts = _leaderPolyData.GetPoints();

            double[] attach = _worldAnchorPoint.GetComputedWorldValue(_renderer);
            _renderer.SetWorldPoint(attach[0], attach[1], attach[2], 1.0);
            _renderer.WorldToDisplay();
            double[] display = _renderer.GetDisplayPoint();
            pts.SetPoint(1, display[0], display[1], 0);

            return display;
        }
        private void RecomputeHead(double[] start, double[] end)
        {
            _headPolyData.GetPoints().SetPoint(0, end[0], end[1], 0);
            _headPolyData.GetPointData().GetVectors().SetTuple3(0, end[0] - start[0], end[1] - start[1], 0);
            _headPolyData.Modified();
        }
        private double[] ProjectPointOnPlane(double[] point, double[] pointOnPlane, double[] normal)
        {
            //The projection of a point q = (x, y, z) onto a plane given by a point p = (a, b, c) and a normal n = (d, e, f) is
            //q_proj = q - dot(q - p, n) * n

            double[] tmp = new double[3];
            tmp[0] = point[0] - pointOnPlane[0];
            tmp[1] = point[1] - pointOnPlane[1];
            tmp[2] = point[2] - pointOnPlane[2];

            double dot = tmp[0] * normal[0] + tmp[1] * normal[1] + tmp[2] * normal[2];

            tmp[0] = point[0] - dot * normal[0];
            tmp[1] = point[1] - dot * normal[1];
            tmp[2] = point[2] - dot * normal[2];

            return tmp;
        }


        // Public methods                                                                                                           
        public override void OnBorderRepresentationModified()
        {
            base.OnBorderRepresentationModified();

            vtkInteractorStyle style = (vtkInteractorStyle)_borderWidget.GetInteractor().GetInteractorStyle();
            if (style.GetState() == vtkInteractorStyleControl.VTKIS_NONE)   // only draging or zoom
                SetWorldPositionFromPosition();

            RecomputeLeader();
        }
        public override void OnRenderWindowInteractorModified()
        {
            base.OnRenderWindowInteractorModified();

            SetPositionFromWorldPosition();

            if (_positionInPixels != null)
            {
                RecomputeLeader();
            }
        }
        public override void OnMouseWheelScrolled()
        {
            base.OnMouseWheelScrolled();

            SetPositionFromWorldPosition();

            if (_positionInPixels != null)
            {
                RecomputeLeader();
                _borderWidget.GetInteractor().Render();
            }

        }
        public override void OnRenderWindowModified()
        {
            base.OnRenderWindowModified();

            if (_positionInPixels != null) RecomputeLeader();
        }

        public override void VisibilityOn()
        {
            base.VisibilityOn();
            if (_leaderActor2D != null) _leaderActor2D.VisibilityOn();
            if (_headActor2D != null) _headActor2D.VisibilityOn();
        }
        public override void VisibilityOff()
        {
            base.VisibilityOff();
            if (_leaderActor2D != null) _leaderActor2D.VisibilityOff();
            if (_headActor2D != null) _headActor2D.VisibilityOff();
        }


        // Public setters                                                                                                           
        public override void SetRenderer(vtkRenderer renderer)
        {
            if (_renderer != null && _leaderActor2D != null) _renderer.RemoveActor(_leaderActor2D);
            renderer.AddActor(_leaderActor2D);
            renderer.AddActor(_headActor2D);
            base.SetRenderer(renderer);
        }

        public override void SetInteractor(vtkRenderWindowInteractor renderWindowInteractor)
        {
            SetInteractor(renderWindowInteractor, true);
        }
        public void SetAnchorPoint(double x, double y, double z)
        {
            _worldAnchorPoint.SetValue(x, y, z);

            double[] position = _borderRepresentation.GetPositionCoordinate().GetValue();

            if (double.IsNaN(position[0]) || double.IsNaN(position[1]) || !_anchorAlreadySet)     // change the position only the first time the anchor point is set
            {
                _renderer.SetWorldPoint(x, y, z, 1.0);
                _renderer.WorldToDisplay();
                double[] displayAnchor = _renderer.GetDisplayPoint();

                double[] size = _borderRepresentation.GetPosition2Coordinate().GetValue();
                int[] windowSize = _renderer.GetSize();

                double dx = -80;
                double dy = 50;

                if (displayAnchor[0] + dx + size[0] > windowSize[0]) dx = -(size[0] + dx);     // if there is no room to the left, go to the right
                if (displayAnchor[1] + dy + size[1] > windowSize[1]) dy = -(size[1] + dy);     // if there is no room to the top, go down

                _renderer.SetDisplayPoint(displayAnchor[0] + dx, displayAnchor[1] + dy, 0);
                _renderer.DisplayToWorld();
                double[] positionWorld = _renderer.GetWorldPoint();

                double[] normal = _renderer.GetActiveCamera().GetViewPlaneNormal();
                double[] pointOnPlane = _worldAnchorPoint.GetValue();
                double[] projectedOnPlane = ProjectPointOnPlane(positionWorld, pointOnPlane, normal);
                _worldPositionPoint.SetValue(projectedOnPlane[0], projectedOnPlane[1], projectedOnPlane[2]);

                SetPositionFromWorldPosition();

                _anchorAlreadySet = true;
            }
            RecomputeLeader();
            OnBorderRepresentationModified();
        }
        private void SetPositionFromWorldPosition()
        {
            vtkCamera camera = _renderer.GetActiveCamera();
            double[] positionWorld = _worldPositionPoint.GetValue();

            // get display projection from the world position
            _renderer.SetWorldPoint(positionWorld[0], positionWorld[1], positionWorld[2], 1.0);
            _renderer.WorldToDisplay();
            double[] displayPosition = _renderer.GetDisplayPoint();

            // move to the center of the caption
            double[] borderSize = _borderRepresentation.GetPosition2Coordinate().GetValue();
            displayPosition[0] -= borderSize[0] / 2;
            displayPosition[1] -= borderSize[1] / 2;

            // convert
            _renderer.DisplayToNormalizedDisplay(ref displayPosition[0], ref displayPosition[1]);

            SetPosition(displayPosition[0], displayPosition[1]);
        }
        private void SetWorldPositionFromPosition()
        {
            if (_positionInPixels == null) return;      // if borederRepresentation is changed before _positionInPixels is initialized

            vtkCamera camera = _renderer.GetActiveCamera();
            double[] borderSize = _borderRepresentation.GetPosition2Coordinate().GetValue();

            // get world projection from display position
            _renderer.SetDisplayPoint(_positionInPixels[0] + borderSize[0] / 2, _positionInPixels[1] + borderSize[1] / 2, 0);
            _renderer.DisplayToWorld();
            double[] positionWorld = _renderer.GetWorldPoint();

            // project world position on the plane through anchor point
            double[] normal = camera.GetViewPlaneNormal();
            double[] pointOnPlane = _worldAnchorPoint.GetValue();
            double[] projectedOnPlane = ProjectPointOnPlane(positionWorld, pointOnPlane, normal);

            // set world position
            _worldPositionPoint.SetValue(projectedOnPlane[0], projectedOnPlane[1], projectedOnPlane[2]);
        }


        // Public getters                                                                                                           
    }
}
