using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeModel;

namespace PrePoMax
{
   [Serializable]
    public class ViewThermalExpansion : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<ThermalExpansionDataPoint> _points;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Thermal expansion"; }
        }
        //
        [Browsable(false)]
        public override MaterialProperty Base
        {
            get
            {
                int i = 0;
                double[][] thermalExpansionTemp = new double[_points.Count][];
                //
                foreach (ThermalExpansionDataPoint point in _points)
                {
                    thermalExpansionTemp[i] = new double[2];
                    thermalExpansionTemp[i][0] = point.ThermalExpansion;
                    thermalExpansionTemp[i][1] = point.Temperature;
                    i++;
                }
                ThermalExpansion thermalExpansion = new ThermalExpansion(thermalExpansionTemp);
                //
                return thermalExpansion;
            }
        }
        //
        [Browsable(false)]
        public List<ThermalExpansionDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [CategoryAttribute("Data"),
        DisplayName("Thermal expansion"),
        DescriptionAttribute("The value of the thermal expansion coefficient.")]
        [TypeConverter(typeof(CaeGlobals.StringThermalExpansionConverter))]
        public double ThermalExpansion
        {
            get
            {
                if (_points != null && _points.Count > 0) return _points[0].ThermalExpansion;
                else return 0;
            }
            set
            {
                if (_points != null && _points.Count > 0) _points[0].ThermalExpansion = value;
            }
        }

        // Constructors                                                                                                             
        public ViewThermalExpansion(ThermalExpansion thermalExpansion)
        {
            _points = new List<ThermalExpansionDataPoint>();
            for (int i = 0; i < thermalExpansion.ThermalExpansionTemp.Length; i++)
            {
                _points.Add(new ThermalExpansionDataPoint(thermalExpansion.ThermalExpansionTemp[i][0],
                                                   thermalExpansion.ThermalExpansionTemp[i][1]));
            }
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  

    }
}
