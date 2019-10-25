using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace vtkControl
{
    public class vtkMaxPointLocator : vtkPointLocator
    {
        public vtkMaxPointLocator()
        {

        }

        public new static vtkMaxPointLocator New()
        {
            return new vtkMaxPointLocator();
        }

        public override int InsertUniquePoint(IntPtr x, ref long ptId)
        {
            ptId = this.InsertNextPoint(x);
            return 1;
        }
    }
}
