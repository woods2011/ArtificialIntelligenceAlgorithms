using System;
using GeneticAlg.Models.GeneticAlg.Mutation;
using SharedWPF.ViewModels;

namespace GeneticAlg.ViewModels.Builders.Mutation
{
    public class UniformMutationBuilderVm : MutationBuilderVm
    {
        public UniformMutationBuilderVm(Random? random = null) : base(random)
        {
        }

        public override UniformMutation Build(BoundsVm x1Bounds, BoundsVm x2Bounds) => new(x1Bounds.ToValTuple(), x2Bounds.ToValTuple(), Random)
        {
            MutationProbability = MutationProbability
        };
    }
}