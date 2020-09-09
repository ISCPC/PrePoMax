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
    public partial class ListViewWithSelection : ListView
    {
        private bool _disableMouse;


        public bool DisableMouse { get { return _disableMouse; } set { _disableMouse = value; } }


        // Keeps the selection at all times - even if clicked outside client area
        protected override void WndProc(ref Message m)
        {
            // Swallow mouse messages that are not in the client area
            if (m.Msg >= 0x201 && m.Msg <= 0x209)
            {
                // Eat left and right mouse clicks
                if (_disableMouse) return;
                //
                Point pos = new Point(m.LParam.ToInt32());
                var hit = this.HitTest(pos);
                switch (hit.Location)
                {
                    case ListViewHitTestLocations.AboveClientArea:
                    case ListViewHitTestLocations.BelowClientArea:
                    case ListViewHitTestLocations.LeftOfClientArea:
                    case ListViewHitTestLocations.RightOfClientArea:
                    case ListViewHitTestLocations.None:
                        return;
                }
            }
            base.WndProc(ref m);
        }

        public ListViewWithSelection()
        {
            InitializeComponent();
            //
            DoubleBuffered = true;
            //
            _disableMouse = false;
        }

        public void ResizeColumnHeaders()
        {
            for (int i = 0; i < this.Columns.Count - 1; i++)
                this.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
            this.Columns[this.Columns.Count - 1].Width = -2;
            this.Columns[this.Columns.Count - 1].Width -= 2;
        }

    }
}
