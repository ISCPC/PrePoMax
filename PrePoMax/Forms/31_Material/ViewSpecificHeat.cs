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
    public class ViewSpecificHeat : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<SpecificHeatDataPoint> _points;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Specific Heat"; }
        }
        //
        [Browsable(false)]
        public List<SpecificHeatDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [CategoryAttribute("Data"),
        DisplayName("Specific heat"),
        DescriptionAttribute("The value of the specific heat.")]
        [TypeConverter(typeof(EquationSpecificHeatConverter))]
        public EquationString SpecificHeat
        {
            get
            {
                if (_points != null && _points.Count > 0) return _points[0].SpecificHeat.Equation;
                else return new EquationString("0");
            }
            set
            {
                if (_points != null && _points.Count > 0) _points[0].SpecificHeat.Equation = value;
            }
        }


        // Constructors                                                                                                             
        public ViewSpecificHeat(SpecificHeat specificHeat)
        {
            _points = new List<SpecificHeatDataPoint>();
            for (int i = 0; i < specificHeat.SpecificHeatTemp.Length; i++)
            {
                _points.Add(new SpecificHeatDataPoint(specificHeat.SpecificHeatTemp[i][0], specificHeat.SpecificHeatTemp[i][1]));
            }
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override MaterialProperty GetBase()
        {
            int i = 0;
            EquationContainer[][] specificHeatTemp = new EquationContainer[_points.Count][];
            //
            foreach (SpecificHeatDataPoint point in _points)
            {
                specificHeatTemp[i] = new EquationContainer[2];
                specificHeatTemp[i][0] = point.SpecificHeat;
                specificHeatTemp[i][1] = point.Temperature;
                i++;
            }
            SpecificHeat specificHeat = new SpecificHeat(specificHeatTemp);
            //
            return specificHeat;
        }
    }
}
