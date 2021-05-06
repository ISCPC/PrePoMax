using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;


namespace CaeModel
{
    [Serializable]
    public enum ModelType
    {
        General,
        Submodel,
    }

    [Serializable]
    public class ModelProperties
    {
        public ModelType ModelType;
        public string GlobalResultsFileName;
        public double AbsoluteZero;
        public double StefanBoltzmann;
        public double NewtonGravity;

        // Constructor
        public ModelProperties()
        {
            ModelType = ModelType.General;
            GlobalResultsFileName = null;
            AbsoluteZero = double.PositiveInfinity;
            StefanBoltzmann = double.PositiveInfinity;
            NewtonGravity = double.PositiveInfinity;
        }
    }
}
