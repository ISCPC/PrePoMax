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
    public class ViewGapConductance : ViewSurfaceInteractionProperty
    {
        // Variables                                                                                                                
        private CaeModel.GapConductance _gapConductance;
        private List<GapConductanceDataPoint> _points;


        // Properties                                                                                                               
        [Browsable(false)]
        public override string Name
        {
            get { return "Gap conductance"; }
        }
        //
        [Browsable(false)]
        public override CaeModel.SurfaceInteractionProperty Base
        {
            get
            {
                int i = 0;
                double[][] conductancePressureTemp = new double[_points.Count][];
                foreach (GapConductanceDataPoint point in _points)
                {
                    conductancePressureTemp[i] = new double[3];
                    conductancePressureTemp[i][0] = point.Conductance;
                    conductancePressureTemp[i][1] = point.Pressure;
                    conductancePressureTemp[i][2] = point.Temperature;
                    i++;
                }
                _gapConductance.ConductnancePressureTemp = conductancePressureTemp;
                //
                return _gapConductance;
            }
        }
        //
        [Browsable(false)]
        public List<GapConductanceDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Type")]
        [DescriptionAttribute("Select the gap conductance type for this surface bahavior.")]
        public CaeModel.GapConductanceEnum GapConductanceType
        {
            get { return _gapConductance.GapConductanceType; }
            set { _gapConductance.GapConductanceType = value; UpdateVisibility(); }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Conductance")]
        [Description("The conductance is the ratio of the heat flow across the contact location and the temperature difference " +
                     "between the corresponding slave and master surfaces.")]
        [TypeConverter(typeof(StringHeatTransferCoefficientConverter))]
        public double Conductance
        {
            get
            {
                if (_points != null && _points.Count > 0) return _points[0].Conductance;
                else return 0;
            }
            set
            {
                if (_points != null && _points.Count > 0) _points[0].Conductance = value;
            }
        }


        // Constructors                                                                                                             
        public ViewGapConductance(CaeModel.GapConductance gapConductance)
        {
            _gapConductance = gapConductance;
            //
            _points = new List<GapConductanceDataPoint>();
            for (int i = 0; i < _gapConductance.ConductnancePressureTemp.Length; i++)
            {
                _points.Add(new GapConductanceDataPoint(_gapConductance.ConductnancePressureTemp[i][0],
                                                        _gapConductance.ConductnancePressureTemp[i][1],
                                                        _gapConductance.ConductnancePressureTemp[i][2]));
            }
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (_gapConductance.GapConductanceType == CaeModel.GapConductanceEnum.Constant)
            {
                DynamicCustomTypeDescriptor.GetProperty(nameof(Conductance)).SetIsBrowsable(true);
            }
            else if (_gapConductance.GapConductanceType == CaeModel.GapConductanceEnum.Tabular)
            {
                DynamicCustomTypeDescriptor.GetProperty(nameof(Conductance)).SetIsBrowsable(false);
            }
            else throw new NotSupportedException();
        }

    }
}
