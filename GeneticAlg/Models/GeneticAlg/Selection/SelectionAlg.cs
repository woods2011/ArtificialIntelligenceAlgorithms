using System;
using System.Collections.Generic;

namespace GeneticAlg.Models.GeneticAlg.Selection
{
    public abstract class SelectionAlg : ISelectionAlg
    {
        protected Random Random { get; }

        protected SelectionAlg(Random? random = null)
        {
            Random = random ?? new Random();
        }

        public abstract List<Chromosome> Selection(IList<Chromosome> currentPopulation, int outPopulationSize);
    }
}