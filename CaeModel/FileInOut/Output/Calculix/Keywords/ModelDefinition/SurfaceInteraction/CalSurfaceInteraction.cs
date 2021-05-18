using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalSurfaceInteraction : CalculixKeyword
    {
        // Variables                                                                                                                
        private SurfaceInteraction _surfaceInteraction;


        // Properties                                                                                                               
        public object GetBase { get { return _surfaceInteraction; } }


        // Constructor                                                                                                              
        public CalSurfaceInteraction(SurfaceInteraction surfaceInteraction)
        {
            _surfaceInteraction = surfaceInteraction;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Surface interaction, Name={0}{1}", _surfaceInteraction.Name, Environment.NewLine);
        }
        public override string GetDataString()
        {
            return "";
        }
    }
}
