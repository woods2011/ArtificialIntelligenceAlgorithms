using System.Collections.Generic;

namespace AbcAlg.Models.BeesAlg.Selection
{
    public interface ISelectionAlg
    {
        public List<FoodSource> Selection(IList<FoodSource> curPopulation, int maxOutputPopSize);
    }
}