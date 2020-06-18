using CaeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserControls
{
    [Serializable]
    public enum ViewGeometryModelResults
    {
        Geometry,
        Model,
        Results
    }
    //
    [Serializable]
    public class AdvisorPage
    {
        // Variables                                                                                                                
        protected int _minOffset;
        protected int _horozontalOffset;
        protected int _verticalOffset;
        //
        protected string _title;
        protected ViewType _associatedView;
        //
        protected List<AdvisorItem> _items;


        // Properties                                                                                                               
        public int HorozontalOffset
        {
            get { return _horozontalOffset; }
            set
            {
                _horozontalOffset = value;
                if (_horozontalOffset < 10) _horozontalOffset = 10;
            }
        }
        public int VerticalOffset
        {
            get { return _verticalOffset; }
            set
            {
                _verticalOffset = value;
                if (_verticalOffset < 0) _verticalOffset = 0;
            }
        }
        //
        public string Title { get { return _title; } set { _title = value; } }
        public ViewType AssociatedView { get { return _associatedView; } set { _associatedView = value; } }
        //
        public List<AdvisorItem> Items { get { return _items; } }


        // Events                                                                                                                   
        public event Action<ViewType> SetViewEvent;


        // Constructors                                                                                                             
        public AdvisorPage()
        {
            _minOffset = 10;
            _horozontalOffset = _minOffset;
            _verticalOffset = _minOffset;
            //
            _items = new List<AdvisorItem>();
        }


        // Event handlers                                                                                                           
        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender is LinkLabel ll && ll.Tag is AdvisorItemLinkLabel aill && aill != null)
            {
                SetViewEvent?.Invoke(_associatedView);
                aill.Activate();
            }
        }


        // Methods                                                                                                                  
        public Control[] GetControls()
        {
            List<Control> controls = new List<Control>();
            //
            foreach (var item in _items)
            {
                if (item is AdvisorItemLabel ail)
                {
                    Label label = new Label();
                    label.AutoSize = true;
                    label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    label.Left = ail.IndentLevel * _horozontalOffset;
                    label.Text = ail.Text;
                    label.Tag = ail;
                    
                    //
                    controls.Add(label);
                }
                else if (item is AdvisorItemLinkLabel aill)
                {
                    LinkLabel linkLabel = new LinkLabel();
                    linkLabel.AutoSize = true;
                    linkLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    linkLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                    linkLabel.Left = aill.IndentLevel * _horozontalOffset;
                    linkLabel.Text = aill.Text;
                    linkLabel.Tag = aill;
                    linkLabel.LinkClicked += LinkLabel_LinkClicked;
                    //
                    controls.Add(linkLabel);
                }
                else throw new NotSupportedException();
            }
            return controls.ToArray();
        }
        public void UpdateControlPositions(Control parentControl)
        {
            int y = 0;
            bool prevControlLinkLabel = false;
            foreach (var control in parentControl.Controls)
            {
                if (control is Label lab)
                {
                    if (lab is LinkLabel lnklab)
                    {
                        if (prevControlLinkLabel) y += 2;
                        else y += _verticalOffset;
                        //
                        lnklab.Top = y;
                        y += lnklab.Height;
                        //
                        prevControlLinkLabel = true;
                    }
                    else
                    {
                        y += _verticalOffset;
                        lab.Top = y;
                        lab.MaximumSize = new System.Drawing.Size(parentControl.Width - lab.Left, 0);
                        y += lab.Height;
                        //
                        prevControlLinkLabel = false;
                    }
                }
                else throw new NotSupportedException();
            }
            //
            parentControl.Height = y + _verticalOffset;
        }
    }
}
