using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;


namespace CaeModel
{
    [Serializable]
    public enum ModelType
    {
        General,
        Submodel,
    }

    [Serializable]
    public struct ModelProperties
    {
        public ModelType ModelType;
        public string GlobalResultsFileName;
    }
}
