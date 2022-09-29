using System;
using GeneticAlg.Models.GeneticAlg.Crossover;
using SharedWPF.ViewModels;

namespace GeneticAlg.ViewModels.Builders.Crossover
{
    public class ExtendedLineCrossoverBuilderVm : CrossoverBuilderVm
    {
        public ExtendedLineCrossoverBuilderVm(Random? random = null) : base(random)
        {
        }

        public override ExtendedLineCrossover Build(BoundsVm x1Bounds, BoundsVm x2Bounds) => new(x1Bounds.ToValTuple(), x2Bounds.ToValTuple(), Random)
        {
            CrossingProbability = CrossingProbability
        };
    }
}