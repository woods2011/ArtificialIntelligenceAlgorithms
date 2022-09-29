using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace AbcAlg.Models.BeesAlg.NeighborhoodSearching
{
    public class GaussianNs : NeighborhoodSearchingAlg
    {
        public double StdDevPercent { get; init; } = 0.1;

        public GaussianNs((double a, double b) x1Bounds, (double a, double b) x2Bounds,
            Func<double, double, double> objFunc, Random? random = null) : base(x1Bounds, x2Bounds, objFunc, random)
        {
        }


        public override List<FoodSource> Search(IList<FoodSource> currentPopulation)
        {
            return currentPopulation.Select(origFoodSource =>
            {
                var rndDimension = DiscreteUniform.Sample(Random, 1, 2);
                var changedFoodSource = origFoodSource.ShallowCopy();

                switch (rndDimension)
                {
                    case 1:
                        changedFoodSource.X1 = Math.Clamp(
                            Normal.Sample(Random, origFoodSource.X1, StdDevPercent * (X1Bounds.b - X1Bounds.a)),
                            X1Bounds.a, X1Bounds.b);
                        break;
                    case 2:
                        changedFoodSource.X2 = Math.Clamp(
                            Normal.Sample(Random, origFoodSource.X2, StdDevPercent * (X2Bounds.b - X2Bounds.a)),
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
            //     var rndDimension = DiscreteUniform.Sample(Random, 1, 2);
            //
            //     switch (rndDimension)
            //     {
            //         case 1:
            //             var newX1 = Math.Clamp(Normal.Sample(Random, chr.X1, StdDevPercent * (X1Bounds.b - X1Bounds.a)),
            //                 X1Bounds.a, X1Bounds.b);
            //             var newX1FuncValue = ObjFunc(newX1, chr.X2);
            //             if (newX1FuncValue + 1e-15 < chr.FuncValue)
            //                 (chr.X1, chr.FuncValue, chr.NumberOfVisits) = (newX1, newX1FuncValue, 0);
            //             else
            //                 chr.NumberOfVisits++;
            //             break;
            //
            //         case 2:
            //             var newX2 = Math.Clamp(Normal.Sample(Random, chr.X2, StdDevPercent * (X2Bounds.b - X2Bounds.a)),
            //                 X2Bounds.a, X2Bounds.b);
            //             var newX2FuncValue = ObjFunc(chr.X1, newX2);
            //             if (newX2FuncValue + 1e-15 < chr.FuncValue)
            //                 (chr.X2, chr.FuncValue, chr.NumberOfVisits) = (newX2, newX2FuncValue, 0);
            //             else
            //                 chr.NumberOfVisits++;
            //             break;
            //
            //         default: throw new InvalidOperationException("Dimension out of bounds");
            //     }
            // }
        }
    }
}