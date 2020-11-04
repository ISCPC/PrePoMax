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

    internal class SinglePointDataEditor : UITypeEditor
    {
        // Variables                                                                                                                
        private static Form _parentForm;
        private static Controller _controller;
        private ItemSetData _itemSetData;
        

        // Properties                                                                                                               
        public static Form ParentForm { get { return _parentForm; } set { _parentForm = value; } }
        public static Controller Controller { get { return _controller; } set { _controller = value; } }


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
                _itemSetData = value as ItemSetData;
                _parentForm.Enabled = false;

                _controller.SelectBy = CaeGlobals.vtkSelectBy.QueryNode;
                _controller.Selection.SelectItem = CaeGlobals.vtkSelectItem.Node;
                _controller.ClearSelectionHistoryAndCallSelectionChanged();
            }
          
            return value;
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
