using System;
using System.Collections.Generic;
using System.Linq;
using SharedCL;
using SharedCL.Extension;

namespace GeneticAlg.Models.GeneticAlg.Selection
{
    public class TournamentSelection : SelectionAlg
    {
        public int TournamentSize { get; init; } = 3;

        public TournamentSelection(Random? random = null) : base(random)
        {
        }

        public override List<Chromosome> Selection(IList<Chromosome> currentPopulation, int outPopulationSize) =>
            Enumerable.Range(0, outPopulationSize).Select(_ =>
                    currentPopulation.GetRandomElements(Random)
                        .Take(Math.Min(TournamentSize, currentPopulation.Count))
                        .MinBy(x => x.FuncValue))
                .ToList();
    }
}