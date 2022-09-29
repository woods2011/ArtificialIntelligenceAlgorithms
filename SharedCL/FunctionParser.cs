using System;
using System.Collections.Generic;
using System.Linq;
using Jace;

namespace SharedCL
{
    public static class FunctionParser
    {
        public static Func<double, double, double> ParseFunction2D(string functionStr)
        {
            var calculationEngine = new CalculationEngine();
            var formula = calculationEngine.Build(functionStr);
            var variables = new Dictionary<string, double>();

            return (x1, x2) =>
            {
                variables["x1"] = x1;
                variables["x2"] = x2;
                return formula(variables);
            };
        }
        
        public static Func<double[], double> ParseFunction(string functionStr, int dim)
        {
            var calculationEngine = new CalculationEngine();
            var formula = calculationEngine.Build(functionStr);

            var variables = Enumerable.Range(0, dim).ToDictionary(d => $"x{d + 1}", _ => 0.0);
            var keys = variables.Keys.ToArray();

            return x =>
            {
                for (var i = 0; i < dim; i++) variables[keys[i]] = x[i];
                return formula(variables);
            };
        }
    }
}