using System.Collections.Generic;

namespace GeneticAlg.Models.GeneticAlg.Selection
{
    public interface ISelectionAlg
    {
        public List<Chromosome> Selection(IList<Chromosome> currentPopulation, int outPopulationSize);
    }
}