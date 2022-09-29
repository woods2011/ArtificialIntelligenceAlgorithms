using System;
using System.Collections.Generic;

namespace AbcAlg.Models.BeesAlg.Selection
{
    public abstract class SelectionAlg : ISelectionAlg
    {
        protected Random Random { get; }

        protected SelectionAlg(Random? random = null) => Random = random ?? new Random();

        public abstract List<FoodSource> Selection(IList<FoodSource> curPopulation, int maxOutputPopSize);
    }
}