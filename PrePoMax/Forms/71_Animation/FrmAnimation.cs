﻿using CaeResults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    enum AnimationStyle
    {
        Once,
        Loop,
        Swing
    }

    enum AnimationType
    {
        ScaleFactor,
        TimeIncrements,
        Harmonic
    }

    enum ColorSpectrumLimitsType
    {
        CurrentFrame,
        AllFrames
    }

    public partial class FrmAnimation : UserControls.PrePoMaxChildForm
    {
        // Variables                                                                                                                
        private int _countFrames;
        private Size _expandedSize;
        private bool _updateAnimation;
        private bool _doUpdateFrame;
        private int _numFrames;
        private int _currFrame;
        private int _frameDelta;
        private int _fps;
        private bool _isFirstFrame;
        private AnimationType _animationType;
        private AnimationStyle _animationStyle;
        private ColorSpectrumLimitsType _colorSpectrumLimitsType;
        private bool _setSaveAsFolder;
        //
        private Controller _controller;
        private FrmMain _form;


        // Callbacks                                                                                                                
        public Action<bool> Form_ControlsEnable;


        // Constructors                                                                                                             
        public FrmAnimation()
        {
            InitializeComponent();
            //
            _countFrames = 0;
            _updateAnimation = false;
            _expandedSize = Size;            
            _setSaveAsFolder = true;
            //
            btnMoreLess_Click(null, null);
        }


        // Event handling                                                                                                           
        private void FrmAnimation_Shown(object sender, EventArgs e)
        {
            if (_form == null || _controller == null)
                throw new Exception("The controler or the form of the FrmAnimation form are not set.");
        }
        private void btnPlayBackward_Click(object sender, EventArgs e)
        {
            PlayBackward();
        }
        private void btnPlayForward_Click(object sender, EventArgs e)
        {
            PlayForward();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void btnMoreLess_Click(object sender, EventArgs e)
        {
            Size size;
            if (btnMoreLess.Text == "More")
            {
                btnMoreLess.Text = "Less";
                size = _expandedSize;
            }
            else
            {
                btnMoreLess.Text = "More";
                size = new Size(320, 118);
            }

            this.MaximumSize = size;
            this.MinimumSize = size;
            this.Size = size;
        }
        private void tbarFrameSelector_ValueChanged(object sender, EventArgs e)
        {
            Stop();
            _currFrame = tbarFrameSelector.Value;
            UpdateFrame();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void FrmAnimation_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void FrmAnimation_VisibleChanged(object sender, EventArgs e)
        {
            // this is called if some other form is shown to close all other forms
            // this is called after the form visibility changes
            // the form was hidden 
            if (!this.Visible)
            {
                OnHide();
            }
        }
        private void btnSaveMovieAs_Click(object sender, EventArgs e)
        {
            if (cbSaveAsImages.Checked) SaveAsImages();
            else SaveAsMovie();
        }
        // Frames
        private void numFirstFrame_ValueChanged(object sender, EventArgs e)
        {
            Stop();
            tbarFrameSelector.Minimum = (int)numFirstFrame.Value;
            numCurrFrame.Minimum = (int)numFirstFrame.Value;
            numLastFrame.Minimum = numFirstFrame.Value + 1;
        }
        private void numCurrFrame_ValueChanged(object sender, EventArgs e)
        {
            Stop();
            _currFrame = (int)numCurrFrame.Value;
            UpdateFrame();
        }
        private void numLastFrame_ValueChanged(object sender, EventArgs e)
        {
            Stop();
            tbarFrameSelector.Maximum = (int)numLastFrame.Value;
            numFirstFrame.Maximum = numLastFrame.Value - 1;
            numCurrFrame.Maximum = (int)numLastFrame.Value;
        }
        // Animation type
        private void AnimationType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked) AnimationTypeChanged(true);
            }
        }
        private void numNumberOfFrames_ValueChanged(object sender, EventArgs e)
        {
            Stop();
            //
            SetNumberOfFrames((int)numNumberOfFrames.Value, false);
        }
        private void numNumberOfAngles_ValueChanged(object sender, EventArgs e)
        {
            Stop();
            //
            SetNumberOfFrames((int)numNumberOfAngles.Value, false);
        }
        private void SetNumberOfFrames(int value, bool setCurrentToLast)
        {
            _doUpdateFrame = false;
            //
            if (value != _numFrames)
            {
                _numFrames = value;
                //
                tbarFrameSelector.Maximum = _numFrames;
                numCurrFrame.Maximum = _numFrames;
                numFirstFrame.Maximum = _numFrames;
                numLastFrame.Maximum = _numFrames;
                //
                numFirstFrame.Value = 1;
                numLastFrame.Value = _numFrames;
                if (_currFrame > _numFrames || setCurrentToLast)
                {
                    _currFrame = _numFrames;
                    tbarFrameSelector.Value = _numFrames;
                }
                //
                _updateAnimation = true;
            }
            //
            _doUpdateFrame = true;
        }
        private void NumIncrementStep_ValueChanged(object sender, EventArgs e)
        {
            Stop();
            //
            if (_animationType == AnimationType.ScaleFactor) _frameDelta = 1;
            else if (_animationType == AnimationType.TimeIncrements) _frameDelta = (int)numIncrementStep.Value;
            else if (_animationType == AnimationType.Harmonic) _frameDelta = 1;
            else throw new NotSupportedException();
        }
        // Animation style
        private void rbAnimationStyle_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOnce.Checked) _animationStyle = AnimationStyle.Once;
            else if (rbLoop.Checked) _animationStyle = AnimationStyle.Loop;
            else _animationStyle = AnimationStyle.Swing;
        }
        private void numFramesPerSecond_ValueChanged(object sender, EventArgs e)
        {
            _fps = (int)numFramesPerSecond.Value;
        }
        // Color spectrum
        private void rbLimitChanged_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLimitsCurrentFrame.Checked) _colorSpectrumLimitsType = ColorSpectrumLimitsType.CurrentFrame;
            else _colorSpectrumLimitsType = ColorSpectrumLimitsType.AllFrames;
            UpdateAnimation();
        }
        // Acceleration
        private void cbGraphicsRam_CheckedChanged(object sender, EventArgs e)
        {
            _form.SetAnimationAcceleration(cbGraphicsRam.Checked);
            Stop();
            _updateAnimation = true;
        }
        // Movie options
        private void cbEncoderOptions_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEncoderOptions.Checked) cbSaveAsImages.Checked = false;
        }
        private void cbSaveAsImages_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSaveAsImages.Checked) cbEncoderOptions.Checked = false;
        }
        // Timer
        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            int firstFrame = (int)numFirstFrame.Value;
            int lastFrame = (int)numLastFrame.Value;

            _currFrame += _frameDelta;
            timerAnimation.Interval = (int)(1000f / _fps);

            if (_animationStyle == AnimationStyle.Once)
            {
                if (_currFrame <= firstFrame - 1) 
                {
                    if (_isFirstFrame) _currFrame = lastFrame;
                    else
                    {
                        _currFrame = firstFrame;
                        Stop();
                    }
                }
                else if (_currFrame >= lastFrame + 1)
                {
                    if (_isFirstFrame) _currFrame = firstFrame;
                    else
                    {
                        _currFrame = lastFrame;
                        Stop();
                    }
                }
            }
            else if (_animationStyle == AnimationStyle.Loop)
            {
                if (_currFrame <= firstFrame - 1) _currFrame = lastFrame;
                else if (_currFrame >= lastFrame + 1) _currFrame = firstFrame;
            }
            else if (_animationStyle == AnimationStyle.Swing)
            {
                if (_currFrame <= firstFrame - 1 || _currFrame >= lastFrame + 1)
                {
                    _frameDelta *= -1;
                    //_currFrme += 2 * _frameDelta;

                    if (_currFrame <= firstFrame - 1) _currFrame = firstFrame;
                    else if (_currFrame >= lastFrame + 1) _currFrame = lastFrame;
                }
            }

            _isFirstFrame = false;
            if (timerAnimation.Enabled) UpdateFrame();
        }

        
        // Methods                                                                                                                  
        public void PrepareForm(FrmMain form, Controller controller)
        {
            _form = form;
            _controller = controller;
            // This calls UpdateFrame
            InitializeVariables();
            //
            LegendSettings legend = _controller.Settings.Legend;
            gbColorSpectrumLimits.Enabled = legend.ColorSpectrum.MinMaxType == vtkControl.vtkColorSpectrumMinMaxType.Automatic;
            //
            if (!gbColorSpectrumLimits.Enabled)
            {
                rbLimitsCurrentFrame.Checked = false;
                rbLimitsAllFrames.Checked = false;
            }
            // Harmonic
            FieldData fieldData = _controller.CurrentFieldData;
            Field field = _controller.CurrentResult.GetField(fieldData);
            ComplexResultTypeEnum resultType = _form.GetComplexResultType();
            rbHarmonic.Enabled = field != null && field.Complex && resultType == ComplexResultTypeEnum.Real;
            if (rbHarmonic.Checked && !rbHarmonic.Enabled) rbScaleFactor.Checked = true;
            //
            _controller.SetSelectByToOff();
            //
            _doUpdateFrame = true;
        }
        private void OnHide()
        {
            Stop();
            _updateAnimation = true;    // this must be here since on hide/maximize of the main form the graphics is reset
            Form_ControlsEnable(true);
            _controller.DrawResults(false);
        }
        //
        private void AnimationTypeChanged(bool updateAnimation)
        {
            Stop();
            //
            if (rbScaleFactor.Checked)
            {
                lNumberOfFrames.Enabled = true;
                numNumberOfFrames.Enabled = true;
                lIncrementStep.Enabled = false;
                numIncrementStep.Enabled = false;
                lNumberOfAngles.Enabled = false;
                numNumberOfAngles.Enabled = false;
                //
                _animationType = AnimationType.ScaleFactor; // must be before SetNumberOfFrames
                //if (updateAnimation) _updateAnimation = true;
                SetNumberOfFrames((int)numNumberOfFrames.Value, true);
            }
            else if(rbTimeIncrements.Checked)
            {
                lNumberOfFrames.Enabled = false;
                numNumberOfFrames.Enabled = false;
                lIncrementStep.Enabled = true;
                numIncrementStep.Enabled = true;
                lNumberOfAngles.Enabled = false;
                numNumberOfAngles.Enabled = false;
                //
                _animationType = AnimationType.TimeIncrements;
            }
            else
            {
                lNumberOfFrames.Enabled = false;
                numNumberOfFrames.Enabled = false;
                lIncrementStep.Enabled = false;
                numIncrementStep.Enabled = false;
                lNumberOfAngles.Enabled = true;
                numNumberOfAngles.Enabled = true;
                //
                _animationType = AnimationType.Harmonic;
                //if (updateAnimation) _updateAnimation = true;
                SetNumberOfFrames((int)numNumberOfAngles.Value, true); // must be before SetNumberOfFrames
            }
            //
            NumIncrementStep_ValueChanged(null, null);
            //
            if (updateAnimation) UpdateAnimation();
        }
        public void UpdateAnimation()
        {
            Stop();
            _updateAnimation = true;
            UpdateFrame();
        }
        //
        private void InitializeVariables()
        {
            _countFrames = 0;
            numNumberOfFrames_ValueChanged(null, null);
            _currFrame = (int)numLastFrame.Value;

            AnimationTypeChanged(false);
            rbAnimationStyle_CheckedChanged(null, null);
            numFramesPerSecond_ValueChanged(null, null);

            cbGraphicsRam_CheckedChanged(null, null);

            rbLimitChanged_CheckedChanged(null, null);  // this calls UpdateFrame

            //_updateAnimation = true;
        }
        //
        private void PlayForward()
        {
            Stop();

            _isFirstFrame = true;
             _frameDelta = +Math.Abs(_frameDelta);
            timerAnimation.Start();
        }
        private void PlayBackward()
        {
            Stop();

            _isFirstFrame = true;
            _frameDelta = -Math.Abs(_frameDelta);
            timerAnimation.Start();
        }
        private void Stop()
        {
            timerAnimation.Stop();
        }
        private void SaveAsMovie()
        {
            try
            {
                if (_setSaveAsFolder)    // set the movie folder only the first time the saveFileDialog is shown
                {
                    saveFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(_controller.CurrentResult.FileName);
                    _setSaveAsFolder = false;
                }
                saveFileDialog1.FileName = System.IO.Path.GetFileNameWithoutExtension(_controller.CurrentResult.FileName) + ".avi";
                saveFileDialog1.Filter = "Avi files | *.avi";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileName;
                    string extension = System.IO.Path.GetExtension(saveFileDialog1.FileName);

                    if (extension != ".avi")
                    {
                        fileName = System.IO.Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + ".avi";
                        System.IO.Path.Combine(System.IO.Path.GetDirectoryName(saveFileDialog1.FileName), fileName);
                    }
                    else fileName = saveFileDialog1.FileName;

                    bool scalarRangeFromAllFrames = _colorSpectrumLimitsType == ColorSpectrumLimitsType.AllFrames;
                    _form.SaveAnimationAsAVI(fileName, new int[] { (int)numFirstFrame.Value - 1, (int)numLastFrame.Value - 1 }, _frameDelta,
                                             _fps, scalarRangeFromAllFrames, _animationStyle == AnimationStyle.Swing, cbEncoderOptions.Checked);

                    UpdateFrame();

                    UserControls.AutoClosingMessageBox.ShowInfo("Done", "The file was successfully created.", 2000);
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void SaveAsImages()
        {
            try
            {
                if (_setSaveAsFolder)    // set the movie folder only the first time the saveFileDialog is shown
                {
                    saveFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(_controller.CurrentResult.FileName);
                    _setSaveAsFolder = false;
                }
                saveFileDialog1.FileName = System.IO.Path.GetFileNameWithoutExtension(_controller.CurrentResult.FileName) + ".png";
                saveFileDialog1.Filter = "Png files | *.png";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileName;
                    string extension = System.IO.Path.GetExtension(saveFileDialog1.FileName);

                    if (extension != ".png")
                    {
                        fileName = System.IO.Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + ".png";
                        System.IO.Path.Combine(System.IO.Path.GetDirectoryName(saveFileDialog1.FileName), fileName);
                    }
                    else fileName = saveFileDialog1.FileName;

                    bool scalarRangeFromAllFrames = _colorSpectrumLimitsType == ColorSpectrumLimitsType.AllFrames;
                    _form.SaveAnimationAsImages(fileName, new int[] { (int)numFirstFrame.Value - 1, (int)numLastFrame.Value - 1 }, _frameDelta,
                                                scalarRangeFromAllFrames, _animationStyle == AnimationStyle.Swing);
                    //
                    UpdateFrame();
                    //
                    UserControls.AutoClosingMessageBox.ShowInfo("Done", "The file was successfully created.", 2000);
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void UpdateFrame()
        {
            if (!_doUpdateFrame) return;
            try
            {
                tbarFrameSelector.ValueChanged -= tbarFrameSelector_ValueChanged;
                numCurrFrame.ValueChanged -= numCurrFrame_ValueChanged;
                tbarFrameSelector.Value = _currFrame;
                numCurrFrame.Value = _currFrame;
                tbarFrameSelector.ValueChanged += tbarFrameSelector_ValueChanged;
                numCurrFrame.ValueChanged += numCurrFrame_ValueChanged;
                //
                bool timer;
                bool close = false;
                if (_updateAnimation)
                {
                    timer = timerAnimation.Enabled; // called from animation so stop the timer in case of an error
                    Stop();

                    if (_animationType == AnimationType.ScaleFactor)
                    {
                        if (!_controller.DrawScaleFactorAnimation(_numFrames)) close = true;
                    }
                    else if (_animationType == AnimationType.TimeIncrements)
                    {
                        _updateAnimation = false; // numNumOfFrames.Value causes this function to be called again
                        int numFrames; // create new variable since _numFrames gets changed by numNumOfFrames_ValueChanged
                        if (!_controller.DrawTimeIncrementAnimation(out numFrames)) close = true;
                        if (numFrames > 0) SetNumberOfFrames(numFrames, true); // this changes _updateAnimation = true
                    }
                    else if (_animationType == AnimationType.Harmonic)
                    {
                        if (!_controller.DrawHarmonicAnimation(_numFrames)) close = true;
                    }
                    else throw new NotSupportedException();
                    //
                    _updateAnimation = false;
                    //
                    if (close)
                    {
                        this.DialogResult = DialogResult.Abort;     // out of memory
                        if (Visible) Hide();
                        else OnHide();          // before it is shown - called from prepare form
                        return;
                    }
                    //
                    timerAnimation.Enabled = timer;
                }
                if (_countFrames >= 1000)
                {
                    _updateAnimation = true;
                    _countFrames = 0;
                }
                _form.SetAnimationFrame(_currFrame - 1, _colorSpectrumLimitsType == ColorSpectrumLimitsType.AllFrames);
                _countFrames++;
            }
            catch (Exception ex)
            {
                Stop();
                CaeGlobals.ExceptionTools.Show(this, ex);
                //_updateAnimation = false;
                //UpdateAnimation();
            }
        }

        
    }
}
