using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using System.Reflection;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax.Forms
{
    public partial class FrmGetInteger : Form
    {
        // Variables                                                                                                                
        private ViewValue<int> _viewValue;
        private double _labelRatio = 2.0; // larger value means wider second column


        // Properties                                                                                                               
        public int Value 
        {
            get { return _viewValue.Value; }
            set { _viewValue.Value = value; } 
        }


        // Constructors                                                                                                             
        public FrmGetInteger()
        {
            InitializeComponent();
            _viewValue = new ViewValue<int>(1);

            propertyGrid.SetParent(this);   // for the Tab key to work
            propertyGrid.SetLabelColumnWidth(_labelRatio);
        }


        // Event hadlers                                                                                                            
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
            if (_viewValue.Value < 1) _viewValue.Value = 1;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
     

        // Methods                                                                                                                  
        public void PrepareForm(string title, string valueName, string valueDescription)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized

            Text = title;

            DynamicCustomTypeDescriptor _dctd = ProviderInstaller.Install(_viewValue);

            CustomPropertyDescriptor cpd = _dctd.GetProperty("Value");
            cpd.SetCategory("Value");
            cpd.SetDisplayName(valueName);
            cpd.SetDescription(valueDescription);

            propertyGrid.SelectedObject = _viewValue;
            propertyGrid.Select();
        }

    

       

       

        


     

       
      
    }
}
