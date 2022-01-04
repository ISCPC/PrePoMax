using System;
using System.Collections.Generic;
using DynamicTypeDescriptor;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    // 2D                                                                                                                           
    [Serializable]
    public enum FeElementTypeSpring
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        SPRING1 = 10,
        SPRING2 = 11,
        SPRINGA = 12,
    }
    //
    [Serializable]
    public enum FeElementTypeLinearTria
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        S3 = 10,
        M3D3 = 11,
        CPS3 = 20,
        CPE3 = 30,
        CAX3 = 40
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicTria
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        S6 = 10,
        M3D6 = 11,
        CPS6 = 20,
        CPE6 = 30,
        CAX6 = 40
    }
    //
    [Serializable]
    public enum FeElementTypeLinearQuad
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        S4 = 10,
        S4R = 11,
        M3D4 = 12,
        M3D4R = 13,
        CPS4 = 20,
        CPS4R = 21,
        CPE4 = 30,
        CPE4R = 31,
        CAX4 = 40,
        CAX4R = 41
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicQuad
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        S8 = 10,
        S8R = 11,
        M3D8 = 12,
        M3D8R = 13,
        CPS8 = 20,
        CPS8R = 21,
        CPE8 = 30,
        CPE8R = 31,
        CAX8 = 40,
        CAX8R = 41
    }
    // 3D                                                                                                                           
    [Serializable]
    public enum FeElementTypeLinearTetra
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        C3D4 = 10
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicTetra
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        C3D10 = 10,
        C3D10T = 11
    }
    //
    [Serializable]
    public enum FeElementTypeLinearWedge
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        C3D6 = 10
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicWedge
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        C3D15 = 10
    }
    //
    [Serializable]
    public enum FeElementTypeLinearHexa
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        C3D8 = 10,
        C3D8R = 11,
        C3D8I = 12
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicHexa
    {
        [StandardValue("None", Visible = false)]
        None = 0,
        C3D20 = 10,
        C3D20R = 11
    }
}
