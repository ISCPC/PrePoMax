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
        protected bool _selectPropertyGrid;
        protected int _preselectIndex;
        protected bool _firstTime;

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
            //
            _lvTypesSelectedIndexChangedEventActive = true;
            _preselectIndex = -1;
            _firstTime = true;
        }


        // Event handlers                                                                                                           
        private void FrmPropertyListView_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && _firstTime)
            {
                _firstTime = false;
                OnListViewTypeSelectedIndexChanged();
            }
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
        private void lvTypes_MouseUp(object sender, MouseEventArgs e)
        {
            OnListViewTypeMouseUp();
        }


        // Methods                                                                                                                  
        public override bool PrepareForm(string stepName, string itemToEditName)
        {
            lvTypes.Enabled = true;
            //
            bool result = OnPrepareForm(stepName, itemToEditName);
            //
            if (_preselectIndex >= 0 && _preselectIndex < lvTypes.Items.Count)
            {
                lvTypes.Items[_preselectIndex].Selected = true;
                lvTypes.Enabled = false;
                _preselectIndex = -1;
            }
            //
            return result;
        }
        protected virtual void OnListViewTypeSelectedIndexChanged()
        {

        }
        protected virtual void OnListViewTypeMouseUp()
        {
            propertyGrid.Select();
        }
        public void PreselectListViewItem(int index)
        {
            _preselectIndex = index;
        }

        
    }
}
