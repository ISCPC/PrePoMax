using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Linq.Expressions;

namespace CaeGlobals
{
    [Serializable]
    public static class MessageBoxes
    {
        // MessageBox
        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ShowWorkDirectoryError()
        {
            MessageBox.Show("The work directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ShowWarning(string text)
        {
            MessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static DialogResult ShowWarningQuestion(string text)
        {
            return MessageBox.Show(text, "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }
        public static DialogResult ShowQuestion(string caption, string text)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        }
    }
}
