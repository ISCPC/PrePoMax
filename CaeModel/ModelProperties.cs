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
        Undefined = 0,
        [DynamicTypeDescriptor.StandardValue("ThreeD", DisplayName = "3D")]
        ThreeD = 1,
        [DynamicTypeDescriptor.StandardValue("PlaneStress", DisplayName = "2D plane stress")]
        PlaneStress = 2,
        [DynamicTypeDescriptor.StandardValue("PlaneStrain", DisplayName = "2D plane strain")]
        PlaneStrain = 3,
        [DynamicTypeDescriptor.StandardValue("Axisymmetric", DisplayName = "2D axisymmetric")]
        Axisymmetric = 4
    }
    public static class ExtensionMethods
    {
        // ModelSpaceEnum
        public static bool IsTwoD(this ModelSpaceEnum modelSpace)
        {
            return (int)modelSpace > 1; // 2, 3 or 4 is 2D
        }
    }

    [Serializable]
    public class ModelProperties
    {
        // Variables                                                                                                                
        public ModelType ModelType;
        public ModelSpaceEnum ModelSpace;
        public string GlobalResultsFileName;
        public double AbsoluteZero;
        public double StefanBoltzmann;
        public double NewtonGravity;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public ModelProperties()
        {
            ModelType = ModelType.General;
            ModelSpace = ModelSpaceEnum.Undefined;
            GlobalResultsFileName = null;
            AbsoluteZero = double.PositiveInfinity;
            StefanBoltzmann = double.PositiveInfinity;
            NewtonGravity = double.PositiveInfinity;
        }


        // Methods                                                                                                                  

    }
}
