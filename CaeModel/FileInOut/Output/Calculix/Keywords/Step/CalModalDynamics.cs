using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalModalDynamics : CalculixKeyword
    {
        // Variables                                                                                                                
        private ModalDynamics _step;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalModalDynamics(ModalDynamics step)
        {
            _step = step;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string solver = _step.SolverType == SolverTypeEnum.Default ? "" : ", Solver=" + _step.SolverType.GetDisplayedName();
            return string.Format("*Modal dynamics{0}{1}", solver, Environment.NewLine);
        }
        public override string GetDataString()
        {
            string data = "";
            if (!_step.RunAnalysis) data += "*No Analysis" + Environment.NewLine;
            return data;
        }
    }
}
