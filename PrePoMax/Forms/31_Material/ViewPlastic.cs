using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeModel;

namespace PrePoMax.PropertyViews
{
    [Serializable]
    public class DataPoint
    {
        // Variables                                                                                                                
        private double _strain;
        private double _stress;


        // Properties                                                                                                               
        [DisplayName("Yield Stress")]
        public double Stress { get { return _stress; } set { _stress = value; } }

        [DisplayName("Plastic strain")]
        public double Strain { get { return _strain; } set { _strain = value; } }


        // Constructors                                                                                                             
        public DataPoint()
        {
            _stress = 0;
            _strain = 0;
        }
        public DataPoint(double stress, double strain)
        {
            _stress = stress;
            _strain = strain;
        }
    }

    [Serializable]
    public class ViewPlastic : IViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<DataPoint> _points;
        private PlasticHardening _hardening;


        // Properties                                                                                                               
        [Browsable(false)]
        public string Name
        {
            get { return "Plastic"; }
        }

        [Browsable(false)]
        public MaterialProperty Base
        {
            get
            {
                int i = 0;
                double[][] stressStrain = new double[_points.Count][];
                foreach (DataPoint point in _points)
                {
                    stressStrain[i] = new double[2];
                    stressStrain[i][0] = point.Stress;
                    stressStrain[i][1] = point.Strain;
                    i++;
                }
                Plastic plastic = new Plastic(stressStrain);
                plastic.Hardening = _hardening;
                return plastic;
            }
        }

        [Browsable(false)]
        public List<DataPoint> DataPoints { get { return _points; } set { _points = value; } }

        [CategoryAttribute("Data")]
        [Description("Select the hardening rule.")]
        public PlasticHardening Hardening { get { return _hardening; } set { _hardening = value; } }


        // Constructors                                                                                                             
        public ViewPlastic(Plastic plastic)
        {
            _points = new List<DataPoint>();
            for (int i = 0; i < plastic.StressStrain.GetLength(0); i++)
            {
                _points.Add(new DataPoint(plastic.StressStrain[i][0], plastic.StressStrain[i][1]));
            }
            _hardening = plastic.Hardening;
        }


        // Methods                                                                                                                  
    }
}
