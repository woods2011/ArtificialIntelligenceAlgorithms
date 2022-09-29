using System;
using GeneticAlg.Models.GeneticAlg.Selection;

namespace GeneticAlg.ViewModels.Builders.Selection
{
    public class TournamentSelectionBuilderVm : SelectionBuilderVm
    {
        public int TournamentSize { get; set; } = 3;

        public TournamentSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override TournamentSelection Build() => new(Random) {TournamentSize = TournamentSize};
    }
}