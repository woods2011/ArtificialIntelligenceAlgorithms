using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace GeneticAlg.Models.GeneticAlg.Mutation
{
    public class GaussianMutation : MutationAlg
    {
        public double StdDevPercent { get; init; } = 0.1;

        public GaussianMutation((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null) :
            base(x1Bounds, x2Bounds, random)
        {
        }


        public override void Mutate(IList<Chromosome> currentPopulation)
        {
            for (var i = 0; i < currentPopulation.Count; i++)
            {
                var isMutationInFirstDim = MutationProbability > Random.NextDouble();
                var isMutationInSecondDim = MutationProbability > Random.NextDouble();

                if (!isMutationInFirstDim && !isMutationInSecondDim) continue;

                var mutatedChr = currentPopulation[i].ShallowCopy();
                if (isMutationInFirstDim)
                    mutatedChr.X1 = Math.Clamp(
                        Normal.Sample(Random, mutatedChr.X1, StdDevPercent * (X1Bounds.b - X1Bounds.a)),
                        X1Bounds.a, X1Bounds.b);
                if (isMutationInSecondDim)
                    mutatedChr.X2 = Math.Clamp(
                        Normal.Sample(Random, mutatedChr.X2, StdDevPercent * (X2Bounds.b - X2Bounds.a)),
                        X2Bounds.a, X2Bounds.b);
                currentPopulation[i] = mutatedChr;
            }
            // foreach (var chromosome in currentPopulation)
            // {
            //     chromosome.X1 = MutationProbability < Random.NextDouble()
            //         ? chromosome.X1
            //         : Math.Clamp(
            //             Normal.Sample(Random, chromosome.X1, StdDevPercent * (X1Bounds.b - X1Bounds.a)),
            //             X1Bounds.a, X1Bounds.b);
            //     chromosome.X2 = MutationProbability < Random.NextDouble()
            //         ? chromosome.X2
            //         : Math.Clamp(
            //             Normal.Sample(Random, chromosome.X2, StdDevPercent * (X2Bounds.b - X2Bounds.a)),
            //             X2Bounds.a, X2Bounds.b);
            // }
        }
    }
}