using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    public class FrdFileNameEditor : System.Windows.Forms.Design.FileNameEditor
    {
        public static string Filter = "";


        public FrdFileNameEditor()
        {
        }


        protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog)
        {
            base.InitializeDialog(openFileDialog);
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = Filter; // "Result files|*.frd";
        }
    }
}
