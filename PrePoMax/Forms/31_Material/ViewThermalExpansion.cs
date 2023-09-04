using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeModel;
using CaeGlobals;

namespace PrePoMax
{
   [Serializable]
    public class ViewThermalExpansion : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<ThermalExpansionDataPoint> _points;
        private EquationContainer _zeroTemperature;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Thermal Expansion"; }
        }
        //
        [Browsable(false)]
        public List<ThermalExpansionDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [CategoryAttribute("Data"),
        DisplayName("Thermal expansion"),
        DescriptionAttribute("The value of the thermal expansion coefficient.")]
        [TypeConverter(typeof(EquationThermalExpansionConverter))]
        public EquationString ThermalExpansion
        {
            get
            {
                if (_points != null && _points.Count > 0) return _points[0].ThermalExpansion.Equation;
                else return new EquationString("0");
            }
            set
            {
                if (_points != null && _points.Count > 0) _points[0].ThermalExpansion.Equation = value;
            }
        }
        //
        [CategoryAttribute("Data"),
        DisplayName("Zero temperature"),
        DescriptionAttribute("The value of the zero temperature after which the thermal expansion will start.")]
        [TypeConverter(typeof(EquationTemperatureConverter))]
        public EquationString ZeroTemperature
        {
            get { return _zeroTemperature.Equation; }
            set { _zeroTemperature.Equation = value; }
        }


        // Constructors                                                                                                             
        public ViewThermalExpansion(ThermalExpansion thermalExpansion, bool temperatureDependent)
        {
            _points = new List<ThermalExpansionDataPoint>();
            for (int i = 0; i < thermalExpansion.ThermalExpansionTemp.Length; i++)
            {
                _points.Add(new ThermalExpansionDataPoint(thermalExpansion.ThermalExpansionTemp[i][0],
                                                   thermalExpansion.ThermalExpansionTemp[i][1]));
            }
            _zeroTemperature = thermalExpansion.ZeroTemperature;
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            SetTemperatureDependence(temperatureDependent);
        }


        // Methods                                                                                                                  
        public override MaterialProperty GetBase()
        {
            int i = 0;
            EquationContainer[][] thermalExpansionTemp = new EquationContainer[_points.Count][];
            //
            foreach (ThermalExpansionDataPoint point in _points)
            {
                thermalExpansionTemp[i] = new EquationContainer[2];
                thermalExpansionTemp[i][0] = point.ThermalExpansion;
                thermalExpansionTemp[i][1] = point.Temperature;
                i++;
            }
            ThermalExpansion thermalExpansion = new ThermalExpansion(thermalExpansionTemp);
            thermalExpansion.ZeroTemperature = _zeroTemperature;
            //
            return thermalExpansion;
        }
        public void SetTemperatureDependence(bool temperatureDependent)
        {
             DynamicCustomTypeDescriptor.GetProperty(nameof(ThermalExpansion)).SetIsBrowsable(!temperatureDependent);
        }
    }
}
