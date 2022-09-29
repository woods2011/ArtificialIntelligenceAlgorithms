using System;
using System.Collections.Generic;

namespace GeneticAlg.Models.GeneticAlg.Mutation
{
    public abstract class MutationAlg : IMutationAlg
    {
        protected Random Random { get; }

        public (double a, double b) X1Bounds { get; }
        public (double a, double b) X2Bounds { get; }

        public double MutationProbability { get; init; } = 0.1;

        protected MutationAlg((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null)
        {
            X1Bounds = x1Bounds;
            X2Bounds = x2Bounds;
            Random = random ?? new Random();
        }


        public abstract void Mutate(IList<Chromosome> currentPopulation);
    }
}