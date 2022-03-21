using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeModel
{
    [Serializable]
    public enum AmplitudeTimeSpanEnum
    {
        [StandardValue("StepTime", DisplayName = "Step time")]
        StepTime,
        [StandardValue("TotalTime", DisplayName = "Total time")]
        TotalTime
    }
    [Serializable]
    public abstract class Amplitude : NamedClass
    {
        // Variables                                                                                                                
        private AmplitudeTimeSpanEnum _timeSpan;
        private double _shiftX;
        private double _shiftY;
        //
        protected double[][] _timeAmplitude;


        // Properties                                                                                                               
        public AmplitudeTimeSpanEnum TimeSpan { get { return _timeSpan; } set { _timeSpan = value; } }
        public double ShiftX { get { return _shiftX; } set { _shiftX = value; } }
        public double ShiftY { get { return _shiftY; } set { _shiftY = value; } }
        public double[][] TimeAmplitude { get { return _timeAmplitude; } set { _timeAmplitude = value; } }

        // Constructors                                                                                                             
        public Amplitude(string name)
            : base(name)
        {
            _timeSpan = AmplitudeTimeSpanEnum.StepTime;
            _shiftX = 0;
            _shiftY = 0;
        }


        // Methods                                                                                                                  
    }
}
