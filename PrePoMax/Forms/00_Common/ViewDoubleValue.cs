using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class ViewDoubleValue
    {
        // Variables                                                                                                                
        private double _value;
        private string _presetValue;
        OrderedDictionary<string, double> _presetValues;
        DynamicCustomTypeDescriptor _dctd;


        // Properties                                                                                                               
        [Category("Value")]
        [OrderedDisplayName(0, 1, "Preset values")]
        public string PresetValue 
        { 
            get { return _presetValue; }
            set 
            {
                if (_presetValue != value)
                {
                    _presetValue = value;
                    //
                    if (_presetValue != "User defined") _value = _presetValues[_presetValue];
                }
            }
        }
        //
        [Category("Value")]
        public double Value
        {
            get { return _value; }
            set
            {
                //if (_value != value)
                {
                    _value = value;
                    if (_value < MinValue) _value = MinValue;
                    if (_value > MaxValue) _value = MaxValue;
                    _value = Math.Round(_value, NumOfDigits);
                    //
                    if (_presetValues != null)
                    {
                        _presetValue = "User defined";
                        foreach (var entry in _presetValues)
                        {
                            if (value == entry.Value)
                            {
                                _presetValue = entry.Key;
                                break;
                            }
                        }
                    }
                }
            }
        }
        //
        [Browsable(false)]
        public byte NumOfDigits { get; set; }
        //
        [Browsable(false)]
        public double MaxValue { get; set; }
        //
        [Browsable(false)]
        public double MinValue { get; set; }
        //
        [Browsable(false)]
        public OrderedDictionary<string, double> PresetValues 
        { 
            get { return _presetValues; } 
            set 
            {
                _presetValues = value;
                //
                CustomPropertyDescriptor cpd;
                if (_presetValues == null)
                {
                    cpd = _dctd.GetProperty(nameof(PresetValue));
                    cpd.SetIsBrowsable(false);
                }
                else
                {
                    List<string> allPresetValues = new List<string>();
                    allPresetValues.Add("User defined");
                    allPresetValues.AddRange(_presetValues.Keys);
                    _dctd.PopulateProperty(nameof(PresetValue), allPresetValues.ToArray());
                }
                //
                Value = _value;
            } 
        }
        //
        [Browsable(false)]
        public void SetDisplayName(string displayName)
        {
            CustomPropertyDescriptor cpd = _dctd.GetProperty(nameof(Value));
            cpd.SetDisplayName("\t" + displayName);
        }
        //
        [Browsable(false)]
        public void SetDescription(string description)
        {
            CustomPropertyDescriptor cpd = _dctd.GetProperty(nameof(Value));
            cpd.SetDescription(description);
        }
        //
        [Browsable(false)]
        public void SetTypeConverter(TypeConverter typeConverter)
        {
            CustomPropertyDescriptor cpd = _dctd.GetProperty(nameof(Value));
            cpd.SetTypeConverter(typeConverter);
        }


        // Constructors                                                                                                             
        public ViewDoubleValue()
            : this(0, null)
        { }
        public ViewDoubleValue(double value, OrderedDictionary<string, double> presetValues)
        {
            _dctd = ProviderInstaller.Install(this);
            //
            NumOfDigits = 15;
            MinValue = -double.MaxValue;
            MaxValue = double.MaxValue;
            Value = value;
            PresetValues = presetValues;    // needs _dctd
        }
    }
}
