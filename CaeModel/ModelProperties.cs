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
    public enum ModelSpaceEnum
    {
        Undefined,
        [DynamicTypeDescriptor.StandardValue("Two_D", DisplayName = "2D")]
        Two_D,
        [DynamicTypeDescriptor.StandardValue("Three_D", DisplayName = "3D")]
        Three_D
    }

    [Serializable]
    public class ModelProperties
    {
        public ModelType ModelType;
        public ModelSpaceEnum ModelSpace;
        public string GlobalResultsFileName;
        public double AbsoluteZero;
        public double StefanBoltzmann;
        public double NewtonGravity;

        // Constructor
        public ModelProperties()
        {
            ModelType = ModelType.General;
            ModelSpace = ModelSpaceEnum.Undefined;
            GlobalResultsFileName = null;
            AbsoluteZero = double.PositiveInfinity;
            StefanBoltzmann = double.PositiveInfinity;
            NewtonGravity = double.PositiveInfinity;
        }
    }
}
