using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace CaeGlobals
{
    static class MyUnit
    {
        static public readonly int NoUnit = 255;
        static public readonly EnergyUnit InchPound = (EnergyUnit)100;
        static public readonly PowerUnit InchPoundPerSecond = (PowerUnit)101;
        static public readonly ThermalConductivityUnit InchPoundPerSecondInchFahrenheit = (ThermalConductivityUnit)102;
        //
        static public string InchPoundAbbreviation = "in·lb";
        static public string InchPoundPerSecondAbbreviation = "in·lb/s";
        static public string InchPoundPerSecondInchFahrenheitAbbreviation = "in·lb/(s·in·°F)";
    }
}
