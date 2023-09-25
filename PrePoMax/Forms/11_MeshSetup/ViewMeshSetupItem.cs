using CaeMesh;
using DynamicTypeDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax.Forms
{
    public abstract class ViewMeshSetupItem
    {
        // Variables                                                                                                                
        protected DynamicCustomTypeDescriptor _dctd;


        // Methods
        public abstract MeshSetupItem GetBase();
    }
}
