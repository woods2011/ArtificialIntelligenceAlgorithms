using System;
using System.Collections.Generic;
using AbcAlg.Models.BeesAlg.TempUpdaters;
using SharedCL.Extension;

namespace AbcAlg.Models.BeesAlg.Selection
{
    public abstract class SimAnnealingByBestSelectionA : SimAnnealingSelection
    {
        protected Func<IList<FoodSource>, FoodSource> RefSelector { get; } =
            population => population.MinBy(chr => chr.FuncValue);

        protected SimAnnealingByBestSelectionA(ITempUpdater tempUpdater, Random? random = null) : base(tempUpdater,
            random)
        {
        }

        public sealed override List<FoodSource> Selection(IList<FoodSource> curPopulation, int maxOutputPopSize) =>
            Selection(curPopulation, RefSelector(curPopulation), maxOutputPopSize);

        public abstract List<FoodSource> Selection(IList<FoodSource> curPopulation, FoodSource refChr,
            int maxOutputPopSize);
    }
}