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
    public partial class AnnotationEditor : UserControl
    {
        // Variables                                                                                                                
        private int _pad = 6;
        private Rectangle _parentArea;
        private Point _initialLocation;


        // Properties                                                                                                               
        public Size MinSize { get { return wertbData.MinSize; } set { wertbData.MinSize = value; } }
        public Rectangle ParentArea
        {
            get { return _parentArea; }
            set
            {
                _parentArea = value;
                wertbData.MaxSize = new Size(_parentArea.Size.Width - _pad, _parentArea.Size.Height - _pad);
            }
        }
        public new Point Location
        {
            get { return Location; }
            set
            {
                base.Location = value;
                _initialLocation = value;
            }
        }
        public override string Text
        {
            get { return wertbData.Text; }
            set
            {
                wertbData.Text = value;
                wertbData_SizeChanged(null, null);
            }
        }


        // Event handlers                                                                                                           
        private void wertbData_SizeChanged(object sender, EventArgs e)
        {
            Size = new Size(wertbData.Width + _pad, wertbData.Height + _pad);
            Point location = _initialLocation;
            //
            if (_initialLocation.X + Width > _parentArea.Right) location.X = _parentArea.Right - Width;
            if (_initialLocation.Y + Height > _parentArea.Bottom) location.Y = _parentArea.Bottom - Height;
            //
            base.Location = location;
        }


        // Constructors                                                                                                             
        public AnnotationEditor()
        {
            InitializeComponent();
        }

        // Methods                                                                                                                  
        public bool IsOrContainsControl(Control control)
        {
            if (this == control) return true;
            else if (pBorder == control) return true;
            else if (wertbData == control) return true;
            else return false;
        }
    }
}
