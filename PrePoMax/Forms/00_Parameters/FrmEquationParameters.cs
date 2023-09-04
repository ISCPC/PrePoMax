using CaeGlobals;
using CaeMesh;
using CaeModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PrePoMax.Forms
{
    public partial class FrmParametersEditor : Form
    {
        // Variables                                                                                                                
        private Controller _controller;
        private int _cellRow;
        private int _cellCol;


        // Constructors                                                                                                             
        public FrmParametersEditor(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _cellRow = -1;
            _cellCol = -1;
        }


        // Event handlers                                                                                                           
        private void FrmParametersEditor_Load(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model != null)
                {
                    List<EquationParameter> parameters = new List<EquationParameter>();
                    foreach (var entry in _controller.Model.Parameters) parameters.Add(entry.Value.DeepClone());
                    // Binding
                    SetDataGridViewBinding(parameters);
                    //
                    UpdateNCalcParameters();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void FrmParametersEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _controller.UpdateNCalcParameters();
        }
        //
        private void dgvData_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpdateNCalcParameters();
        }
        private void dgvData_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateNCalcParameters();
        }
        //
        private void dgvData_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            _cellRow = e.RowIndex;
            _cellCol = e.ColumnIndex;
        }
        private void dgvData_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            try
            {
                if (e.RowIndex == _cellRow && e.ColumnIndex == _cellCol)
                {
                    BindingSource binding = dgvData.DataSource as BindingSource;
                    if (binding != null)
                    {
                        int count = 0;
                        MyNCalc.ExistingParameters = new Dictionary<string, double>();
                        List<EquationParameter> parameters = binding.DataSource as List<EquationParameter>;
                        //
                        foreach (var parameter in parameters)
                        {
                            if (count++ < e.RowIndex) MyNCalc.ExistingParameters.Add(parameter.Name, parameter.Value);
                            else break;
                        }
                        //
                        if (e.ColumnIndex == 0) parameters[e.RowIndex].Name = e.FormattedValue.ToString();
                        else if (e.ColumnIndex == 1) parameters[e.RowIndex].Equation.SetEquation(e.FormattedValue.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxes.ShowError(ex.Message);
                e.Cancel = true;
            }
            finally
            {
                if (e.RowIndex == _cellRow && e.ColumnIndex == _cellCol)
                {
                    UpdateNCalcParameters();
                    dgvData.Refresh();
                }
            }
        }
        private void dgvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _cellRow = -1;
            _cellCol = -1;
        }
        //
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                BindingSource binding = dgvData.DataSource as BindingSource;
                if (binding != null)
                {
                    EquationParameter existingParameter;
                    HashSet<string> parameterNames = new HashSet<string>();
                    List<EquationParameter> parameters = binding.DataSource as List<EquationParameter>;
                    //
                    foreach (var parameter in parameters)
                    {
                        parameterNames.Add(parameter.Name);
                        if (_controller.Model.Parameters.TryGetValue(parameter.Name, out existingParameter))
                        {
                            if (parameter.Equation == existingParameter.Equation) { }
                            // Replace
                            else _controller.ReplaceParameterCommand(existingParameter.Name, parameter);
                        }
                        // Add
                        else _controller.AddParameterCommand(parameter);
                    }
                    // Remove
                    string[] parameterNamesToRemove = _controller.Model.Parameters.Keys.Except(parameterNames).ToArray();
                    if (parameterNamesToRemove.Length > 0) _controller.RemoveParametersCommand(parameterNamesToRemove);
                    //
                    btnCancel_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }


        // Methods                                                                                                                  
        private void SetDataGridViewBinding(object data)
        {
            BindingSource binding = new BindingSource();
            binding.DataSource = data;
            dgvData.DataSource = binding; // bind datagridview to binding source - enables adding of new lines
            binding.ListChanged += Binding_ListChanged;
            //
            //int columnWidth;
            foreach (DataGridViewColumn column in dgvData.Columns)
            {
                if (column.Index == 1)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    column.MinimumWidth = 150;
                }
                //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //columnWidth = column.Width;
                //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                //column.Width = columnWidth * 2;
                //
                column.Width = 150;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomLeft;
            }
        }
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
        }
        private void UpdateNCalcParameters()
        {
            try
            {
                BindingSource binding = dgvData.DataSource as BindingSource;
                if (binding != null)
                {
                    List<EquationParameter> parameters = binding.DataSource as List<EquationParameter>;
                    MyNCalc.ExistingParameters = new Dictionary<string, double>();
                    foreach (var parameter in parameters)
                    {
                        MyNCalc.ExistingParameters.Add(parameter.Name, parameter.Value);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
        }

        
    }
}
