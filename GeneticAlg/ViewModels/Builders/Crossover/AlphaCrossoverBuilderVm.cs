using System;
using GeneticAlg.Models.GeneticAlg.Crossover;
using SharedWPF.ViewModels;

namespace GeneticAlg.ViewModels.Builders.Crossover
{
    public class AlphaCrossoverBuilderVm : CrossoverBuilderVm
    {
        public double Alpha { get; set; } = 0.5;

        public AlphaCrossoverBuilderVm(Random? random = null) : base(random)
        {
        }

        public override AlphaCrossover Build(BoundsVm x1Bounds, BoundsVm x2Bounds) => new(x1Bounds.ToValTuple(), x2Bounds.ToValTuple(), Random)
        {
            CrossingProbability = CrossingProbability,
            Alpha = Alpha
        };
    }
}