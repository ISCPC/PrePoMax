using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    public interface IViewMaterialProperty
    {
        string Name { get; }
        CaeModel.MaterialProperty Base { get; }
    }
}
