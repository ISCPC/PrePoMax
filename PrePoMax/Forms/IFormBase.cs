using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax.Forms
{
    interface IFormBase
    {
        bool PrepareForm(string stepName, string itemToEdit);
        bool Visible { get; set; }
        string Text { get; set; }
        void Show();
        System.Drawing.Point Location { get; set; }
    }
}
