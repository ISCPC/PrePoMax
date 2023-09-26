using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
	[Serializable]
	public enum GeomCurveType
    {
		Line,
		Circle,
		Ellipse,
		Hyperbola,
		Parabola,
		BezierCurve,
		BSplineCurve,
		OffsetCurve,
		OtherCurve,
		Unknown
	}
}
