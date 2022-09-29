using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace GeneticAlg.Models.GeneticAlg.Selection
{
    public class RouletteSelection : SelectionAlg
    {
        public RouletteSelection(Random? random = null) : base(random)
        {
        }

        public override List<Chromosome> Selection(IList<Chromosome> currentPopulation, int outPopulationSize)
        {
            var sortedDescByFuncPopulation = currentPopulation.OrderByDescending(chr => chr.FuncValue).ToList();

            List<(Chromosome chr, int accWeight)> chrWithAccWList = new(sortedDescByFuncPopulation.Count);
            var (weight, accWeight, prevChr) = (1, 0, sortedDescByFuncPopulation.First());
            foreach (var chromosome in sortedDescByFuncPopulation)
            {
                if (Math.Abs(prevChr.FuncValue - chromosome.FuncValue) > 1e-15)
                    weight++;
                accWeight += weight;
                chrWithAccWList.Add((chromosome, accWeight));
            }

            var maxAccWeight = chrWithAccWList.Last().accWeight;

            return DiscreteUniform.Samples(Random, 0, maxAccWeight - 1)
                .Take(outPopulationSize)
                .Select(randomAccWeight =>
                    chrWithAccWList.First(chrWithAccW => chrWithAccW.accWeight > randomAccWeight).chr)
                .ToList();
        }
    }
}