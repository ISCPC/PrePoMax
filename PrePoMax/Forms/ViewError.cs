using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewError
    {
        private string _message;

        [CategoryAttribute("Error")]
        [DisplayName("Message")]
        [DescriptionAttribute("Error message.")]
        public string Message { get { return _message; } }

        public ViewError(string errorMessage)
        {
            _message = errorMessage;
        }
    }
}
