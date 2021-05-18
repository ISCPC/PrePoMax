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
    internal class CalStep : CalculixKeyword
    {
        // Variables                                                                                                                
        private Step _step;


        // Properties                                                                                                               
        public object GetBase { get { return _step; } }


        // Constructor                                                                                                              
        public CalStep(Step step)
        {
            _step = step;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string perturbation = _step.Perturbation ? ", Perturbation" : "";
            string nlGeom = _step.Nlgeom ? ", Nlgeom" : "";
            string inc = "";
            if (_step.IncrementationType != IncrementationTypeEnum.Default) inc = ", Inc=" + _step.MaxIncrements;
            //
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Step{0}{1}{2}", perturbation, nlGeom, inc).AppendLine();
            return sb.ToString();
        }
        public override string GetDataString()
        {
            return "";
        }
    }
}
