using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vtkControl
{
    public enum vtkFieldAssociations
    {
        FIELD_ASSOCIATION_POINTS,
        FIELD_ASSOCIATION_CELLS,
        FIELD_ASSOCIATION_NONE,
        FIELD_ASSOCIATION_POINTS_THEN_CELLS,
        FIELD_ASSOCIATION_VERTICES,
        FIELD_ASSOCIATION_EDGES,
        FIELD_ASSOCIATION_ROWS,
        NUMBER_OF_ASSOCIATIONS
    };
}
