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
    public enum FeElementTypeLinearTria
    {
        [StandardValue("None", Visible = false)]
        None,
        S3,
        M3D3,
        CPS3,
        CPE3,
        CAX3
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicTria
    {
        [StandardValue("None", Visible = false)]
        None,
        S6,
        M3D6,
        CPS6,
        CPE6,
        CAX6
    }
    //
    [Serializable]
    public enum FeElementTypeLinearQuad
    {
        [StandardValue("None", Visible = false)]
        None,
        S4,
        S4R,
        M3D4,
        M3D4R,
        CPS4,
        CPS4R,
        CPE4,
        CPE4R,
        CAX4,
        CAX4R
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicQuad
    {
        [StandardValue("None", Visible = false)]
        None,
        S8,
        S8R,
        M3D8,
        M3D8R,
        CPS8,
        CPS8R,
        CPE8,
        CPE8R,
        CAX8,
        CAX8R
    }
    // 3D                                                                                                                           
    [Serializable]
    public enum FeElementTypeLinearTetra
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D4
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicTetra
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D10,
        C3D10T
    }
    //
    [Serializable]
    public enum FeElementTypeLinearWedge
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D6
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicWedge
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D15
    }
    //
    [Serializable]
    public enum FeElementTypeLinearHexa
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D8,
        C3D8R,
        C3D8I
    }
    //
    [Serializable]
    public enum FeElementTypeParabolicHexa
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D20,
        C3D20R
    }
}
