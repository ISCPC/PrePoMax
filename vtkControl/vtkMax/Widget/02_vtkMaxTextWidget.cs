using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace vtkControl
{
    class vtkMaxTextWidget : vtkMaxBorderWidget
    {
        // Variables                                                                                                                
        protected vtkActor2D _textActor;
        protected vtkTextMapper _textMapper;
        protected int _padding;
        protected string _numberFormat;


        // Constructors                                                                                                             
        public vtkMaxTextWidget()
        {
            _padding = 0;

            // Text property
            vtkTextProperty textProperty = vtkTextProperty.New();

            // Mapper
            _textMapper = vtkTextMapper.New();
            _textMapper.SetTextProperty(textProperty);

            // Actor
            _textActor = vtkActor2D.New();
            _textActor.SetMapper(_textMapper);

            // Set relative text position
            _textActor.GetPositionCoordinate().SetCoordinateSystemToDisplay();  // set offsets in pixels
            _textActor.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
            _textActor.GetPositionCoordinate().SetValue(_padding, _padding);
        }


        // Public methods                                                                                                           
        public override void VisibilityOn()
        {
            if (_visibility == false)
            {
                OnSizeChanged();    // the text might chnage when the widget is turened off
                base.VisibilityOn();
                if (_textActor != null) _renderer.AddActor(_textActor);
            }
        }
        public override void VisibilityOff()
        {
            if (_visibility == true)
            {
                base.VisibilityOff();
                if (_textActor != null) _renderer.RemoveActor(_textActor);
            }
        }
        public override void BackgroundVisibilityOn()
        {
            if (_backgroundVisibility == false)
            {
                _renderer.RemoveActor(_textActor); // remove text
                base.BackgroundVisibilityOn();
                _renderer.AddActor(_textActor); // add text back
            }
        }
        public override void OnSizeChanged()
        {
            int[] textSize = vtkMaxWidgetTools.GetTextSize(_textMapper, _renderer);
            _size[0] = textSize[0] + 2 * _padding;
            _size[1] = textSize[1] + 2 * _padding;
            base.OnSizeChanged();
        }


        // Private methods                                                                                                          


        // Public setters                                                                                                           
        public override void SetInteractor(vtkRenderer renderer, vtkRenderWindowInteractor renderWindowInteractor)
        {
            base.SetInteractor(renderer, renderWindowInteractor);
            //
            _renderer.AddActor(_textActor);
        }
        public override void RemoveInteractor()
        {
            _renderer.RemoveActor(_textActor);
            //
            base.RemoveInteractor();
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
        public void SetNumberFormat(string numberFormat)
        {
            _numberFormat = numberFormat;
        }


        // Public getters                                                                                                           
        public string GetText()
        {
            return _textMapper.GetInput();
        }
        public string GetNumberFormat()
        {
            return _numberFormat;
        }
    }
}
