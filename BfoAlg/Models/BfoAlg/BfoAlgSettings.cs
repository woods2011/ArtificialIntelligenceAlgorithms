using System;
using System.Linq;

namespace BfoAlg.Models.BfoAlg
{
    public class BfoAlgSettings
    {
        public int ColonySize { get; init; } = 26;
        public int NumChemotacticLoops { get; init; } = 14;
        public int NumSwimLoops { get; init; } = 6;
        public int NumReproduceElimLoops { get; init; } = 8;
        public int NumElimDispLoops { get; init; } = 3;
        public double ProbElimDisp { get; init; } = 0.25;
        public double StepSize { get; init; } = 0.075;

        public BfoAlgSettings((double a, double b)[] xBounds) => (XBounds, Dim) = (xBounds, xBounds.Length);

        public BfoAlgSettings(int dim) : this(Enumerable.Repeat((-10.0, 10.0), dim).ToArray())
        {
        }


        public Func<double[], double> ObjectiveFunction { get; init; } =
            x => 4 * Math.Pow(x[0] - 5, 2) + Math.Pow(x[1] - 6, 2);

        public (double a, double b)[] XBounds { get; }
        public int Dim { get; }
    }
}