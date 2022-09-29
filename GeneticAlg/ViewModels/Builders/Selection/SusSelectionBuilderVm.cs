using System;
using GeneticAlg.Models.GeneticAlg.Selection;

namespace GeneticAlg.ViewModels.Builders.Selection
{
    public class SusSelectionBuilderVm : SelectionBuilderVm
    {
        public SusSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override SusSelection Build() => new(Random);
    }
}