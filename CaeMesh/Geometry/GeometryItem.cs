using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    public abstract class GeometryItem
    {
        // Variables                                                                                                                        
        public int Id;
        public int[] Labels;


        // Constructors                                                                                                             
        public GeometryItem(int id, int[] labels)
        {
            Id = id;
            Labels = labels;
        }
    }
}
