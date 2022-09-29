using System;
using System.ComponentModel;
using GeneticAlg.Models.GeneticAlg.Mutation;
using SharedWPF.ViewModels;

namespace GeneticAlg.ViewModels.Builders.Mutation
{
    public abstract class MutationBuilderVm : INotifyPropertyChanged
    {
        protected Random Random { get; }

        public double MutationProbability { get; set; } = 0.1;


        protected MutationBuilderVm(Random? random = null)
        {
            Random = random ?? new Random();
        }

        public abstract MutationAlg Build(BoundsVm x1Bounds, BoundsVm x2Bounds);


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}