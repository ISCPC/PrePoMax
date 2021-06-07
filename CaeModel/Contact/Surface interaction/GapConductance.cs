using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public enum GapConductanceEnum
    {
        Constant,
        Tabular
    }
    [Serializable]
    public class GapConductance : SurfaceInteractionProperty
    {
        // Variables                                                                                                                
        private static string _positive = "The value must be larger than 0.";
        //
        private double[][] _conductnancePressureTemp;
        private GapConductanceEnum _gapConductanceType;


        // Properties                                                                                                               
        public double[][] ConductnancePressureTemp
        {
            get { return _conductnancePressureTemp; }
            set
            {
                _conductnancePressureTemp = value;
                if (_conductnancePressureTemp != null)
                {
                    for (int i = 0; i < _conductnancePressureTemp.Length; i++)
                    {
                        //if (_conductnancePressureTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }
        public GapConductanceEnum GapConductanceType { get { return _gapConductanceType; } set { _gapConductanceType = value; } }


        // Constructors                                                                                                             
        public GapConductance()
            : this(new double[][] { new double[] { 0, 0, 0 } })
        { 
        }
        public GapConductance(double[][] conductnancePressureTemp)
        {
            if (conductnancePressureTemp.Length == 1) _gapConductanceType = GapConductanceEnum.Constant;
            else _gapConductanceType = GapConductanceEnum.Tabular;
            //
            _conductnancePressureTemp = conductnancePressureTemp;
        }


        // Methods                                                                                                                  
    }
}
