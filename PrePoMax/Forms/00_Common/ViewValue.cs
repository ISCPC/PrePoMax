using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax.Forms
{
    class ViewValue<T>
    {
        public T Value { get; set; }

        public ViewValue(T initialValue)
        {
            Value = initialValue;
        }
    }
}
