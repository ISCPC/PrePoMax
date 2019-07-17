using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

//using System.Windows.Forms.Design;

namespace PrePoMax
{
    //                                                                                                              
    // Walkthrough: Implementing a UI Type Editor                                                                   
    // https://msdn.microsoft.com/en-us/library/ms171840.aspx                                                       
    //                                                                                                              

    internal class ItemSetDataEditor : UITypeEditor
    {
        // Variables                                                                                                                
        private static FrmSelectItemSet _frmSelectItemSet;
        private static Form _parentForm;
        private ItemSetData _itemSetData;
        

        // Properties                                                                                                               
        public static FrmSelectItemSet SelectionForm { get { return _frmSelectItemSet; } set { _frmSelectItemSet = value; } }
        public static Form ParentForm { get { return _parentForm; } set { _parentForm = value; } }


        // Methods                                                                                                                  
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;

            if (provider != null)
            {
                editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }

            if (editorService != null)
            {
                if (_frmSelectItemSet != null)    // IsDisposed is true if the form was closed on Cancel or OK button
                {
                    _itemSetData = value as ItemSetData;

                    _frmSelectItemSet.ItemSetData = _itemSetData;
                    _frmSelectItemSet.Location = _parentForm.Location;
                    _frmSelectItemSet.VisibleChanged += _frmSelectItemSet_VisibleChanged;

                    _parentForm.Enabled = false;
                    _frmSelectItemSet.Show();
                }
            }
          
            return value;
        }
        void _frmSelectItemSet_VisibleChanged(object sender, EventArgs e)
        {
            // OK or Cancel are user events at closing the form
            if (_frmSelectItemSet.Visible == false && (_frmSelectItemSet.DialogResult == DialogResult.OK || _frmSelectItemSet.DialogResult == DialogResult.Cancel))
            {
                _parentForm.DialogResult = _frmSelectItemSet.DialogResult;
                _parentForm.Enabled = true;
                _frmSelectItemSet.VisibleChanged -= _frmSelectItemSet_VisibleChanged;
            }
        }
        

        // This method indicates to the design environment that 
        // the type editor will paint additional content in the 
        // LightShape entry in the PropertyGrid. 
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            //return true;
            return false;
        }

        // This method paints a graphical representation of the  
        // selected value of the LightShpae property. 
        public override void PaintValue(PaintValueEventArgs e)
        {
            try
            {
                //ItemSetData setData = (ItemSetData)e.Value;
                using (Pen p = Pens.Black)
                {
                    //e.Graphics.DrawLine(p, 1, 1, 10, 10);
                }
            }
            catch (Exception ex)
            {
            }
        }
        
    }
}
