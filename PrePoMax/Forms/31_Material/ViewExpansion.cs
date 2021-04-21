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
    public class ViewExpansion : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<ExpansionDataPoint> _points;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Expansion"; }
        }
        //
        [Browsable(false)]
        public override MaterialProperty Base
        {
            get
            {
                int i = 0;
                double[][] expansionTemp = new double[_points.Count][];
                //
                foreach (ExpansionDataPoint point in _points)
                {
                    expansionTemp[i] = new double[2];
                    expansionTemp[i][0] = point.Expansion;
                    expansionTemp[i][1] = point.Temperature;
                    i++;
                }
                Expansion expansion = new Expansion(expansionTemp);
                //
                return expansion;
            }
        }
        //
        [Browsable(false)]
        public List<ExpansionDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [CategoryAttribute("Data"),
        DisplayName("Expansion"),
        DescriptionAttribute("The value of the thermal expansion coefficient.")]
        [TypeConverter(typeof(CaeGlobals.StringExpansionConverter))]
        public double Expansion
        {
            get
            {
                if (_points != null && _points.Count > 0) return _points[0].Expansion;
                else return 0;
            }
            set
            {
                if (_points != null && _points.Count > 0) _points[0].Expansion = value;
            }
        }

        // Constructors                                                                                                             
        public ViewExpansion(Expansion expansion)
        {
            _points = new List<ExpansionDataPoint>();
            for (int i = 0; i < expansion.ExpansionTemp.Length; i++)
            {
                _points.Add(new ExpansionDataPoint(expansion.ExpansionTemp[i][0], expansion.ExpansionTemp[i][1]));
            }
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  

    }
}
