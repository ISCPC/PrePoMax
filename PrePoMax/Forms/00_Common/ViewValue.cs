using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PrePoMax.Forms
{
    class ViewDoubleValue
    {
        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_value < MinValue) _value = MinValue;
                if (_value > MaxValue) _value = MaxValue;
                _value = Math.Round(_value, NumOfDigits);
            }
        }

        [Browsable(false)]
        public byte NumOfDigits { get; set; }

        [Browsable(false)]
        public double MaxValue { get; set; }

        [Browsable(false)]
        public double MinValue { get; set; }

        public ViewDoubleValue(double value)
        {
            NumOfDigits = 15;
            MinValue = -double.MaxValue;
            MaxValue = double.MaxValue;
            Value = value;
        }
    }
}
