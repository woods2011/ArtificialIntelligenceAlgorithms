using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using SharedCL;
using SharedCL.Extension;

namespace GeneticAlg.Models.GeneticAlg.Crossover
{
    public class AlphaCrossover : CrossoverAlg
    {
        public double Alpha { get; init; } = 0.5;

        public AlphaCrossover((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null) :
            base(x1Bounds, x2Bounds, random)
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
            var (x1Min, x1Max) = (Math.Min(parent1.X1, parent2.X1), Math.Max(parent1.X1, parent2.X1));
            var (x2Min, x2Max) = (Math.Min(parent1.X2, parent2.X2), Math.Max(parent1.X2, parent2.X2));
            var (i1, i2) = (x1Max - x1Min, x2Max - x2Min);

            return new Chromosome
            {
                X1 = Math.Clamp(ContinuousUniform.Sample(Random, x1Min - Alpha * i1, x1Max + Alpha * i1), X1Bounds.a,
                    X1Bounds.b),
                X2 = Math.Clamp(ContinuousUniform.Sample(Random, x2Min - Alpha * i2, x2Max + Alpha * i2), X2Bounds.a,
                    X2Bounds.b)
            };
        }
    }
}