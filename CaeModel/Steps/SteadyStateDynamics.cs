using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class SteadyStateDynamics : Step, ISerializable
    {
        // Variables                                                                                                                
        private bool _harmonic;             //ISerializable
        private double _frequencyLower;     //ISerializable
        private double _frequencyUpper;     //ISerializable
        private int _numDataPoints;         //ISerializable
        private double _bias;               //ISerializable
        private int _numFourierTerms;       //ISerializable
        private double _timeLower;          //ISerializable
        private double _timeUpper;          //ISerializable


        // Properties                                                                                                               
        public bool Harmonic { get { return _harmonic; } set { _harmonic = value; } }
        public double FrequencyLower { get { return _frequencyLower; } set { _frequencyLower = value; } }
        public double FrequencyUpper { get { return _frequencyUpper; } set { _frequencyUpper = value; } }
        public int NumDataPoints
        {
            get { return _numDataPoints; }
            set
            {
                _numDataPoints = value;
                if (_numDataPoints != int.MinValue && _numDataPoints < 2) _numDataPoints = 2;
            }
        }
        public double Bias
        {
            get { return _bias; }
            set
            {
                _bias = value;
                if (_bias != double.NaN && _bias < 1) _bias = 1;
            }
        }
        public int NumFourierTerms
        {
            get { return _numFourierTerms; }
            set
            {
                _numFourierTerms = value;
                if (_numFourierTerms < 1) _numFourierTerms = 1;
            }
        }
        public double TimeLower { get { return _timeLower; } set { _timeLower = value; } }
        public double TimeUpper { get { return _timeUpper; } set { _timeUpper = value; } }


        // Constructors                                                                                                             
        public SteadyStateDynamics(string name)
            :base(name)
        {
            _harmonic = true;
            _frequencyLower = 0;
            _frequencyUpper = 10;
            _numDataPoints = 20;
            _bias = 3;
            _numFourierTerms = 20;
            _timeLower = 0;
            _timeUpper = 1;
            //
            AddFieldOutput(new NodalFieldOutput("NF-Output-1", NodalFieldVariable.U | NodalFieldVariable.RF));
            AddFieldOutput(new ElementFieldOutput("EF-Output-1", ElementFieldVariable.E | ElementFieldVariable.S));
        }
        //ISerializable
        public SteadyStateDynamics(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_harmonic":
                        _harmonic = (bool)entry.Value; break;
                    case "_frequencyLower":
                        _frequencyLower = (double)entry.Value; break;
                    case "_frequencyUpper":
                        _frequencyUpper = (double)entry.Value; break;
                    case "_numDataPoints":
                        _numDataPoints = (int)entry.Value; break;
                    case "_bias":
                        _bias = (double)entry.Value; break;
                    case "_numFourierTerms":
                        _numFourierTerms = (int)entry.Value; break;
                    case "_timeLower":
                        _timeLower = (double)entry.Value; break;
                    case "_timeUpper":
                        _timeUpper = (double)entry.Value; break;
                }
            }
        }


        // Methods                                                                                                                  
        public override bool IsBoundaryConditionSupported(BoundaryCondition boundaryCondition)
        {
            if (boundaryCondition is FixedBC || 
                boundaryCondition is DisplacementRotation)
                return true;
            else if (boundaryCondition is SubmodelBC ||
                     boundaryCondition is TemperatureBC)
                return false;
            else throw new NotSupportedException();
        }
        public override bool IsLoadTypeSupported(Type loadType)
        {
            if (loadType == typeof(CLoad) ||
                loadType == typeof(MomentLoad) ||
                loadType == typeof(DLoad) ||
                loadType == typeof(HydrostaticPressure) ||
                loadType == typeof(ImportedPressure) ||
                loadType == typeof(STLoad) ||
                loadType == typeof(ShellEdgeLoad) ||
                loadType == typeof(GravityLoad) ||
                loadType == typeof(CentrifLoad) ||
                loadType == typeof(PreTensionLoad))
            {
                return true;
            }
            else if (loadType == typeof(CFlux) ||
                     loadType == typeof(DFlux) ||
                     loadType == typeof(BodyFlux) ||
                     loadType == typeof(FilmHeatTransfer) ||
                     loadType == typeof(RadiationHeatTransfer))
            {
                return false;
            }
            else throw new NotSupportedException();
        }

        public override bool IsDefinedFieldSupported(DefinedField definedField)
        {
            if (definedField is DefinedTemperature) return false;
            else throw new NotSupportedException();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_harmonic", _harmonic, typeof(bool));
            info.AddValue("_frequencyLower", _frequencyLower, typeof(double));
            info.AddValue("_frequencyUpper", _frequencyUpper, typeof(double));
            info.AddValue("_numDataPoints", _numDataPoints, typeof(int));
            info.AddValue("_bias", _bias, typeof(double));
            info.AddValue("_numFourierTerms", _numFourierTerms, typeof(int));
            info.AddValue("_timeLower", _timeLower, typeof(double));
            info.AddValue("_timeUpper", _timeUpper, typeof(double));
        }
    }
}
