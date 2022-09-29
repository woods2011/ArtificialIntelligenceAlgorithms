using System;
using System.ComponentModel;
using GeneticAlg.Models.GeneticAlg.Selection;

namespace GeneticAlg.ViewModels.Builders.Selection
{
    public abstract class SelectionBuilderVm : INotifyPropertyChanged
    {
        protected Random Random { get; }

        protected SelectionBuilderVm(Random? random = null)
        {
            Random = random ?? new Random();
        }

        public abstract SelectionAlg Build();
        

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}