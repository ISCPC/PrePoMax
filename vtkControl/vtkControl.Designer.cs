namespace vtkControl
{
    partial class vtkControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Called to set the vtkRenderWindow size according to this control's
        /// Size property.
        /// </summary>
        internal void SyncRenderWindowSize()
        {
            if (this._renderWindow != null)
            {
                this._renderWindow.SetSize(this.Size.Width, this.Size.Height);
            }
        }

        private System.IntPtr XDisplay;

        /// <summary>
        /// Retrieve the X11 Display* to pass to VTK's vtkRenderWindow::SetDisplayId
        /// </summary>
        private System.IntPtr GetXDisplay()
        {
            System.Type xplatui = System.Type.GetType("System.Windows.Forms.XplatUIX11, System.Windows.Forms");
            if (xplatui != null)
            {
                System.IntPtr DisplayHandle = (System.IntPtr)xplatui.
                  GetField("DisplayHandle", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).
                  GetValue(null);
                XDisplay = DisplayHandle;

                // Also, may need possibly:
                // Setup correct X visual and colormap so that VTK OpenGL stuff
                // works properly on mono/X11. Cache the display value so that we
                // can use it to set the RenderWindowId in OnCreated.

                //System.IntPtr RootWindow = (System.IntPtr)xplatui.GetField("RootWindow", System.Reflection.BindingFlags.Static |
                //System.Reflection.BindingFlags.NonPublic).GetValue(null);
                //int ScreenNo = (int)xplatui.GetField("ScreenNo", System.Reflection.BindingFlags.Static | 
                //System.Reflection.BindingFlags.NonPublic).GetValue(null);
                //int[] dblBuf = new int[] { 5, (int)GLXFlags.GLX_RGBA, (int)GLXFlags.GLX_RED_SIZE, 1, (int)GLXFlags.GLX_GREEN_SIZE, 
                //1, (int)GLXFlags.GLX_BLUE_SIZE, 1, (int)GLXFlags.GLX_DEPTH_SIZE, 1, 0 };
                //GLXVisualInfo = glXChooseVisual(DisplayHandle, ScreenNo, dblBuf);
                //XVisualInfo xVisualInfo = (XVisualInfo)Marshal.PtrToStructure(GLXVisualInfo, typeof(XVisualInfo));
                //System.IntPtr visual = System.IntPtr.Zero; // xVisualInfo.visual;
                //System.IntPtr colormap = XCreateColormap(DisplayHandle, RootWindow, visual, 0/*AllocNone*/);
                //xplatui.GetField("CustomVisual", System.Reflection.BindingFlags.Static | 
                //System.Reflection.BindingFlags.NonPublic).SetValue(null, visual);
                //xplatui.GetField("CustomColormap", System.Reflection.BindingFlags.Static | 
                //System.Reflection.BindingFlags.NonPublic).SetValue(null, colormap);
            }

            return XDisplay;
        }

        private bool AttachedInteractor;

        private void AttachInteractor()
        {
            if (!this.AttachedInteractor)
            {
                this.AttachedInteractor = true;
                this._renderWindow.SetInteractor(this._renderWindowInteractor);
            }
        }

        /// <summary>
        /// OnHandleCreated.
        /// </summary>
        protected override void OnHandleCreated(System.EventArgs e)
        {
            if (!this.DesignMode)
            {
                this._renderer = Kitware.VTK.vtkRenderer.New();
                this._renderer.SetLayer(0);
                
                this._overlayRenderer = Kitware.VTK.vtkRenderer.New();
                this._overlayRenderer.SetLayer(1);
                this._overlayRenderer.InteractiveOff();
                this._overlayRenderer.SetActiveCamera(this._renderer.GetActiveCamera());

                this._selectionRenderer = Kitware.VTK.vtkRenderer.New();
                this._selectionRenderer.SetLayer(2);
                this._selectionRenderer.InteractiveOff();
                this._selectionRenderer.SetActiveCamera(this._renderer.GetActiveCamera());
                
                this._renderWindow = Kitware.VTK.vtkRenderWindow.New();

                System.IntPtr xdisplay = GetXDisplay();
                bool isX11 = false;
                if (System.IntPtr.Zero != xdisplay)
                {
                    isX11 = true;
                }

                if (isX11)
                {
                    // If running an X11-based build, then we must use a
                    // vtkGenericRenderWindowInteractor:
                    //
                    this._renderWindowInteractor = Kitware.VTK.vtkGenericRenderWindowInteractor.New();
                    this._renderWindow.SetDisplayId(xdisplay);
                }
                else
                {
                    // Allow the native factory method to produce the natively expected subclass
                    // of vtkRenderWindowInteractor:
                    //
                    this._renderWindowInteractor = Kitware.VTK.vtkRenderWindowInteractor.New();
                }

                Kitware.VTK.vtkInteractorStyleSwitch style = this._renderWindowInteractor.GetInteractorStyle() as Kitware.VTK.vtkInteractorStyleSwitch;
                if (null != style)
                {
                    style.SetCurrentStyleToTrackballCamera();
                }

                this._renderWindow.SetParentId(this.Handle);
                this._renderWindow.SetNumberOfLayers(3);

                this._renderWindow.AddRenderer(this._renderer);
                this._renderWindow.AddRenderer(this._overlayRenderer);
                this._renderWindow.AddRenderer(this._selectionRenderer);

                if (!isX11)
                {
                    this.AttachInteractor();
                }

                //InitializeControl();
            }

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// OnHandleDestroyed.
        /// </summary>
        protected override void OnHandleDestroyed(System.EventArgs e)
        {
            if (this._renderer != null) this._renderer.SetRenderWindow(null);
            if (this._overlayRenderer != null) this._overlayRenderer.SetRenderWindow(null);
            if (this._selectionRenderer != null) this._selectionRenderer.SetRenderWindow(null);

            if (this._renderWindowInteractor != null)
            {
                if (_coorSys != null) this._coorSys.SetInteractor(null);
                //if (_statusBlockWidget != null) this._statusBlockWidget.SetInteractor(null, null);
                //if (_minValueWidget != null) this._minValueWidget.SetInteractor(null, null);
                //if (_maxValueWidget != null) this._maxValueWidget.SetInteractor(null, null);
                //if (_probeWidget != null) this._probeWidget.SetInteractor(null, null);
                //if (_scalarBarWidget != null) this._scalarBarWidget.SetInteractor(null, null);

                this._renderWindowInteractor.Dispose();
                this._renderWindowInteractor = null;
            }

            if (this._renderWindow != null)
            {
                this._renderWindow.Dispose();
                this._renderWindow = null;
            }

            if (this._renderer != null)
            {
                this._renderer.Dispose();
                this._renderer = null;
            }
            if (this._overlayRenderer != null)
            {
                this._overlayRenderer.Dispose();
                this._overlayRenderer = null;
            }
            if (this._selectionRenderer != null)
            {
                this._selectionRenderer.Dispose();
                this._selectionRenderer = null;
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// OnMouseDown.
        /// </summary>
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            // This is only for X11
            Kitware.VTK.vtkGenericRenderWindowInteractor grwi = this._renderWindowInteractor as Kitware.VTK.vtkGenericRenderWindowInteractor;
            if (null != grwi)
            {
                grwi.SetEventInformationFlipY(e.X, e.Y, 0, 0, 0, e.Clicks, null);

                switch (e.Button)
                {
                    case System.Windows.Forms.MouseButtons.Left:
                        grwi.LeftButtonPressEvent();
                        break;

                    case System.Windows.Forms.MouseButtons.Middle:
                        grwi.MiddleButtonPressEvent();
                        break;

                    case System.Windows.Forms.MouseButtons.Right:
                        grwi.RightButtonPressEvent();
                        break;
                }
            }
            Kitware.VTK.vtkRenderWindowInteractor rwi = this._renderWindowInteractor as Kitware.VTK.vtkRenderWindowInteractor;
            if (null != rwi)
            {
                rwi.SetEventInformationFlipY(e.X, e.Y, 0, 0, 0, e.Clicks, null);

                switch (e.Button)
                {
                    case System.Windows.Forms.MouseButtons.Left:
                        rwi.LeftButtonPressEvent();
                        break;

                    case System.Windows.Forms.MouseButtons.Middle:
                        rwi.MiddleButtonPressEvent();
                        break;

                    case System.Windows.Forms.MouseButtons.Right:
                        rwi.RightButtonPressEvent();
                        break;
                }
            }
        }

        /// <summary>
        /// OnMouseMove.
        /// </summary>
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            // This is only for X11
            Kitware.VTK.vtkGenericRenderWindowInteractor grwi = this._renderWindowInteractor as Kitware.VTK.vtkGenericRenderWindowInteractor;
            if (null != grwi)
            {
                grwi.SetEventInformationFlipY(e.X, e.Y, 0, 0, 0, e.Clicks, null);

                grwi.MouseMoveEvent();
            }
        }

        /// <summary>
        /// OnMouseUp.
        /// </summary>
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            // This is only for X11
            Kitware.VTK.vtkGenericRenderWindowInteractor grwi = this._renderWindowInteractor as Kitware.VTK.vtkGenericRenderWindowInteractor;
            if (null != grwi)
            {
                grwi.SetEventInformationFlipY(e.X, e.Y, 0, 0, 0, e.Clicks, null);

                switch (e.Button)
                {
                    case System.Windows.Forms.MouseButtons.Left:
                        grwi.LeftButtonReleaseEvent();
                        break;

                    case System.Windows.Forms.MouseButtons.Middle:
                        grwi.MiddleButtonReleaseEvent();
                        break;

                    case System.Windows.Forms.MouseButtons.Right:
                        grwi.RightButtonReleaseEvent();
                        break;
                }
            }
        }

        /// <summary>
        /// OnMouseWheel.
        /// </summary>
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            //Kitware.VTK.vtkGenericRenderWindowInteractor grwi = this._renderWindowInteractor as Kitware.VTK.vtkGenericRenderWindowInteractor;
            Kitware.VTK.vtkRenderWindowInteractor grwi = this._renderWindowInteractor;
            
            if (null != grwi)
            {
                grwi.SetEventInformationFlipY(e.X, e.Y, 0, 0, 0, e.Clicks, null);
                if (_mouseIn)
                {
                    if (e.Delta > 0)
                    {
                        grwi.MouseWheelForwardEvent();
                    }
                    else
                    {
                        grwi.MouseWheelBackwardEvent();
                    }
                }
            }
        }

        /// <summary>
        /// OnKeyDown.
        /// </summary>
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (this._renderWindowInteractor is Kitware.VTK.vtkGenericRenderWindowInteractor grwi)
            {
                grwi.SetKeyEventInformation(e.Control ? 1 : 0, e.Shift ? 1 : 0, (sbyte)e.KeyCode, 1, null);
                grwi.KeyPressEvent();
            }
            else if (this._renderWindowInteractor is Kitware.VTK.vtkRenderWindowInteractor rwi)
            {
                rwi.SetKeyEventInformation(e.Control ? 1 : 0, e.Shift ? 1 : 0, (sbyte)e.KeyCode, 1, null);
                rwi.KeyPressEvent();
            }
        }

        /// <summary>
        /// OnKeyPress.
        /// </summary>
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            Kitware.VTK.vtkGenericRenderWindowInteractor grwi = this._renderWindowInteractor as Kitware.VTK.vtkGenericRenderWindowInteractor;
            if (null != grwi)
            {
                grwi.SetKeyEventInformation(0, 0, (sbyte)e.KeyChar, 1, e.KeyChar.ToString());

                grwi.CharEvent();
            }
        }

        /// <summary>
        /// OnKeyUp.
        /// </summary>
        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            Kitware.VTK.vtkGenericRenderWindowInteractor grwi = this._renderWindowInteractor as Kitware.VTK.vtkGenericRenderWindowInteractor;
            if (null != grwi)
            {
                grwi.SetKeyEventInformation(e.Control ? 1 : 0, e.Shift ? 1 : 0, (sbyte)e.KeyCode, 1, null);

                grwi.KeyReleaseEvent();
            }
        }

        /// <summary>
        /// OnSizeChanged fires after the Size property has changed value.
        /// </summary>
        protected override void OnSizeChanged(System.EventArgs e)
        {
            this.SyncRenderWindowSize();

            Kitware.VTK.vtkGenericRenderWindowInteractor grwi = this._renderWindowInteractor as Kitware.VTK.vtkGenericRenderWindowInteractor;
            if (null != grwi)
            {
                grwi.ConfigureEvent();
            }

            base.OnSizeChanged(e);

            this.Invalidate();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern System.IntPtr SetFocus(System.IntPtr hWnd);

        /// <summary>
        /// OnGotFocus fires after Windows keyboard focus enters the control.
        /// </summary>
        protected override void OnGotFocus(System.EventArgs e)
        {
            if (this._renderWindow != null)
            {
                System.IntPtr hWnd = (System.IntPtr)this._renderWindow.GetGenericWindowId();
                if (System.IntPtr.Zero != hWnd)
                {
                    try
                    {
                        // TODO: X-Windows equivalent to Win32 SetFocus?
                        //
                        // (For now, the try/catch block handles trying to call the Win32
                        // function even on mono/Linux/Mac, but it would be nice to have
                        // the correct behavior on all platforms...)
                        //
                        SetFocus(hWnd);
                    }
                    catch
                    {
                    }
                }
            }

            base.OnGotFocus(e);
        }


        /// <summary>
        /// Override to do "last minute cram" of child control...
        /// </summary>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (this._renderWindow != null && _renderingOn)
            {
                if (this.Visible)
                {
                    this.SyncRenderWindowSize();
                    //
                    if (this._renderWindow.GetInteractor() != this._renderWindowInteractor)
                    {
                        // On X11, the SetInteractor method cannot be called until the parent
                        // window is already visible/mapped/painting... Since SetInteractor
                        // is the method that does this in VTK, we avoid calling it until now:
                        // when our parent has definitely shown us and we are in the process
                        // of painting.
                        //
                        this.AttachInteractor();
                        //this._renderWindow.Render();
                    }
                    this._renderWindow.Render();
                    //System.Console.WriteLine(System.DateTime.Now + ": OnRender");
                }
            }
            base.OnPaint(e);
        }


        /// <summary>
        /// OnVisibleChanged fires after the Visible property has changed value.
        /// </summary>
        protected override void OnVisibleChanged(System.EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // vtkControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "vtkControl";
            this.Size = new System.Drawing.Size(475, 326);
            this.Load += new System.EventHandler(this.vtkControl_Load);
            this.Resize += new System.EventHandler(this.vtkControl_Resize);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
