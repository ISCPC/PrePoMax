using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserControls
{
    public partial class AdvisorControl : UserControl
    {
        // Variables                                                                                                                
        protected LinkedListNode<AdvisorPage> _currentPageNode;
        protected LinkedList<AdvisorPage> _pages;


        // Properties                                                                                                               
        

        // Events                                                                                                                   
        public event Action<object, EventArgs> CloseButtonPressed;


        // Constructor                                                                                                              
        public AdvisorControl()
        {
            InitializeComponent();
            //
            _currentPageNode = null;
            _pages = new LinkedList<AdvisorPage>();
        }


        // Event handlers                                                                                                           
        private void WizardControl_Resize(object sender, EventArgs e)
        {
            UpdateCurentPagePositions();
        }
        //
        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                this.btnClose.Image = global::UserControls.Properties.Resources.Close_White;
            }
            catch
            { }
        }
        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                this.btnClose.Image = global::UserControls.Properties.Resources.Close;
            }
            catch
            { }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                CloseButtonPressed?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        //
        private void lnklabPrevious_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_currentPageNode != null && _currentPageNode.Previous != null)
            {
                _currentPageNode = _currentPageNode.Previous;
            }
            //
            ShowCurrentPage();
        }
        private void lnklabNext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_currentPageNode != null && _currentPageNode.Next != null)
            {
                _currentPageNode = _currentPageNode.Next;
            }
            //
            ShowCurrentPage();
        }

        // Methods                                                                                                                  
        public void AddPage(AdvisorPage wizardPage)
        {
            var page = _pages.AddLast(wizardPage);
            //
            if (_currentPageNode == null) _currentPageNode = page;
            //
            ShowCurrentPage();  // must be here
        }
        public void ShowCurrentPage()
        {
            if (_currentPageNode != null)
            {
                // Title
                labTitle.Text = _currentPageNode.Value.Title;
                // Add controls
                panPage.Controls.Clear();
                panPage.Controls.AddRange(_currentPageNode.Value.GetControls());
                // Update positions
                UpdateCurentPagePositions();
                // Previous/Next
                lnklabPrevious.Visible = _currentPageNode.Previous != null;
                lnklabNext.Visible = _currentPageNode.Next != null;
            }
        }
        public void ShowPageByIndex(int pageIndex)
        {
            if (pageIndex < 0) pageIndex = 0;
            if (pageIndex >= _pages.Count) pageIndex = _pages.Count - 1;
            //
            int count = 0;
            for (LinkedListNode<AdvisorPage> page = _pages.First; page != null && count++ != pageIndex; page = page.Next)
            {
                _currentPageNode = page;
            }
            //
            ShowCurrentPage();
        }
        private void UpdateCurentPagePositions()
        {
            if (_currentPageNode != null)
            {
                _currentPageNode.Value.UpdateControlPositions(panPage);
                //
                panLine.Top = panPage.Bottom;
                lnklabPrevious.Top = panLine.Bottom + 10;
                lnklabNext.Top = panLine.Bottom + 10;
            }
        }

        
    }
}
