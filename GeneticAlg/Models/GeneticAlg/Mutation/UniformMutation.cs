using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace GeneticAlg.Models.GeneticAlg.Mutation
{
    public class UniformMutation : MutationAlg
    {
        public UniformMutation((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null) :
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
                    mutatedChr.X1 = ContinuousUniform.Sample(Random, X1Bounds.a, X1Bounds.b);
                if (isMutationInSecondDim)
                    mutatedChr.X2 = ContinuousUniform.Sample(Random, X2Bounds.a, X2Bounds.b);
                currentPopulation[i] = mutatedChr;
            }
        }
    }
}