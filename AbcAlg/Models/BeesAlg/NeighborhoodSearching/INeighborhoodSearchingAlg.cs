using System.Collections.Generic;

namespace AbcAlg.Models.BeesAlg.NeighborhoodSearching
{
    public interface INeighborhoodSearchingAlg
    {
        public List<FoodSource> Search(IList<FoodSource> currentPopulation); // ToDo: make it mutable and return void
    }
}