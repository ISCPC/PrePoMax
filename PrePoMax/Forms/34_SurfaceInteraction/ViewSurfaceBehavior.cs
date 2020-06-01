using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax
{
    [Serializable]
    public class PressureOverclosureDataPoint
    {
        // Variables                                                                                                                
        private double _pressure;
        private double _overclosure;


        // Properties                                                                                                               
        [DisplayName("Pressure\n[?]")]
        [TypeConverter(typeof(CaeModel.StringPressureConverter))]
        public double Pressure { get { return _pressure; } set { _pressure = value; } }
        //
        [DisplayName("Overclosure\n[?]")]
        [TypeConverter(typeof(CaeModel.StringLengthConverter))]
        public double Overclosure { get { return _overclosure; } set { _overclosure = value; } }


        // Constructors                                                                                                             
        public PressureOverclosureDataPoint()
        {
            _overclosure = 0;
            _pressure = 0;
        }
        public PressureOverclosureDataPoint(double pressure, double overclosure)
        {
            _pressure = pressure;
            _overclosure = overclosure;
        }
    }

    [Serializable]
    public class ViewSurfaceBehavior : ViewSurfaceInteractionProperty
    {
        // Variables                                                                                                                
        private CaeModel.SurfaceBehavior _surfaceBehavior;
        private List<PressureOverclosureDataPoint> _points;


        // Properties                                                                                                               
        [Browsable(false)]
        public override string Name
        {
            get { return "Surface behavior"; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Type")]
        [DescriptionAttribute("Select the pressure-overclosure type for this surface bahavior.")]
        public CaeModel.PressureOverclosureEnum PressureOverclosureType
        {
            get { return _surfaceBehavior.PressureOverclosureType; } 
            set { _surfaceBehavior.PressureOverclosureType = value; UpdateVisibility(); } 
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "K")]
        [Description("Slope of the pressure-overclosure curve (usually 5 to 50 times the E modulus of the adjacent materials).")]
        [TypeConverter(typeof(CaeModel.StringForcePerVolumeConverter))]
        public double K { get { return _surfaceBehavior.K; } set { _surfaceBehavior.K = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Sinf")]
        [Description("Tension value for large clearances (about 0.25 % of the maximum expected von Mises stress).")]
        [TypeConverter(typeof(CaeModel.StringPressureConverter))]
        public double Sinf { get { return _surfaceBehavior.Sinf; } set { _surfaceBehavior.Sinf = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "c0")]
        [DescriptionAttribute("The value from which the maximum clearance is calculated for which a spring contact " +
                              "element is generated. The default for c0 is 0.001.")]
        [TypeConverter(typeof(StringDefaultDoubleConverter))]
        public double C0_lin
        {
            get
            {
                StringDefaultDoubleConverter.InitialValue = 0.001;
                //
                return _surfaceBehavior.C0;
            }
            set { _surfaceBehavior.C0 = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "p0")]
        [DescriptionAttribute("The contact pressure at zero clerance (p0 > 0).")]
        [TypeConverter(typeof(CaeModel.StringPressureConverter))]
        public double P0 { get { return _surfaceBehavior.P0; } set { _surfaceBehavior.P0 = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "c0")]
        [DescriptionAttribute("The clerance at which the contact pressure is decreased to 1 % of p0 (c0 > 0).")]
        [TypeConverter(typeof(CaeModel.StringLengthConverter))]
        public double C0_exp { get { return _surfaceBehavior.C0; } set { _surfaceBehavior.C0 = value; } }
        
        //
        [Browsable(false)]
        public List<PressureOverclosureDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [Browsable(false)]
        public override CaeModel.SurfaceInteractionProperty Base
        {
            get            
            {
                int i = 0;
                double[][] pressureOverclosure = new double[_points.Count][];
                foreach (PressureOverclosureDataPoint point in _points)
                {
                    pressureOverclosure[i] = new double[2];
                    pressureOverclosure[i][0] = point.Pressure;
                    pressureOverclosure[i][1] = point.Overclosure;
                    i++;
                }
                _surfaceBehavior.PressureOverclosure = pressureOverclosure;
                //
                return _surfaceBehavior; 
            }
        }


        // Constructors                                                                                                             
        public ViewSurfaceBehavior(CaeModel.SurfaceBehavior surfaceBehavior)
        {
            _surfaceBehavior = surfaceBehavior;
            //
            _points = new List<PressureOverclosureDataPoint>();
            for (int i = 0; i < _surfaceBehavior.PressureOverclosure.Length; i++)
            {
                _points.Add(new PressureOverclosureDataPoint(_surfaceBehavior.PressureOverclosure[i][0],
                                                             _surfaceBehavior.PressureOverclosure[i][1]));
            }
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            DynamicTypeDescriptor.CustomPropertyDescriptor cpd;
            if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Hard)
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(K));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(P0));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp));
                cpd.SetIsBrowsable(false);
            }
            else if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Linear)
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(K));
                cpd.SetIsBrowsable(true);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf));
                cpd.SetIsBrowsable(true);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin));
                cpd.SetIsBrowsable(true);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(P0));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp));
                cpd.SetIsBrowsable(false);
            }
            else if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Exponential)
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(K));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(P0));
                cpd.SetIsBrowsable(true);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp));
                cpd.SetIsBrowsable(true);
            }
            else if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Tabular)
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(K));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(P0));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp));
                cpd.SetIsBrowsable(false);
            }
            else if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Tied)
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(K));
                cpd.SetIsBrowsable(true);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(P0));
                cpd.SetIsBrowsable(false);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp));
                cpd.SetIsBrowsable(false);
            }
            else throw new NotSupportedException();
        }

    }
}
