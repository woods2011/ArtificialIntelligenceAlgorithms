using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using SharedCL.Extension;

namespace AbcAlg.Models.BeesAlg.NeighborhoodSearching
{
    public class RndNborAndBestNborNs : NeighborhoodSearchingAlg
    {
        private readonly double _cFactor;

        private readonly Func<IList<FoodSource>, FoodSource> _bestSelector =
            population => population.MinBy(chr => chr.FuncValue);


        public RndNborAndBestNborNs((double a, double b) x1Bounds, (double a, double b) x2Bounds,
            Func<double, double, double> objFunc, double cFactor, Random? random = null)
            : base(x1Bounds, x2Bounds, objFunc, random) => _cFactor = cFactor;

        public virtual List<FoodSource> Search(IList<FoodSource> currentPopulation, FoodSource best)
        {
            return currentPopulation.Select(origFoodSource =>
            {
                var rndNeighbor = currentPopulation.GetRandomElementsExcept(origFoodSource, Random).First();
                var rndDimension = DiscreteUniform.Sample(Random, 1, 2);
                var changedFoodSource = origFoodSource.ShallowCopy();

                switch (rndDimension)
                {
                    case 1:
                        changedFoodSource.X1 = Math.Clamp(
                            origFoodSource.X1
                            + ContinuousUniform.Sample(Random, -1.0, 1.0) * (origFoodSource.X1 - rndNeighbor.X1)
                            + ContinuousUniform.Sample(Random, -1e-20, _cFactor) * (best.X1 - origFoodSource.X1),
                            X1Bounds.a, X1Bounds.b);
                        break;
                    case 2:
                        changedFoodSource.X2 = Math.Clamp(
                            origFoodSource.X2
                            + ContinuousUniform.Sample(Random, -1.0, 1.0) * (origFoodSource.X2 - rndNeighbor.X2)
                            + ContinuousUniform.Sample(Random, -1e-20, _cFactor) * (best.X2 - origFoodSource.X2),
                            X2Bounds.a, X2Bounds.b);
                        break;

                    default: throw new InvalidOperationException("Dimension out of bounds");
                }


                changedFoodSource.EvalFunction(ObjFunc);

                if (changedFoodSource.FuncValue + 1e-15 < origFoodSource.FuncValue)
                {
                    changedFoodSource.NumberOfVisits = 0;
                    return changedFoodSource;
                }

                origFoodSource.NumberOfVisits++;
                return origFoodSource;
            }).ToList();

            // foreach (var chr in currentPopulation)
            // {
            //     var rndNeighbor = currentPopulation.GetRandomElementsExcept(chr, Random).First();
            //     var rndDimension = DiscreteUniform.Sample(Random, 1, 2);
            //
            //     switch (rndDimension)
            //     {
            //         case 1:
            //             var newX1 = Math.Clamp(
            //                 chr.X1 + ContinuousUniform.Sample(Random, -1.0, 1.0) * (chr.X1 - rndNeighbor.X1)
            //                        + ContinuousUniform.Sample(Random, -1e-20, _cFactor) * (best.X1 - chr.X1),
            //                 X1Bounds.a, X1Bounds.b);
            //             var newX1FuncValue = ObjFunc(newX1, chr.X2);
            //             if (newX1FuncValue + 1e-10 < chr.FuncValue)
            //                 (chr.X1, chr.FuncValue, chr.NumberOfVisits) = (newX1, newX1FuncValue, 0);
            //             else
            //                 chr.NumberOfVisits++;
            //             break;
            //
            //         case 2:
            //             var newX2 = Math.Clamp(
            //                 chr.X2 + ContinuousUniform.Sample(Random, -1.0, 1.0) * (chr.X2 - rndNeighbor.X2)
            //                        + ContinuousUniform.Sample(Random, -1e-20, _cFactor) * (best.X2 - chr.X2),
            //                 X2Bounds.a, X2Bounds.b);
            //             var newX2FuncValue = ObjFunc(chr.X1, newX2);
            //             if (newX2FuncValue + 1e-10 < chr.FuncValue)
            //                 (chr.X2, chr.FuncValue, chr.NumberOfVisits) = (newX2, newX2FuncValue, 0);
            //             else
            //                 chr.NumberOfVisits++;
            //             break;
            //
            //         default: throw new InvalidOperationException("Dimension out of bounds");
            //     }
            // }

            //var tempPop = currentPopulation.Select(chr => new Chromosome(chr.X1, chr.X2, chr.FuncValue)).ToList();
        }

        public override List<FoodSource> Search(IList<FoodSource> currentPopulation) =>
            Search(currentPopulation, _bestSelector(currentPopulation));
    }
}