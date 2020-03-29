using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using CaeGlobals;

namespace UserControls
{
    public partial class FrmProperties : PrePoMaxChildForm
    {
        // Variables                                                                                                                
        protected Action _controller_SelectionClear;
        protected string _stepName;
        protected bool _hideOnClose;
        protected bool _addNew;
        protected bool _selectedPropertyGridItemChangedEventActive;


        // Properties                                                                                                               
        public Action SelectionClear { get { return _controller_SelectionClear; } set { _controller_SelectionClear = value; } }


        // Constructors                                                                                                             
        public FrmProperties()
            : this(2.0)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelRatio">Larger value means wider second column. Default = 2.0</param>
        public FrmProperties(double labelRatio)
        {
            InitializeComponent();

            _controller_SelectionClear = null;
            _hideOnClose = true;
            _addNew = true;
            _selectedPropertyGridItemChangedEventActive = false;

            propertyGrid.SetParent(this);   // for the Tab key to work
            propertyGrid.SetLabelColumnWidth(labelRatio);
        }


        // Event hadlers                                                                                                            
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            try
            {
                if (propertyGrid.SelectedGridItem.Value != null) OnPropertyGridPropertyValueChanged();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void propertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            try
            {
                if (_selectedPropertyGridItemChangedEventActive) OnPropertyGridSelectedGridItemChanged();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Apply(false);
                //
                this.DialogResult = DialogResult.OK;       // use OK to update the model tree selected item highlight
                if (_hideOnClose) Hide();
                else Close();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnOKAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                Apply(true);
                //
                if (_addNew) OnPrepareForm(_stepName, null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        protected virtual void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Cancel();
                //
                _controller_SelectionClear?.Invoke();
                this.DialogResult = DialogResult.Cancel;
                if (_hideOnClose) Hide();
                else Close();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void FrmProperties_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_hideOnClose && e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    _controller_SelectionClear?.Invoke();
                    this.DialogResult = DialogResult.Cancel;
                    Hide();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void FrmProperties_EnabledChanged(object sender, EventArgs e)
        {
            try
            {
                OnEnabledChanged();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }


        // Methods                                                                                                                  
        protected virtual bool OnPrepareForm(string stepName, string itemToEditName) { return true; }
        protected virtual void OnPropertyGridPropertyValueChanged()
        {
            propertyGrid.Refresh();
            _propertyItemChanged = true;
        }
        protected virtual void OnPropertyGridSelectedGridItemChanged()
        {
        }
        protected virtual void OnEnabledChanged() { }
        protected virtual void Apply(bool onOkAddNew) { }
        protected virtual void Cancel() { }


    }
}
