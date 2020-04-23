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
        private double _c0;
        private double _p0;
        private double _k;
        private double _sInf;
        private double[][] _pressureOverclosure;
        private PressureOverclosureEnum _pressureOverclosureType;


        // Properties                                                                                                               
        public double C0 { get { return _c0; } set { if (value > 0) _c0 = value; else throw new CaeException(_positive); } }
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
        public SurfaceBehavior(double c0, double p0)
        {
            SetDefaultValues();
            _c0 = c0;
            _p0 = p0;
            //
            _pressureOverclosureType = PressureOverclosureEnum.Exponential;
        }
        public SurfaceBehavior(double k, double sInf, double c0)
        {
            SetDefaultValues();
            _k = k;
            _sInf = sInf;
            _c0 = c0;
            //
            _pressureOverclosureType = PressureOverclosureEnum.Linear;
        }
        public SurfaceBehavior(double[][] pressureOverclosure)
        {
            SetDefaultValues();
            _pressureOverclosure = pressureOverclosure;
            //
            _pressureOverclosureType = PressureOverclosureEnum.Tabular; 
        }
        public SurfaceBehavior(double k)
        {
            SetDefaultValues();
            _k = k;
            //
            _pressureOverclosureType = PressureOverclosureEnum.Tied;
        }
        public SurfaceBehavior()
        {
            SetDefaultValues();
            //
            _pressureOverclosureType = PressureOverclosureEnum.Hard;
        }


        // Methods                                                                                                                  
        private void SetDefaultValues()
        {
            _c0 = 1E-3;
            _p0 = 0.1;
            _k = 10000000;
            _sInf = 1;
            _pressureOverclosure = new double[][] { new double[] { 0, 0 }, new double[] { 1000000, 10 } };
        }
    }
}
