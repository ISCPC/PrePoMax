using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeModel;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalAmplitude : CalculixKeyword
    {
        // Variables                                                                                                                
        private Amplitude _amplitude;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalAmplitude(Amplitude amplitude)
        {
            _amplitude = amplitude;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string time = "";
            if (_amplitude.TimeSpan == AmplitudeTimeSpanEnum.TotalTime) time = ", Time=Total time";
            string shiftX = "";
            if (_amplitude.ShiftX != 0) shiftX = ", ShiftX=" + _amplitude.ShiftX.ToCalculiX16String();
            string shiftY = "";
            if (_amplitude.ShiftY != 0) shiftY = ", ShiftY=" + _amplitude.ShiftY.ToCalculiX16String();
            //
            return string.Format("*Amplitude, Name={0}{1}{2}{3}{4}", _amplitude.Name, time, shiftX, shiftY, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _amplitude.TimeAmplitude.Length; i++)
            {
                if (i != 0 && i % 4 == 0) sb.AppendLine();      // a new line for more than 8 data values
                sb.AppendFormat("{0}, {1}", _amplitude.TimeAmplitude[i][0], _amplitude.TimeAmplitude[i][1]);
                //
                if (i < _amplitude.TimeAmplitude.Length - 1) sb.Append(", ");
            }
            sb.AppendLine(); // a new line for the next keyword
            //
            return sb.ToString();
        }
    }
}
