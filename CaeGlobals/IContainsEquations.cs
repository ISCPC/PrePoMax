using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    public interface IContainsEquations
    {
        void CheckEquations();
        bool TryCheckEquations();
    }
}
