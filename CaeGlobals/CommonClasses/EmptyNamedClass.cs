using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    [Serializable]
    public class EmptyNamedClass : NamedClass
    {
        // Empty named class is used to trasfer the name only

        public EmptyNamedClass(string name)
            :base()
        {
            _checkName = false;     // the name may contain other cahracters - do not use constructor with name
            Name = name;
        }
    }
}
