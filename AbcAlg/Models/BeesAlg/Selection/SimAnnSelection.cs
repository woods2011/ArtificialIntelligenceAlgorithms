using System;
using AbcAlg.Models.BeesAlg.TempUpdaters;

namespace AbcAlg.Models.BeesAlg.Selection
{
    public abstract class SimAnnealingSelection : SelectionAlg
    {
        protected readonly ITempUpdater TempUpdater;

        protected SimAnnealingSelection(ITempUpdater tempUpdater, Random? random = null) : base(random) =>
            TempUpdater = tempUpdater;

        protected bool AcceptanceProb(FoodSource chr, FoodSource refChr) =>
            chr.FuncValue <= refChr.FuncValue + 1e-15 ||
            Math.Exp(-Math.Abs(chr.FuncValue - refChr.FuncValue) / TempUpdater.CurTemp) >
            Random.NextDouble();
    }
}