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
    public class ViewDensity : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<DensityDataPoint> _points;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Density"; }
        }
        //
        [Browsable(false)]
        public override MaterialProperty Base
        {
            get
            {
                int i = 0;
                double[][] densityTemp = new double[_points.Count][];
                //
                foreach (DensityDataPoint point in _points)
                {
                    densityTemp[i] = new double[2];
                    densityTemp[i][0] = point.Density;
                    densityTemp[i][1] = point.Temperature;
                    i++;
                }
                Density density = new Density(densityTemp);
                //
                return density;
            }
        }
        //
        [Browsable(false)]
        public List<DensityDataPoint> DataPoints { get { return _points; } set { _points = value; } }


        // Constructors                                                                                                             
        public ViewDensity(Density density)
        {
            _points = new List<DensityDataPoint>();
            for (int i = 0; i < density.DensityTemp.Length; i++)
            {
                _points.Add(new DensityDataPoint(density.DensityTemp[i][0], density.DensityTemp[i][1]));
            }
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  

    }
}
