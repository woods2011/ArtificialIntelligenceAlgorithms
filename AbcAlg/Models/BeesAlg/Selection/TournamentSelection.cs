using System;
using System.Collections.Generic;
using System.Linq;
using SharedCL.Extension;

namespace AbcAlg.Models.BeesAlg.Selection
{
    public class TournamentSelection : SelectionAlg
    {
        public int TournamentSize { get; init; } = 3;

        public TournamentSelection(Random? random = null) : base(random)
        {
        }

        public override List<FoodSource> Selection(IList<FoodSource> curPopulation, int maxOutputPopSize) => Enumerable
            .Range(0, maxOutputPopSize)
            .Select(_ => curPopulation
                .GetRandomElements(Random)
                .Take(TournamentSize)
                .MinBy(x => x.FuncValue))
            .ToList();
    }
}