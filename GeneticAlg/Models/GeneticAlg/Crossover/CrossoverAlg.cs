using System;
using System.Collections.Generic;

namespace GeneticAlg.Models.GeneticAlg.Crossover
{
    public abstract class CrossoverAlg : ICrossoverAlg
    {
        protected Random Random { get; }

        public (double a, double b) X1Bounds { get; }
        public (double a, double b) X2Bounds { get; }

        public double CrossingProbability { get; init; } = 0.97;

        protected CrossoverAlg((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null)
        {
            X1Bounds = x1Bounds;
            X2Bounds = x2Bounds;
            Random = random ?? new Random();
        }

        public abstract List<Chromosome> Crossover(IList<Chromosome> parents, int outPopulationSize);
    }
}