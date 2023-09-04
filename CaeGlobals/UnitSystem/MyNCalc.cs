using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using NCalc;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace CaeGlobals
{
    public static class MyNCalc
    {
        // Variables                                                                                                                
        public static Dictionary<string, double> ExistingParameters = null;


        // Methods                                                                                                                  
        static public double ConvertFromString(string valueString, Func<string, double> ConvertToCurrentUnits)
        {
            double valueDouble;
            valueString = valueString.Trim();
            //
            if (valueString.Length == 0 || valueString == "=") return 0;   // empty string -> 0
            if (!double.TryParse(valueString, out valueDouble))
            {
                if (valueString.StartsWith("="))
                {
                    valueString = valueString.Substring(1, valueString.Length - 1);
                    //List<string> parameters = GetParameters(valueString);
                    Expression e = GetExpression(valueString);
                    if (!e.HasErrors())
                    {
                        object result = e.Evaluate();
                        if (result is bool bl) valueDouble = bl ? 1 : 0;
                        else if (result is byte byt) valueDouble = byt;
                        else if (result is decimal dec) valueDouble = (double)dec;
                        else if (result is int i) valueDouble = i;
                        else if (result is float f) valueDouble = f;
                        else if (result is double d) valueDouble = d;
                        else
                        {
                            double.TryParse(result.ToString(), out valueDouble);
                        }
                    }
                    else
                    {
                        throw new CaeException("Equation error:" + Environment.NewLine + e.Error);
                    }
                }
                else valueDouble = ConvertToCurrentUnits(valueString);
            }
            //
            return valueDouble;
        }
        static public Expression GetExpression(string expression)
        {
            Expression e = new Expression(expression, EvaluateOptions.IgnoreCase);
            // Add constants
            e.Parameters.Add("pi", Math.PI);
            e.Parameters.Add("Pi", Math.PI);
            //
            if (ExistingParameters != null)
            {
                foreach (var entry in ExistingParameters) e.Parameters.Add(entry.Key, entry.Value);
            }
            //e.EvaluateParameter += EvaluateParameter;
            return e;
        }
        static public HashSet<string> GetParameters(string expression)
        {
            HashSet<string> parameters = new HashSet<string>();
            //Random random = new Random();
            Expression e = new Expression(expression);
            //
            e.EvaluateFunction += delegate (string name, FunctionArgs args) {
                args.EvaluateParameters();
                args.Result = 0;// random.Next(0, 100);
            };
            e.EvaluateParameter += delegate (string name, ParameterArgs args) {
                parameters.Add(name);
                args.Result = 0;// random.Next(0, 100);
            };
            try
            {
                e.Evaluate();
            }
            catch
            {
            }
            return parameters;
        }
        static public void EvaluateParameter(string name, ParameterArgs args)
        {
            //if (name == "Pi") args.Result = Math.PI;
        }
    }
}
