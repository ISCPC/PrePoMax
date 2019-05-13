using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace vtkControl
{
    public static class vtkMaxWidgetTools
    {
        public static int[] GetTextSize(vtkTextMapper textMapper, vtkRenderer renderer)
        {
            int[] size = new int[2];

            if (renderer != null)
            {
                IntPtr sizeIntPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * 4);
                textMapper.GetSize(renderer, sizeIntPtr);
                System.Runtime.InteropServices.Marshal.Copy(sizeIntPtr, size, 0, 2);
            }
            else
            {
                size[0] = size[1] = 50;
            }

            return size;
        }
    }
}
