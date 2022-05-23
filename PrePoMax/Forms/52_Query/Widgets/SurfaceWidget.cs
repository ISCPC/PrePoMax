using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public class SurfaceWidget : WidgetBase
    {
        // Variables                                                                                                                
        private int _geometryId;


        // Properties                                                                                                               
        public int GeometryId { get { return _geometryId; } set { _geometryId = value; } }


        // Constructors                                                                                                             
        public SurfaceWidget(string name, int geometryId)
            : base(name)
        {
            _geometryId = geometryId;
        }

    }
}
