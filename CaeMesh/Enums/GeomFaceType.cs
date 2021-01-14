using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
	[Serializable]
	public enum GeomFaceType
	{
		Plane,
		Cylinder,
		Cone,
		Sphere,
		Torus,
		BezierSurface,
		BSplineSurface,
		SurfaceOfRevolution,
		SurfaceOfExtrusion,
		OffsetSurface,
		OtherSurface,
		Unknown
	}
}
