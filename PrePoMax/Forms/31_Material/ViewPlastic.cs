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
    public class MaterialDataPoint
    {
        // Variables                                                                                                                
        private double _strain;
        private double _stress;
        

        // Properties                                                                                                               
        [DisplayName("Yield Stress\n[?]")]
        [TypeConverter(typeof(CaeModel.StringPressureFromConverter))]
        public double Stress { get { return _stress; } set { _stress = value; } }

        [DisplayName("Plastic strain\n[?]")]
        public double Strain { get { return _strain; } set { _strain = value; } }


        // Constructors                                                                                                             
        public MaterialDataPoint()
        {
            _stress = 0;
            _strain = 0;
        }
        public MaterialDataPoint(double stress, double strain)
        {
            _stress = stress;
            _strain = strain;
        }
    }

    [Serializable]
    public class ViewPlastic : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<MaterialDataPoint> _points;
        private PlasticHardening _hardening;


        // Properties                                                                                                               
        [Browsable(false)]
        public override string Name
        {
            get { return "Plastic"; }
        }

        [Browsable(false)]
        public override MaterialProperty Base
        {
            get
            {
                int i = 0;
                double[][] stressStrain = new double[_points.Count][];
                foreach (MaterialDataPoint point in _points)
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
        public List<MaterialDataPoint> DataPoints { get { return _points; } set { _points = value; } }

        [CategoryAttribute("Data")]
        [Description("Select the hardening rule.")]
        public PlasticHardening Hardening { get { return _hardening; } set { _hardening = value; } }


        // Constructors                                                                                                             
        public ViewPlastic(Plastic plastic)
        {
            _points = new List<MaterialDataPoint>();
            for (int i = 0; i < plastic.StressStrain.GetLength(0); i++)
            {                
                _points.Add(new MaterialDataPoint(plastic.StressStrain[i][0], plastic.StressStrain[i][1]));
            }
            _hardening = plastic.Hardening;
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
    }
}
