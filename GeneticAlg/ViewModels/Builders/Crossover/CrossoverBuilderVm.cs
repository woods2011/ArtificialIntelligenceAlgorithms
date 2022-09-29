using System;
using System.ComponentModel;
using GeneticAlg.Models.GeneticAlg.Crossover;
using SharedWPF.ViewModels;

namespace GeneticAlg.ViewModels.Builders.Crossover
{
    public abstract class CrossoverBuilderVm : INotifyPropertyChanged
    {
        protected Random Random { get; }

        public double CrossingProbability { get; set; } = 0.97;


        protected CrossoverBuilderVm(Random? random = null)
        {
            Random = random ?? new Random();
        }


        public abstract CrossoverAlg Build(BoundsVm x1Bounds, BoundsVm x2Bounds);

        
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}