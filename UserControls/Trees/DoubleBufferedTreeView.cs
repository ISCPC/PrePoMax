using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.Generic;

namespace UserControls
{
    public class BufferedTreeView : TreeView
    {
        // double buffer
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }
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


        // Properties                                                                   
        public Color HighLightSelectedTextColor { get { return _highlightSelectedTextColor; } set { _highlightSelectedTextColor = value; } }
        public Color HighLightSelectedColor { get { return _highlightSelectedColor; } set { _highlightSelectedColor = value; } }
        public Color HighLightDeselectedTextColor { get { return _highlightDeselectedTextColor; } set { _highlightDeselectedTextColor = value; } }
        public Color HighLightDeselectedColor { get { return _highlightDeselectedColor; } set { _highlightDeselectedColor = value; } }
        public Color HighlightErrorColor { get { return _highlightErrorColor; } set { _highlightErrorColor = value; } }

        // Constructor                                                                  
        public BufferedTreeView()
        {
            this.DrawMode = TreeViewDrawMode.OwnerDrawText;
            ResetHighlightColors();
        }

        // Methods                                                                      
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            TreeNodeStates treeState = e.State;
            Font treeFont = e.Node.NodeFont ?? e.Node.TreeView.Font;

            // Colors
            Color foreColor = e.Node.ForeColor;
            string strDeselectedColor = @"#6B6E77";
            string strSelectedColor = @"#94C7FC";
            Color selectedColor = System.Drawing.ColorTranslator.FromHtml(strSelectedColor);
            Color deselectedColor = System.Drawing.ColorTranslator.FromHtml(strDeselectedColor);

            selectedColor = HighLightSelectedColor;
            deselectedColor = HighLightDeselectedColor;

            // New brush
            SolidBrush selectedTreeBrush = new SolidBrush(selectedColor);
            SolidBrush deselectedTreeBrush = new SolidBrush(deselectedColor);

            if (foreColor == _highlightErrorColor) selectedTreeBrush = new SolidBrush(_highlightErrorColor);

            // Set default font color
            if (foreColor == Color.Empty)
                foreColor = e.Node.TreeView.ForeColor;

            // Change bounds
            Rectangle rect = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top, e.Bounds.Width - 2, e.Bounds.Height);

            // Draw bounding box and fill
            if (e.Node == e.Node.TreeView.SelectedNode)
            {
                // Change fore color
                foreColor = SystemColors.HighlightText;
                // Use appropriate brush depending on if the tree has focus
                if (this.Focused)
                {
                    e.Graphics.FillRectangle(selectedTreeBrush, rect);
                    ControlPaint.DrawFocusRectangle(e.Graphics, rect, foreColor, SystemColors.Highlight);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, foreColor, TextFormatFlags.VerticalCenter);
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
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, System.Drawing.Color.Black, TextFormatFlags.VerticalCenter);
                }
                else
                {
                    e.Graphics.FillRectangle(SystemBrushes.Window, rect);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rect, foreColor, TextFormatFlags.VerticalCenter);
                }
            }
        }

        public void ResetHighlightColors()
        {
            _highlightSelectedTextColor = SystemColors.HighlightText;
            _highlightSelectedColor = SystemColors.Highlight;

            _highlightDeselectedTextColor = SystemColors.ScrollBar;
            _highlightDeselectedColor = SystemColors.Highlight;

            _highlightErrorColor = Color.Red;
        }

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
