using System;
using System.Collections.Generic;
using System.Linq;
using AbcAlg.Models.BeesAlg.TempUpdaters;

namespace AbcAlg.Models.BeesAlg.Selection
{
    public class SimAnnealingByBestSelection : SimAnnealingByBestSelectionA
    {
        public SimAnnealingByBestSelection(ITempUpdater tempUpdater, Random? random = null) : base(tempUpdater, random)
        {
        }

        public override List<FoodSource> Selection(IList<FoodSource> curPopulation, FoodSource refChr,
            int maxOutputPopSize)
        {
            var result = curPopulation
                .Where(chr => AcceptanceProb(chr, refChr))
                .Take(maxOutputPopSize)
                .ToList();
            TempUpdater.UpdateTemperature();
            return result; // ToDo: check ToList here
        }
    }
}