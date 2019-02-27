using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections;

namespace UserControls
{
    public class MultiSelectBufferedTreeView : TreeView
    {
        //  Multi-selection                                                                         
        //  https://www.codeproject.com/Articles/2756/C-TreeView-with-multiple-selection            
        //                                                                                          

        // Pinvoke:
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);


        // Variables                                                                    
        private Color _highlightSelectedTextColor;
        private Color _highlightSelectedColor;
        private Color _highlightDeselectedTextColor;
        private Color _highlightDeselectedColor;
        private Color _highlightErrorColor;
        private ArrayList m_coll;
        private TreeNode m_lastNode, m_firstNode;
        private bool _paintOn;


        // Properties                                                                   
        public Color HighLightSelectedTextColor { get { return _highlightSelectedTextColor; } set { _highlightSelectedTextColor = value; } }
        public Color HighLightSelectedColor { get { return _highlightSelectedColor; } set { _highlightSelectedColor = value; } }
        public Color HighLightDeselectedTextColor { get { return _highlightDeselectedTextColor; } set { _highlightDeselectedTextColor = value; } }
        public Color HighDeselectedLightColor { get { return _highlightDeselectedColor; } set { _highlightDeselectedColor = value; } }
        public Color HighlightErrorColor { get { return _highlightErrorColor; } set { _highlightErrorColor = value; } }
        public ArrayList SelectedNodes
        {
            get
            {
                return m_coll;
            }
            set
            {
                m_coll.Clear();
                m_coll = value;
                this.Invalidate();
            }
        }
        public bool PaintOn
        {
            get { return _paintOn; }
            set
            {
                _paintOn = value;

                if (_paintOn)
                {
                    this.DrawMode = TreeViewDrawMode.OwnerDrawText;
                    Invalidate();
                }
                else
                {
                    this.DrawMode = TreeViewDrawMode.Normal;
                }
            }
        }


        // Constructor                                                                  
        public MultiSelectBufferedTreeView()
        {
            // Enable default double buffering processing (DoubleBuffered returns true)
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            this.DrawMode = TreeViewDrawMode.OwnerDrawText;
            ResetHighlightColors();

            m_coll = new ArrayList();
            _paintOn = true;
        }

        // Methods                                                                      

        // Double buffer
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }


        // Highlight
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (!_paintOn)
                return;

            TreeNodeStates treeState = e.State;
            Font treeFont = e.Node.NodeFont ?? e.Node.TreeView.Font;

            // Colors
            Color foreColor = e.Node.ForeColor;
            string strDeselectedColor = @"#6B6E77";
            string strSelectedColor = @"#94C7FC";
            Color selectedColor = System.Drawing.ColorTranslator.FromHtml(strSelectedColor);
            Color deselectedColor = System.Drawing.ColorTranslator.FromHtml(strDeselectedColor);

            selectedColor = HighLightSelectedColor;
            deselectedColor = HighLightDeselectedTextColor;

            // New brush
            SolidBrush selectedTreeBrush = new SolidBrush(selectedColor);
            SolidBrush deselectedTreeBrush = new SolidBrush(deselectedColor);

            if (foreColor == _highlightErrorColor) selectedTreeBrush = new SolidBrush(_highlightErrorColor);

            // Set default font color
            if (foreColor == Color.Empty)
                foreColor = e.Node.TreeView.ForeColor;

            // Change bounds

            //Rectangle rect = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top, e.Bounds.Width - 2, e.Bounds.Height);

            Rectangle rect = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top, e.Bounds.Width - 2, e.Bounds.Height);

            // Draw bounding box and fill
            //if (e.Node == e.Node.TreeView.SelectedNode)
            if (m_coll.Contains(e.Node))
            {
                // Change fore color
                foreColor = SystemColors.HighlightText;                
                // Use appropriate brush depending on if the tree has focus
                if (this.Focused)
                {
                    e.Graphics.FillRectangle(selectedTreeBrush, rect);
                    ControlPaint.DrawFocusRectangle(e.Graphics, rect, foreColor, SystemColors.Highlight);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect,foreColor, TextFormatFlags.VerticalCenter);
                }
                else
                {
                    e.Graphics.FillRectangle(deselectedTreeBrush, rect);
                    ControlPaint.DrawFocusRectangle(e.Graphics, rect, foreColor, SystemColors.Highlight);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, foreColor, TextFormatFlags.VerticalCenter);
                }
            }
            else
            {
                if ((e.State & TreeNodeStates.Hot) == TreeNodeStates.Hot)                
                {
                    e.Graphics.FillRectangle(SystemBrushes.Window, rect);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, Color.Black, TextFormatFlags.VerticalCenter);
                }
                else
                {
                    e.Graphics.FillRectangle(SystemBrushes.Window, rect);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, foreColor, TextFormatFlags.VerticalCenter);
                }
            }
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + "     " + m_coll.Count);
        }
        public void ResetHighlightColors()
        {
            _highlightSelectedTextColor = SystemColors.HighlightText;
            _highlightSelectedColor = SystemColors.Highlight;

            _highlightDeselectedTextColor = SystemColors.ScrollBar;
            _highlightDeselectedColor = SystemColors.Highlight;

            _highlightErrorColor = Color.Red;
        }

        // Multi-select
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (HitTest(e.Location).Node == null)
            {
                m_coll.Clear();
                Invalidate();
            }
        }
        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);

            if (e.Button == MouseButtons.Left || (e.Button == MouseButtons.Right && m_coll.Count <= 1 && ModifierKeys != Keys.Shift && ModifierKeys != Keys.Control))
            {
                UpdateSelectedNodes(e.Node);
            }
        }
        protected bool isParent(TreeNode parentNode, TreeNode childNode)
        {
            if (parentNode == childNode)
                return true;

            TreeNode n = childNode;
            bool bFound = false;
            while (!bFound && n != null)
            {
                n = n.Parent;
                bFound = (n == parentNode);
            }
            return bFound;
        }
        private void UpdateSelectedNodes(TreeNode node)
        {           
            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl)
            {
                if (!m_coll.Contains(node)) // new node ?
                {
                    m_coll.Add(node);
                }
                else  // not new, remove it from the collection
                {
                    m_coll.Remove(node);
                }
            }
            else
            {
                if (bShift)
                {
                    if (m_firstNode == null) m_firstNode = node;

                    m_coll.Clear();
                    m_coll.Add(m_firstNode);

                    Queue myQueue = new Queue();

                    TreeNode uppernode = m_firstNode;
                    TreeNode bottomnode = node;

                    // case 1 : begin and end nodes are parent
                    bool bParent = isParent(m_firstNode, node);
                    if (!bParent)
                    {
                        bParent = isParent(bottomnode, uppernode);
                        if (bParent) // swap nodes
                        {
                            TreeNode t = uppernode;
                            uppernode = bottomnode;
                            bottomnode = t;
                        }
                    }
                    if (bParent)
                    {
                        TreeNode n = bottomnode;
                        while (n != uppernode.Parent)
                        {
                            if (!m_coll.Contains(n)) // new node ?
                                myQueue.Enqueue(n);

                            n = n.Parent;
                        }
                    }
                    // case 2 : nor the begin nor the
                    // end node are descendant one another
                    else
                    {
                        // are they siblings ?                 

                        if ((uppernode.Parent == null && bottomnode.Parent == null)
                              || (uppernode.Parent != null &&
                              uppernode.Parent.Nodes.Contains(bottomnode)))
                        {
                            int nIndexUpper = uppernode.Index;
                            int nIndexBottom = bottomnode.Index;
                            if (nIndexBottom < nIndexUpper) // reversed?
                            {
                                TreeNode t = uppernode;
                                uppernode = bottomnode;
                                bottomnode = t;
                                nIndexUpper = uppernode.Index;
                                nIndexBottom = bottomnode.Index;
                            }

                            TreeNode n = uppernode;
                            while (nIndexUpper <= nIndexBottom)
                            {
                                if (!m_coll.Contains(n)) // new node ?
                                    myQueue.Enqueue(n);

                                n = n.NextNode;

                                nIndexUpper++;
                            } // end while

                        }
                        else
                        {
                            if (!m_coll.Contains(uppernode))
                                myQueue.Enqueue(uppernode);
                            if (!m_coll.Contains(bottomnode))
                                myQueue.Enqueue(bottomnode);
                        }

                    }

                    m_coll.AddRange(myQueue);

                    // let us chain several SHIFTs if we like it
                    m_firstNode = node;

                } // end if m_bShift
                else
                {
                    // in the case of a simple click, just add this item
                    if (m_coll != null && m_coll.Count > 0)
                    {
                        m_coll.Clear();
                    }
                    m_coll.Add(node);
                }
            }

            m_lastNode = node;
            m_firstNode = node; // store begin of shift sequence
        }
    }
}
