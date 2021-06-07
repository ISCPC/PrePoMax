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
        static public readonly EnergyUnit PoundForceInch = (EnergyUnit)100;
        static public readonly PowerUnit PoundForceInchPerSecond = (PowerUnit)101;
        static public readonly ThermalConductivityUnit PoundForcePerSecondFahrenheit = (ThermalConductivityUnit)102;
        static public readonly HeatTransferCoefficientUnit PoundForcePerInchSecondFahrenheit = (HeatTransferCoefficientUnit)103;
        static public readonly PowerPerAreaUnit PoundForcePerInchSecond = (PowerPerAreaUnit)104;
        static public readonly PowerPerVolumeUnit PoundForcePerSquareInchSecond = (PowerPerVolumeUnit)105;
        //
        //http://www2.me.rochester.edu/courses/ME204/nx_help/index.html#uid:id1246862
        //
        static public string PoundForceInchAbbreviation = "lbf·in";                             // Energy
        static public string PoundForceInchPerSecondAbbreviation = "lbf·in/s";                  // Power
        static public string PoundForcePerSecondFahrenheitAbbreviation = "lbf/(s·°F)";          // Thermal conductivity
        static public string PoundForcePerInchSecondFahrenheitAbbreviation = "lbf/(in·s·°F)";   // Heat transfer coefficient
        static public string PoundForcePerInchSecondAbbreviation = "lbf/(in·s)";                // Power per area
        static public string PoundForcePerSquareInchSecondAbbreviation = "lbf/(in²·s)";         // Power per volume
    }
}
