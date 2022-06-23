using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace CaeResults
{
    [Serializable]
    public enum DeformationScaleFactorTypeEnum
    {
        [StandardValue("Undeformed", DisplayName = "Undeformed", Description = "Undeformed")]
        Undeformed,
        //
        [StandardValue("TrueScale", DisplayName = "True scale", Description = "True scale")]
        TrueScale,
        //
        [StandardValue("Automatic", DisplayName = "Automatic", Description = "Automatic")]
        Automatic,
        //
        [StandardValue("Automatic025", DisplayName = "Automatic x 0.25", Description = "Automatic x 0.25")]
        Automatic025,
        //
        [StandardValue("Automatic05", DisplayName = "Automatic x 0.5", Description = "Automatic x 0.5")]
        Automatic05,
        //
        [StandardValue("Automatic2", DisplayName = "Automatic x 2", Description = "Automatic x 2")]
        Automatic2,
        //
        [StandardValue("Automatic5", DisplayName = "Automatic x 5", Description = "Automatic x 5")]
        Automatic5,
        //
        [StandardValue("UserDefined", DisplayName = "User defined", Description = "User defined")]
        UserDefined
    }
    public static class Extensions
    {
        public static float GetAutomaticFactor(this DeformationScaleFactorTypeEnum type)
        {
            if (type == DeformationScaleFactorTypeEnum.Automatic) return 1;
            else if (type == DeformationScaleFactorTypeEnum.Automatic025) return 0.25f;
            else if (type == DeformationScaleFactorTypeEnum.Automatic05) return 0.5f;
            else if (type == DeformationScaleFactorTypeEnum.Automatic2) return 2;
            else if (type == DeformationScaleFactorTypeEnum.Automatic5) return 5;
            else return -1;
        }
    }
}
