using System;
using System.Collections.Generic;
using System.Linq;
using AbcAlg.Models.BeesAlg.TempUpdaters;
using SharedCL.Extension;

namespace AbcAlg.Models.BeesAlg.Selection
{
    public class SimAnnealingByRndElExtSelection : SimAnnealingSelection
    {
        public SimAnnealingByRndElExtSelection(ITempUpdater tempUpdater, Random? random = null)
            : base(tempUpdater, random)
        {
        }

        public override List<FoodSource> Selection(IList<FoodSource> curPopulation, int maxOutputPopSize)
        {
            var result = curPopulation
                .IterateInfinitely()
                .Take(maxOutputPopSize * maxOutputPopSize) // Bound max enumeration count
                .Where(chr =>
                    AcceptanceProb(chr, curPopulation.GetRandomElementsExcept(chr, Random).FirstOrDefault() ?? chr))
                .Take(maxOutputPopSize)
                .ToList();
            TempUpdater.UpdateTemperature();
            return result;
        }
    }
}