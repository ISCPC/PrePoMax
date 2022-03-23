using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.Generic;

namespace UserControls
{
    public class BufferedTreeView : TreeView
    {
        // Pinvoke:
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);


        // Variables                                                                    
        private Color _selectedFocusedForeColor;
        private Color _selectedFocusedBackColor;
        private Color _selectedUnfocusedForeColor;
        private Color _selectedUnfocusedBackColor;
        private Color _deselectedForeColor;
        private Color _deselectedBackColor;
        private Color _highlightErrorColor;


        // Properties                                                                   
        public Color ColorSelectedFocusedFore { get { return _selectedFocusedForeColor; } set { _selectedFocusedForeColor = value; } }
        public Color ColorSelectedFocusedBack { get { return _selectedFocusedBackColor; } set { _selectedFocusedBackColor = value; } }
        public Color ColorSelectedUnfocusedFore { get { return _selectedUnfocusedForeColor; } set { _selectedUnfocusedForeColor = value; } }
        public Color ColorSelectedUnfocusedBack { get { return _selectedUnfocusedBackColor; } set { _selectedUnfocusedBackColor = value; } }
        public Color ColorDeselectedFore { get { return _deselectedForeColor; } set { _deselectedForeColor = value; } }
        public Color ColorDeselectedBack { get { return _deselectedBackColor; } set { _deselectedBackColor = value; } }
        public Color ColorHighlightError { get { return _highlightErrorColor; } set { _highlightErrorColor = value; } }


        // Constructor                                                                  
        public BufferedTreeView()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.DrawMode = TreeViewDrawMode.OwnerDrawText;
            //
            ResetHighlightColors();
        }


        // Overrides                                                                    
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }
        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            BeginUpdate();
            //
            base.OnBeforeCollapse(e);
        }
        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            EndUpdate();
            //
            base.OnAfterCollapse(e);
        }
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            BeginUpdate();
            //
            base.OnBeforeExpand(e);
        }
        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            EndUpdate();
            //
            base.OnAfterExpand(e);
        }


        // Methods                                                                      
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            Font treeFont = e.Node.NodeFont ?? e.Node.TreeView.Font;
            // New brush
            SolidBrush selectedFocusedTreeBrush = new SolidBrush(_selectedFocusedBackColor);
            SolidBrush selectedUnfocusedTreeBrush = new SolidBrush(_selectedUnfocusedBackColor);
            SolidBrush deselectedTreeBrush = new SolidBrush(_deselectedBackColor);
            // Colors
            Color foreColor = e.Node.ForeColor;
            if (foreColor == Color.Empty) foreColor = e.Node.TreeView.ForeColor;
            if (foreColor == _highlightErrorColor)
            {
                selectedFocusedTreeBrush = new SolidBrush(_highlightErrorColor);
                selectedUnfocusedTreeBrush = new SolidBrush(_highlightErrorColor);
            }
            // Change bounds
            Rectangle rect = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top, e.Bounds.Width - 2, e.Bounds.Height);

            //rect = new Rectangle(e.Bounds.Left , e.Bounds.Top, e.Bounds.Width , e.Bounds.Height);

            // Draw bounding box and fill
            if (e.Node == e.Node.TreeView.SelectedNode)
            {
                // Use appropriate brush depending on if the tree has focus
                if (this.Focused)
                {
                    foreColor = _selectedFocusedForeColor;
                    //
                    e.Graphics.FillRectangle(selectedFocusedTreeBrush, rect);
                    ControlPaint.DrawFocusRectangle(e.Graphics, rect, _selectedFocusedBackColor, _selectedFocusedBackColor);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, foreColor, TextFormatFlags.VerticalCenter);
                }
                else
                // Unfocused
                {
                    foreColor = _selectedUnfocusedForeColor;
                    //
                    e.Graphics.FillRectangle(selectedUnfocusedTreeBrush, rect);
                    ControlPaint.DrawFocusRectangle(e.Graphics, rect, _selectedUnfocusedBackColor, _selectedUnfocusedBackColor);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, foreColor, TextFormatFlags.VerticalCenter);
                }
            }
            else
            {
                // Change fore color
                foreColor = _deselectedForeColor;
                //
                if ((e.State & TreeNodeStates.Hot) == TreeNodeStates.Hot)
                {
                    e.Graphics.FillRectangle(deselectedTreeBrush, rect);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, foreColor, TextFormatFlags.VerticalCenter);
                }
                else
                {
                    e.Graphics.FillRectangle(deselectedTreeBrush, rect);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, foreColor, TextFormatFlags.VerticalCenter);
                }
            }
        }
        //
        public void ResetHighlightColors()
        {
            _selectedFocusedForeColor = SystemColors.HighlightText; // white
            _selectedFocusedBackColor = SystemColors.Highlight;     // blue
            //
            _selectedUnfocusedForeColor = SystemColors.ControlText; // black
            _selectedUnfocusedBackColor = SystemColors.Control;     // light gray
            //
            _deselectedForeColor = SystemColors.ControlText;        // black
            _deselectedBackColor = SystemColors.Window;             // white
            //
            _highlightErrorColor = Color.Red;
        }
        //
        public bool[] GetTreeExpandCollapseState()
        {
            List<bool> states = new List<bool>();
            foreach (TreeNode node in Nodes)
            {
                GetNodeExpandCollapseState(node, states);
            }

            return states.ToArray();
        }
        private void GetNodeExpandCollapseState(TreeNode node, List<bool> states)
        {
            states.Add(node.IsExpanded);
            foreach (TreeNode child in node.Nodes)
            {
                GetNodeExpandCollapseState(child, states);
            }
        }
        public void SetTreeExpandCollapseState(bool[] states)
        {
            try
            {
                int id = 0;
                foreach (TreeNode node in Nodes)
                {
                    SetNodeExpandCollapseState(node, states, ref id);
                }
            }
            catch (Exception)
            {
            }

        }
        private void SetNodeExpandCollapseState(TreeNode node, bool[] states, ref int id)
        {
            if (states[id]) node.Expand();
            else node.Collapse();
            id++;

            foreach (TreeNode child in node.Nodes)
            {
                SetNodeExpandCollapseState(child, states, ref id);
            }
        }
        
    }
}
