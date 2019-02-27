using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Kitware.VTK;

namespace vtkControl
{
    public static class ExtensionMethods
    {
        public static Color Black(this vtkProperty prop)
        {
            return Color.Black;
        }
    }
}
