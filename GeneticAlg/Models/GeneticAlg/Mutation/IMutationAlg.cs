using System.Collections.Generic;

namespace GeneticAlg.Models.GeneticAlg.Mutation
{
    public interface IMutationAlg
    {
        public void Mutate(IList<Chromosome> currentPopulation);
    }
}