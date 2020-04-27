using CaeGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public enum PressureOverclosureEnum
    {
        Hard,
        Linear,
        Exponential,
        Tabular,
        Tied
    }

    [Serializable]
    public class SurfaceBehavior : SurfaceInteractionProperty
    {
        // Variables                                                                                                                
        private static string _positive = "The value must be larger than 0.";
        //
        private double _c0;
        private double _p0;
        private double _k;
        private double _sInf;
        private double[][] _pressureOverclosure;
        private PressureOverclosureEnum _pressureOverclosureType;


        // Properties                                                                                                               
        public double C0
        {
            get { return _c0; }
            set { if (double.IsNaN(value) || value > 0) _c0 = value; else throw new CaeException(_positive); }
        }
        public double P0 { get { return _p0; } set { if (value > 0) _p0 = value; else throw new CaeException(_positive); } }
        public double K { get { return _k; } set { if (value > 0) _k = value; else throw new CaeException(_positive); } }
        public double Sinf { get { return _sInf; } set { if (value > 0) _sInf = value; else throw new CaeException(_positive); } }
        public double[][] PressureOverclosure { get { return _pressureOverclosure; } set { _pressureOverclosure = value; } }
        public PressureOverclosureEnum PressureOverclosureType 
        { 
            get { return _pressureOverclosureType; }
            set 
            {
                if (value != _pressureOverclosureType)
                {
                    _pressureOverclosureType = value;
                    SetDefaultValues();
                }
            }
        }


        // Constructors                                                                                                             
        public SurfaceBehavior()
        {
            SetDefaultValues();
            //
            _pressureOverclosureType = PressureOverclosureEnum.Hard;
        }


        // Methods                                                                                                                  
        private void SetDefaultValues()
        {
            if (_pressureOverclosureType == PressureOverclosureEnum.Linear) C0 = double.NaN;
            else C0 = 1E-3;
            //
            P0 = 0.1;
            K = 10000000;
            Sinf = 1;
            _pressureOverclosure = new double[][] { new double[] { 0, 0 }, new double[] { 1000000, 10 } };
        }
    }
}
