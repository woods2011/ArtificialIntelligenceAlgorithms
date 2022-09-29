using System.Collections.Generic;

namespace GeneticAlg.Models.GeneticAlg.Crossover
{
    public interface ICrossoverAlg
    {
        public List<Chromosome> Crossover(IList<Chromosome> parents, int outPopulationSize);
    }
}