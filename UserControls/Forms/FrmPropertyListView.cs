using CaeGlobals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserControls
{
    public partial class FrmPropertyListView : FrmProperties
    {
        // Variables                                                                                                                
        protected bool _lvTypesSelectedIndexChangedEventActive;


        // Constructors                                                                                                             
        public FrmPropertyListView()
            :this(2.0)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelRatio">Larger value means wider second column. Default = 2.0</param>
        public FrmPropertyListView(double labelRatio)
            :base(labelRatio)
        {
            InitializeComponent();

            _lvTypesSelectedIndexChangedEventActive = true;
        }

        private void lvTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_lvTypesSelectedIndexChangedEventActive) OnListViewTypeSelectedIndexChanged();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        // Methods                                                                                                                  
        protected virtual void OnListViewTypeSelectedIndexChanged()
        {

        }

      
    }
}
