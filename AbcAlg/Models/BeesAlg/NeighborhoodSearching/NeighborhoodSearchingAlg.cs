using System;
using System.Collections.Generic;

namespace AbcAlg.Models.BeesAlg.NeighborhoodSearching
{
    public abstract class NeighborhoodSearchingAlg : INeighborhoodSearchingAlg
    {
        protected readonly Func<double, double, double> ObjFunc;

        protected Random Random { get; }

        public (double a, double b) X1Bounds { get; }
        public (double a, double b) X2Bounds { get; }


        protected NeighborhoodSearchingAlg((double a, double b) x1Bounds, (double a, double b) x2Bounds,
            Func<double, double, double> objFunc, Random? random = null)
        {
            X1Bounds = x1Bounds;
            X2Bounds = x2Bounds;
            ObjFunc = objFunc;
            Random = random ?? new Random();
        }


        public abstract List<FoodSource> Search(IList<FoodSource> currentPopulation);
    }
}