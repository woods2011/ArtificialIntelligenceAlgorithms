using System;
using GeneticAlg.Models.GeneticAlg.Selection;

namespace GeneticAlg.ViewModels.Builders.Selection
{
    public class RouletteSelectionBuilderVm : SelectionBuilderVm
    {
        public RouletteSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override RouletteSelection Build() => new(Random);
    }
}