using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Kitware.VTK;
using CaeGlobals;

namespace vtkControl
{
    public class vtkMaxExtreemeNode
    {
        // Variables                                                                                                                
        public int Id;
        public double[] Coor;
        public float Value;
        

        // Constructors                                                                                                             
        public vtkMaxExtreemeNode()
        {
            Id = -1;
            Coor = null;
            Value = -1;
        }
        public vtkMaxExtreemeNode(vtkMaxExtreemeNode source)
        {
            Id = source.Id;
            Coor = source.Coor;
            Value = source.Value;
        }
        public vtkMaxExtreemeNode(int id, double[] coordinates, float value)
        {
            Id = id;
            Coor = coordinates;
            Value = value;
        }
    }
}
