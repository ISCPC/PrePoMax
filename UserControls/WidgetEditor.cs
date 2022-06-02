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
    public partial class WidgetEditor : UserControl
    {
        // Variables                                                                                                                
        private int _pad = 6;
        private Rectangle _parentRectangle;
        private Point _initialLocation;


        // Properties                                                                                                               
        public override string Text { get { return wertbData.Text; } set { wertbData.Text = value; } }
        public new Point Location
        {
            get { return Location; }
            set
            {
                base.Location = value;
                _initialLocation = value;
            }
        }
        public Size MinSize { get { return wertbData.MinSize; } set { wertbData.MinSize = value; } }
        public Rectangle ParentRectangle
        {
            get { return _parentRectangle; }
            set
            {
                _parentRectangle = value;
                wertbData.MaxSize = new Size(_parentRectangle.Size.Width - _pad, _parentRectangle.Size.Height - _pad);
            }
        }


        // Event handlers                                                                                                           
        private void wertbData_SizeChanged(object sender, EventArgs e)
        {
            Size = new Size(wertbData.Width + _pad, wertbData.Height + _pad);
            Point location = _initialLocation;
            //
            if (_initialLocation.X + Width > _parentRectangle.Right) location.X = _parentRectangle.Right - Width;
            if (_initialLocation.Y + Height > _parentRectangle.Bottom) location.Y = _parentRectangle.Bottom - Height;
            //
            base.Location = location;
        }


        // Constructors                                                                                                             
        public WidgetEditor()
        {
            InitializeComponent();
        }

       
    }
}
