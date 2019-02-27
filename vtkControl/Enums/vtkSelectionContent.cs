using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vtkControl
{
    enum vtkSelectionContent
    {
        SELECTIONS,  // Deprecated.
        GLOBALIDS,
        PEDIGREEIDS,
        VALUES,
        INDICES,
        FRUSTUM,
        LOCATIONS,
        THRESHOLDS,
        BLOCKS,       // used to select blocks within a composite dataset.
        QUERY
    }
}
