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
    public class ViewPlastic : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<PlasticDataPoint> _points;
        private PlasticHardening _hardening;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Plastic"; }
        }
        //
        [Browsable(false)]
        public override MaterialProperty Base
        {
            get
            {
                int i = 0;
                double[][] stressStrainTemp = new double[_points.Count][];
                //
                foreach (PlasticDataPoint point in _points)
                {
                    stressStrainTemp[i] = new double[3];
                    stressStrainTemp[i][0] = point.Stress;
                    stressStrainTemp[i][1] = point.Strain;
                    stressStrainTemp[i][2] = point.Temperature;
                    i++;
                }
                Plastic plastic = new Plastic(stressStrainTemp);
                plastic.Hardening = _hardening;
                //
                return plastic;
            }
        }
        //
        [Browsable(false)]
        public List<PlasticDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [CategoryAttribute("Data")]
        [Description("Select the hardening rule.")]
        public PlasticHardening Hardening { get { return _hardening; } set { _hardening = value; } }


        // Constructors                                                                                                             
        public ViewPlastic(Plastic plastic)
        {
            _points = new List<PlasticDataPoint>();
            for (int i = 0; i < plastic.StressStrainTemp.Length; i++)
            {                
                _points.Add(new PlasticDataPoint(plastic.StressStrainTemp[i][0],
                                                 plastic.StressStrainTemp[i][1],
                                                 plastic.StressStrainTemp[i][2]));
            }
            _hardening = plastic.Hardening;
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
    }
}
