using System;

namespace AbcAlg.Models.BeesAlg
{
    public class BeesAlgSettings
    {
        public int MaxIterationsCount { get; init; }
        public int FoodSourcesCount { get; init; }
        public int MaxNumOfVisits { get; init; }

        public Func<double, double, double> ObjectiveFunction { get; init; } =
            (x1, x2) => 4 * Math.Pow(x1 - 5, 2) + Math.Pow(x2 - 6, 2);

        public (double a, double b) X1Bounds { get; init; }
        public (double a, double b) X2Bounds { get; init; }
    }
}