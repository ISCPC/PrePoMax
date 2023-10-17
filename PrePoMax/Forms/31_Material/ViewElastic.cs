using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewElastic : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<ElasticDataPoint> _points;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Elastic"; }
        }
        //
        [Browsable(false)]
        public List<ElasticDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 2, "Young's modulus")]
        [DescriptionAttribute("The value of the Young's modulus.")]
        [TypeConverter(typeof(EquationPressureConverter))]
        public EquationString YoungsModulus
        {
            get
            {
                if (_points != null && _points.Count > 0) return _points[0].YoungsModulus.Equation;
                else return new EquationString("0");
            }
            set
            {
                if (_points != null && _points.Count > 0) _points[0].YoungsModulus.Equation = value;
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 2, "Poisson's ratio")]
        [DescriptionAttribute("The value of the Poisson's ratio.")]
        [TypeConverter(typeof(EquationDoubleConverter))]
        public EquationString PoissonsRatio
        {
            get
            {
                if (_points != null && _points.Count > 0) return _points[0].PoissonsRatio.Equation;
                else return new EquationString("0");
            }
            set
            {
                if (_points != null && _points.Count > 0) _points[0].PoissonsRatio.Equation = value;
            }
        }


        // Constructors                                                                                                             
        public ViewElastic(Elastic elastic)
        {
            _points = new List<ElasticDataPoint>();
            for (int i = 0; i < elastic.YoungsPoissonsTemp.Length; i++)
            {
                _points.Add(new ElasticDataPoint(elastic.YoungsPoissonsTemp[i][0],
                                                 elastic.YoungsPoissonsTemp[i][1],
                                                 elastic.YoungsPoissonsTemp[i][2]));
            }
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override MaterialProperty GetBase()
        {
            int i = 0;
            EquationContainer[][] youngsPoissonsTemp = new EquationContainer[_points.Count][];
            //
            foreach (ElasticDataPoint point in _points)
            {
                youngsPoissonsTemp[i] = new EquationContainer[3];
                youngsPoissonsTemp[i][0] = point.YoungsModulus;
                youngsPoissonsTemp[i][1] = point.PoissonsRatio;
                youngsPoissonsTemp[i][2] = point.Temperature;
                i++;
            }
            Elastic elastic = new Elastic(youngsPoissonsTemp);
            //
            return elastic;
        }
        public double GetYoungsModulusValue()
        {
            if (_points != null && _points.Count > 0) return _points[0].YoungsModulus.Value;
            else return 0;
        }

    }
}
