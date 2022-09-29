using System;
using System.Collections.Generic;
using System.Linq;
using AbcAlg.Models.BeesAlg.TempUpdaters;
using SharedCL.Extension;

namespace AbcAlg.Models.BeesAlg.Selection
{
    public class SimAnnealingByRndPairsSelection : SimAnnealingSelection
    {
        public SimAnnealingByRndPairsSelection(ITempUpdater tempUpdater, Random? random = null)
            : base(tempUpdater, random)
        {
        }

        public override List<FoodSource> Selection(IList<FoodSource> curPopulation, int maxOutputPopSize)
        {
            var result = curPopulation.GetRandomUniquePairsIndexes(Random)
                .Select(chrsIndexes =>
                    (chr: curPopulation[chrsIndexes.Item1], refChr: curPopulation[chrsIndexes.Item2]))
                .Where(chrs => AcceptanceProb(chrs.chr, chrs.refChr))
                .Select(chrs => chrs.chr)
                .Take(maxOutputPopSize)
                .ToList();
            TempUpdater.UpdateTemperature();
            return result;
            // if (maxOutputPopSize > result.Count)
            //     result.AddRange(
            //         Enumerable.Repeat(result.MinBy(chr => chr.FuncValue), maxOutputPopSize));
        }
    }
}