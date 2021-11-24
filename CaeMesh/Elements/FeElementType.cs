using System;
using System.Collections.Generic;
using DynamicTypeDescriptor;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public enum FeElementTypeLinearTria
    {
        [StandardValue("None", Visible = false)]
        None,
        S3
    }

    [Serializable]
    public enum FeElementTypeParabolicTria
    {
        [StandardValue("None", Visible = false)]
        None,
        S6,
        CPS6
    }

    [Serializable]
    public enum FeElementTypeLinearQuad
    {
        [StandardValue("None", Visible = false)]
        None,
        S4,
        S4R
    }
    
    [Serializable]
    public enum FeElementTypeParabolicQuad
    {
        [StandardValue("None", Visible = false)]
        None,
        S8,
        S8R
    }

    [Serializable]
    public enum FeElementTypeLinearTetra
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D4
    }

    [Serializable]
    public enum FeElementTypeParabolicTetra
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D10
    }

    [Serializable]
    public enum FeElementTypeLinearWedge
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D6
    }

    [Serializable]
    public enum FeElementTypeParabolicWedge
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D15
    }

    [Serializable]
    public enum FeElementTypeLinearHexa
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D8,
        C3D8R,
        C3D8I
    }

    [Serializable]
    public enum FeElementTypeParabolicHexa
    {
        [StandardValue("None", Visible = false)]
        None,
        C3D20,
        C3D20R
    }

    [Serializable]
    public enum FeElementType
    {
        // hexa
        C3D8,
        C3D8R,
        C3D8I,
        //
        C3D20,
        C3D20R,

        // tetra
        C3D4,
        //
        C3D10,

        // wedge
        C3D6,
        //
        C3D15,

        // triangle
        S3,
        //
        S6,

        // quad
        S4,
        S4R,
        //
        S8,
        S8R,

        // beam
        B31,
        B31R,
        //
        B32,
        B32R
    }
}
