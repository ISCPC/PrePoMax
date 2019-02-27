using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Kitware.VTK;

namespace vtkControl
{
    // The widget is a vtkBorderWidget and a vtkActor2D representing text             
    //                                                                                
    // The position of the vtkActor2D is given by the relative position to the position of the border.
    // _borderRepresentation.GetPositionCoordinate().SetCoordinateSystemToNormalizedViewport() must be defined in order for the draging to work
    // _borderRepresentation.GetPosition2Coordinate().SetCoordinateSystemToDisplay() must be defined in order for the padding to work

    class vtkMaxTextWidget
    {
        // Variables                                                                                                                
        protected vtkActor2D _textActor;
        protected vtkTextMapper _textMapper;
        protected vtkBorderRepresentation _borderRepresentation;
        protected vtkBorderWidget _borderWidget;
        protected vtkRenderer _renderer;

        protected bool _visibility;
        protected int _padding;
        protected double[] _positionInPixels;
        protected int _quadrant;
        protected int[] _prewWindowSize;
        protected bool _scaleBySectors;


        // Constructors                                                                                                             
        public vtkMaxTextWidget()
        {
            _visibility = false;
            _padding = 0;
            _positionInPixels = null;
            _prewWindowSize = new int[] { 0, 0 };
            _scaleBySectors = true;

            _renderer = null;

            // Text property
            vtkTextProperty textProperty = vtkTextProperty.New();

            // Mapper
            _textMapper = vtkTextMapper.New();
            _textMapper.SetTextProperty(textProperty);
            
            // Actor
            _textActor = vtkActor2D.New();
            _textActor.SetMapper(_textMapper);
            _textActor.VisibilityOff();

            // Border representation
            _borderRepresentation = vtkBorderRepresentation.New();

            // Set relative text position
            _textActor.GetPositionCoordinate().SetCoordinateSystemToDisplay();  // set offsets in pixels
            _textActor.GetPositionCoordinate().SetValue(_padding, _padding);

            // Border widget
            _borderWidget = vtkBorderWidget.New();
            _borderWidget.SelectableOff();                                      // to enable draging
            _borderWidget.ResizableOff();
            _borderWidget.SetEnabled(0);


            _borderRepresentation = vtkBorderRepresentation.New();            
            _borderRepresentation.GetBorderProperty().SetOpacity(0);
            // the public method can not be called here; it calls the overloaded SetBorderRepresentation() and not this classes
            SetBorderRepresentationPrivate(_borderRepresentation);

            _borderWidget.KeyPressActivationOff();
        }


        // Event handlers                                                                                                           
        void renderWindow_ModifiedEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            if (_visibility)
            {
                int[] windowSize = _renderer.GetSize();
                if (windowSize[0] != _prewWindowSize[0] || windowSize[1] != _prewWindowSize[1])
                {
                    //Console.WriteLine(DateTime.Now.ToString() + " renderWindow_ModifiedEvt");

                    //OnBorderRepresentationModified();   // to reposition the widget
                    OnRenderWindowModified();
                }
            }
            _renderer.GetSize().CopyTo(_prewWindowSize, 0);
        }
        void renderWindowInteractor_ModifiedEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            vtkInteractorStyleControl style = (vtkInteractorStyleControl)_borderWidget.GetInteractor().GetInteractorStyle();

            if (_visibility && (style.GetState() != vtkInteractorStyleControl.VTKIS_NONE || style.Animating))
            {
                //Console.WriteLine(DateTime.Now.ToString() + " renderWindowInteractor_ModifiedEvt");

                OnRenderWindowInteractorModified();
            }
        }
        void renderWindowInteractor_MouseWheelEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            if (_visibility)
            {
                //Console.WriteLine(DateTime.Now.ToString() + " renderWindowInteractor_MouseWheelEvt");

                OnMouseWheelScrolled();
            }
        }
        private void renderWindowInteractor_KeyPressEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            // update position after key press
            if (_visibility)
            {
                OnMouseWheelScrolled();
            }
        }
        void BorderRepresentation_ModifiedEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            //Console.WriteLine(DateTime.Now.ToString() + " BorderRepresentation_ModifiedEvt");

            OnBorderRepresentationModified();
        }
        

        // Private methods                                                                                                          
        protected int[] GetTextSize(vtkTextMapper textMapper)
        {
            int[] size = new int[2];

            if (_renderer != null)
            {
                IntPtr sizeIntPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * 4);
                textMapper.GetSize(_renderer, sizeIntPtr);
                System.Runtime.InteropServices.Marshal.Copy(sizeIntPtr, size, 0, 2);
            }
            else
            {
                size[0] = size[1] = 50;
            }

            return size;
        }
        private void SetBorderRepresentationPrivate(vtkBorderRepresentation borderRepresentation)
        {
            // dereference
            if (_borderRepresentation != null) _borderRepresentation.ModifiedEvt -= BorderRepresentation_ModifiedEvt;
            
            _borderRepresentation = borderRepresentation;
            _borderRepresentation.GetBorderProperty().SetOpacity(0);
            _borderRepresentation.GetPosition2Coordinate().SetCoordinateSystemToDisplay();              // set size in pixels
            _borderWidget.SetRepresentation(_borderRepresentation);

            _textActor.GetPositionCoordinate().SetReferenceCoordinate(_borderRepresentation.GetPositionCoordinate());

            _borderRepresentation.ModifiedEvt += BorderRepresentation_ModifiedEvt;
        }
        protected void SetInteractor(vtkRenderWindowInteractor renderWindowInteractor, bool interactive)
        {
            // dereference
            if (interactive && _borderWidget != null && _borderWidget.GetInteractor() != null)
            {
                _borderWidget.GetInteractor().ModifiedEvt -= renderWindowInteractor_ModifiedEvt;
                _borderWidget.GetInteractor().GetInteractorStyle().MouseWheelBackwardEvt -= renderWindowInteractor_MouseWheelEvt;
                _borderWidget.GetInteractor().GetInteractorStyle().MouseWheelForwardEvt -= renderWindowInteractor_MouseWheelEvt;
                _borderWidget.GetInteractor().GetInteractorStyle().KeyPressEvt -= renderWindowInteractor_KeyPressEvt;

            }
            if (_borderWidget != null && _borderWidget.GetInteractor() != null && _borderWidget.GetInteractor().GetRenderWindow() != null)
                _borderWidget.GetInteractor().GetRenderWindow().ModifiedEvt -= renderWindow_ModifiedEvt;

            _borderWidget.SetInteractor(renderWindowInteractor);

            // reference
            if (interactive && renderWindowInteractor != null)
            {
                renderWindowInteractor.ModifiedEvt += renderWindowInteractor_ModifiedEvt;
                renderWindowInteractor.GetInteractorStyle().MouseWheelBackwardEvt += renderWindowInteractor_MouseWheelEvt;
                renderWindowInteractor.GetInteractorStyle().MouseWheelForwardEvt += renderWindowInteractor_MouseWheelEvt;
                renderWindowInteractor.GetInteractorStyle().KeyPressEvt += renderWindowInteractor_KeyPressEvt;
            }
            if (renderWindowInteractor != null) renderWindowInteractor.GetRenderWindow().ModifiedEvt += renderWindow_ModifiedEvt;
        }

        

        // Public methods                                                                                                           
        public virtual void OnRenderWindowModified()
        {
            // triggered at window resized

            // set the widget position in relation to the windows borders
            if (_positionInPixels != null && _prewWindowSize[0] != 0 && _prewWindowSize[0] != 0)
            {
                int[] windowSize = _renderer.GetSize();

                if (windowSize[0] != _prewWindowSize[0] || windowSize[1] != _prewWindowSize[1] )
                {
                    double dx = 0;
                    double dy = 0;

                    if (_quadrant == 2 || _quadrant == 3)
                    {
                        dx = windowSize[0] - _prewWindowSize[0];
                    }
                    if (_quadrant == 3 || _quadrant == 4)
                    {
                        dy = windowSize[1] - _prewWindowSize[1];
                    }

                    if (_scaleBySectors)
                    {
                        double[] position = new double[] { _positionInPixels[0] + dx, _positionInPixels[1] + dy };
                        _renderer.DisplayToNormalizedDisplay(ref position[0], ref position[1]);
                        _borderRepresentation.GetPositionCoordinate().SetValue(position[0], position[1]);
                    }
                    _borderRepresentation.Modified();
                }
            }
        }
        public virtual void OnRenderWindowInteractorModified()
        {
        }
        public virtual void OnMouseWheelScrolled()
        {

        }
        public virtual void OnBorderRepresentationModified()
        {
            // triggered by moving the widget

            // save the current position in pixels

            // if this function is called when the renderer is not yet loaded, return
            int[] size = _renderer.GetSize();
            if (size[0] == 0 && size[1] == 0) return;

            double[] position = _borderRepresentation.GetPositionCoordinate().GetValue();
            double[] position2 = _borderRepresentation.GetPosition2Coordinate().GetValue();
            _renderer.DisplayToNormalizedDisplay(ref position2[0], ref position2[1]);

            double[] max = new double[] { 1 - position2[0], 1 - position2[1] };

            // check if widget fits into window to prevent looping
            if (position2[0] < 1 && position2[1] < 1)
            {
                // prevent the widget from going off the window
                if (position[0] < 0 || position[0] > max[0] || position[1] < 0 || position[1] > max[1])
                {
                    if (position[0] < 0) position[0] = 0;
                    else if (position[0] > max[0]) position[0] = max[0];
                    else if (position[1] < 0) position[1] = 0;
                    else if (position[1] > max[1]) position[1] = max[1];
                    _borderRepresentation.GetPositionCoordinate().SetValue(position[0], position[1]);
                    _borderRepresentation.Modified();
                    return;
                }
            }

            double[] center = new double[] { position[0] + position2[0] / 2, position[1] + position2[1] / 2 };

            // Determine the normalized quadrant
            if (center[0] < 0.5 && center[1] < 0.5) _quadrant = 1;
            else if (center[0] > 0.5 && center[1] < 0.5) _quadrant = 2;
            else if (center[0] > 0.5 && center[1] > 0.5) _quadrant = 3;
            else if (center[0] < 0.5 && center[1] > 0.5) _quadrant = 4;

            // Position to normalized coordinates
            _renderer.NormalizedDisplayToDisplay(ref position[0], ref position[1]);

            if (_positionInPixels == null) _positionInPixels = new double[2];

            _positionInPixels[0] = position[0];
            _positionInPixels[1] = position[1];
        }
        public virtual void OnSizeChanged()
        {
            int[] textSize = GetTextSize(_textMapper);
            _borderRepresentation.SetPosition2(textSize[0] + 2 * _padding, textSize[1] + 2 * _padding);
        }

        public static vtkMaxTextWidget New()
        {
            return new vtkMaxTextWidget();
        }
        public virtual void VisibilityOn()
        {
            if (_visibility == false)
            {
                if (_positionInPixels != null) SetDisplayPosition(_positionInPixels[0], _positionInPixels[1]);

                _visibility = true;
                if (_textActor != null) _textActor.VisibilityOn();
                if (_borderWidget != null) _borderWidget.SetEnabled(1); // activates OnBorderRepresentationModified so SetDisplayPosition must preceede it
            }
        }
        public virtual void VisibilityOff()
        {
            if (_visibility == true)
            {
                _visibility = false;
                if (_textActor != null) _textActor.VisibilityOff();
                if (_borderWidget != null) _borderWidget.SetEnabled(0);
            }
        }


        // Public setters                                                                                                           
        public virtual void SetRenderer(vtkRenderer renderer)
        {
            // remove actors from old renderers
            if (_renderer != null && _textActor != null) _renderer.RemoveActor(_textActor);

            _renderer = renderer;
            //_borderRepresentation.GetPositionCoordinate().SetViewport(_renderer);                       // this is the major line
            //_borderRepresentation.GetPosition2Coordinate().SetViewport(_renderer);                      // this is the major line
            renderer.AddActor(_textActor);
        }
        public virtual void SetInteractor(vtkRenderWindowInteractor renderWindowInteractor)
        {
            SetInteractor(renderWindowInteractor, false);
        }
        public virtual void SetTextProperty(vtkTextProperty textProperty)
        {
            _textMapper.SetTextProperty(textProperty);
        }
        public virtual void SetText(string text)
        {
            _textMapper.SetInput(text);
            OnSizeChanged();
        }
        public void SetPosition(double x, double y)
        {
            _borderRepresentation.GetPositionCoordinate().SetValue(x, y);
            _borderRepresentation.Modified();
        }
        public void SetDisplayPosition(double x, double y)
        {
            double[] position = new double[] { x, y };
            _renderer.DisplayToNormalizedDisplay(ref position[0], ref position[1]);

            // if the renderer size = 0, set the positionInPixels to use it in VisibilityOn
            if (_positionInPixels == null) _positionInPixels = new double[2];
            _positionInPixels[0] = x;
            _positionInPixels[1] = y;

            SetPosition(position[0], position[1]);
        }
        public void SetPadding(int padding)
        {
            if (padding != _padding)
            {
                _padding = padding;
                if (_textActor != null)
                {
                    _textActor.GetPositionCoordinate().SetValue(_padding, _padding);
                    OnSizeChanged();
                }
            }
        }


        // Public getters                                                                                                           
        public string GetText()
        {
            return _textMapper.GetInput();
        }
        public int GetVisibility()
        {
            if (_visibility) return 1;
            else return 0;
        }
        public vtkRenderWindowInteractor GetInteractor()
        {
            return _borderWidget.GetInteractor();
        }
        public vtkBorderWidget GetBorderWidget()
        {
            return _borderWidget;
        }
        public vtkBorderRepresentation GetBorderRepresentation()
        {
            return _borderRepresentation;
        }
        public vtkTextProperty GetTextProperty()
        {
            return _textMapper.GetTextProperty();
        }

    }
}
