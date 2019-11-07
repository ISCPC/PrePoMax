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
    public partial class FrmGetValue : Form
    {
        // Variables                                                                                                                
        private ViewDoubleValue _viewValue;
        private double _labelRatio = 2.0; // larger value means wider second column


        // Properties                                                                                                               
        public double Value 
        {
            get { return _viewValue.Value; }
            set { _viewValue.Value = value; } 
        }
        public byte NumOfDigits { get { return _viewValue.NumOfDigits; } set { _viewValue.NumOfDigits = value; } }
        public double MaxValue { get { return _viewValue.MaxValue; } set { _viewValue.MaxValue = value; } }
        public double MinValue { get { return _viewValue.MinValue; } set { _viewValue.MinValue = value; } }


        // Constructors                                                                                                             
        public FrmGetValue(double value)
        {
            InitializeComponent();
            _viewValue = new ViewDoubleValue(value);

            propertyGrid.SetParent(this);   // for the Tab key to work
            propertyGrid.SetLabelColumnWidth(_labelRatio);
        }


        // Event hadlers                                                                                                            
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
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
