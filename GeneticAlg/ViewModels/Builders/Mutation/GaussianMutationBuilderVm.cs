using System;
using GeneticAlg.Models.GeneticAlg.Mutation;
using SharedWPF.ViewModels;

namespace GeneticAlg.ViewModels.Builders.Mutation
{
    public class GaussianMutationBuilderVm : MutationBuilderVm
    {
        public double StdDevPercent { get; set; } = 0.1;

        public GaussianMutationBuilderVm(Random? random = null) : base(random)
        {
        }
        

        public override GaussianMutation Build(BoundsVm x1Bounds, BoundsVm x2Bounds) => new(x1Bounds.ToValTuple(), x2Bounds.ToValTuple(), Random)
        {
            MutationProbability = MutationProbability,
            StdDevPercent = StdDevPercent
        };
    }
}