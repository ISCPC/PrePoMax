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
    internal class CalStaticStep : CalculixKeyword
    {
        // Variables                                                                                                                
        private StaticStep _step;


        // Properties                                                                                                               
        public override object GetBase { get { return _step; } }


        // Constructor                                                                                                              
        public CalStaticStep(StaticStep step)
        {
            _step = step;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string solver = _step.SolverType == SolverTypeEnum.Default ? "" : ", Solver=" + _step.SolverType.GetDisplayedName();
            string direct = _step.IncrementationType == IncrementationTypeEnum.Direct ? ", Direct" : "";
            return string.Format("*Static{0}{1}{2}", solver, direct, Environment.NewLine);
        }
        public override string GetDataString()
        {
            if (_step.IncrementationType != IncrementationTypeEnum.Default)
            {
                string minMax = "";
                if (_step.IncrementationType == IncrementationTypeEnum.Automatic)
                    minMax = string.Format(", {0}, {1}", _step.MinTimeIncrement, _step.MaxTimeIncrement);
                //
                return string.Format("{0}, {1}{2}{3}", _step.InitialTimeIncrement, _step.TimePeriod, minMax, Environment.NewLine);
            }
            else return "";
        }
    }
}
