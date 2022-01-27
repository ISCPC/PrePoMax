using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserControls
{
    internal class NoHighlightRenderer : ToolStripProfessionalRenderer
    {
        public bool Render;
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (Render) base.OnRenderMenuItemBackground(e);
        }
    }
}
