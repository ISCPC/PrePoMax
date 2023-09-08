﻿using CaeGlobals;
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
        private List<EquationParameter> _parameters;
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
            dgvData.ShowErrorMsg = false;
        }


        // Event handlers                                                                                                           
        private void FrmParametersEditor_Load(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model != null)
                {
                    _parameters = new List<EquationParameter>();
                    foreach (var entry in _controller.Model.Parameters) _parameters.Add(entry.Value.DeepClone());
                    // Binding
                    SetDataGridViewBinding(_parameters);
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
                    HashSet<string> existingNames = new HashSet<string>(MyNCalc.ExistingParameters.Keys);
                    existingNames.Remove(_parameters[e.RowIndex].Name);
                    //
                    UpdateNCalcParameters(e.RowIndex);
                    //
                    string value = e.FormattedValue.ToString();
                    EquationParameter ep = new EquationParameter();
                    // Test the name
                    if (e.ColumnIndex == 0)
                    {
                        if (existingNames.Contains(value))
                            throw new CaeException("The parameter named '" + value + "' already exists.");
                        //
                        ep.Name = value;
                    }
                    // Test the equation
                    else if (e.ColumnIndex == 1)
                    {
                        ep.Equation.SetEquation(value);
                        double tmp = ep.Value;
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
                double tmp;
                EquationParameter existingParameter;
                HashSet<string> parameterNames = new HashSet<string>();
                // Check all equation
                foreach (var parameter in _parameters)
                {
                    try
                    {
                        tmp = parameter.Value;
                    }
                    catch (Exception ex)
                    {
                        throw new CaeException("There is an error in the '" + parameter.Name + "' parameter equation.");
                    }
                }
                // Add parameters
                foreach (var parameter in _parameters)
                {
                    parameterNames.Add(parameter.Name);
                        
                    if (_controller.Model.Parameters.TryGetValue(parameter.Name, out existingParameter))
                    {
                        if (parameter.EquationStr == existingParameter.EquationStr) { }
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
        private void UpdateNCalcParameters(int upToRow = int.MaxValue)
        {
            int rowCount = 0;
            MyNCalc.ExistingParameters.Clear();
            foreach (var parameter in _parameters)
            {
                if (rowCount < upToRow)
                {
                    try { MyNCalc.ExistingParameters.Add(parameter.Name, parameter.Value); }
                    catch { }
                }
                rowCount++;
            }
            dgvData.Refresh();
        }

        
    }
}
