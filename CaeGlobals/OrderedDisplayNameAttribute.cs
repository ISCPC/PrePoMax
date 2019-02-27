using System.ComponentModel;
using System.Text;

namespace CaeGlobals
{
    public class OrderedDisplayNameAttribute : DisplayNameAttribute
    {
        public OrderedDisplayNameAttribute(int position, int total, string displayName)
        {
            StringBuilder sb = new StringBuilder(displayName);

            for (int index = position; index < total; index++)
            {
                sb.Insert(0, '\t');
            }

            base.DisplayNameValue = sb.ToString();
        }
    }
}