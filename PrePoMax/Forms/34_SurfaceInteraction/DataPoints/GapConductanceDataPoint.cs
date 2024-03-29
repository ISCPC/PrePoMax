﻿using System;
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
    public class GapConductanceDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private double _conductance;
        private double _pressure;


        // Properties                                                                                                               
        [DisplayName("Conductance\n[?]")]
        [TypeConverter(typeof(StringHeatTransferCoefficientFromConverter))]
        public double Conductance { get { return _conductance; } set { _conductance = value; } }
        //
        [DisplayName("Pressure\n[?]")]
        [TypeConverter(typeof(StringPressureFromConverter))]
        public double Pressure { get { return _pressure; } set { _pressure = value; } }


        // Constructors                                                                                                             
        public GapConductanceDataPoint()
            :base(0)
        {
            _conductance = 0;
            _pressure = 0;
        }
        public GapConductanceDataPoint(double conductance, double pressure, EquationContainer temperature)
            :base(temperature)
        {
            _conductance = conductance;
            _pressure = pressure;
        }
    }
}


