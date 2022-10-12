using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using NCalc;

namespace CaeGlobals
{
    static class MyNCalc
    {
        static public Expression GetExpression(string expression)
        {
            Expression e = new Expression(expression, EvaluateOptions.IgnoreCase);
            e.EvaluateParameter += EvaluateParameter;
            return e;
        }

        static public void EvaluateParameter(string name, ParameterArgs args)
        {
            if (name == "Pi") args.Result = Math.PI;
        }
}
}
