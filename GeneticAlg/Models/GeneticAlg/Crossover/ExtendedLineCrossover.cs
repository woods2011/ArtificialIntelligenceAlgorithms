using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using SharedCL;
using SharedCL.Extension;

namespace GeneticAlg.Models.GeneticAlg.Crossover
{
    public class ExtendedLineCrossover : CrossoverAlg
    {
        public ExtendedLineCrossover((double a, double b) x1Bounds, (double a, double b) x2Bounds,
            Random? random = null) : base(x1Bounds, x2Bounds, random)
        {
        }

        public override List<Chromosome> Crossover(IList<Chromosome> parents, int outPopulationSize) =>
            parents.GetRandomUniquePairsIndexes(Random).Take(outPopulationSize)
                .Select<(int parent1, int parent2), Chromosome>(parentsIndexes =>
                    CrossingProbability > Random.NextDouble()
                        ? CrossoverBody(parents[parentsIndexes.parent1], parents[parentsIndexes.parent2])
                        : parents[Random.NextDouble() > 0.5 ? parentsIndexes.parent1 : parentsIndexes.parent2])
                .ToList();


        protected virtual Chromosome CrossoverBody(Chromosome parent1, Chromosome parent2)
        {
            var w = ContinuousUniform.Sample(Random, -0.25, 1.25);
            return new Chromosome
            {
                X1 = Math.Clamp(parent1.X1 + w * (parent2.X1 - parent1.X1), X1Bounds.a, X1Bounds.b),
                X2 = Math.Clamp(parent1.X2 + w * (parent2.X2 - parent1.X2), X2Bounds.a, X2Bounds.b)
            };
        }
    }
}