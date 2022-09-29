using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using SharedCL;
using SharedCL.Extension;

namespace GeneticAlg.Models.GeneticAlg.Selection
{
    public class SusSelection : SelectionAlg
    {
        public SusSelection(Random? random = null) : base(random)
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


            chrWithAccWList = chrWithAccWList.GetRandomElements(Random).ToList(); // ShuffleList
            var distBetweenPoints = maxAccWeight / (double)outPopulationSize;
            var seedPoint = ContinuousUniform.Sample(Random, 0, distBetweenPoints);
            var randomPoints = Enumerable.Range(0, outPopulationSize).Select(i => seedPoint + i * distBetweenPoints)
                .ToList();
            List<Chromosome> selectedChromosomes = new();

            var (index, curChrWithAccW) = (0, chrWithAccWList.First());
            foreach (var point in randomPoints)
            {
                while (point > curChrWithAccW.accWeight)
                {
                    index++;
                    if (index < chrWithAccWList.Count) curChrWithAccW = chrWithAccWList[index];
                }
                selectedChromosomes.Add(curChrWithAccW.chr);
            }

            return selectedChromosomes;
        }
    }
}