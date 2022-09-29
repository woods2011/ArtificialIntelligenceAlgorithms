using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using SharedCL.Extension;

namespace AbcAlg.Models.BeesAlg.Selection
{
    public class SusSelection : SelectionAlg
    {
        public SusSelection(Random? random = null) : base(random)
        {
        }

        public override List<FoodSource> Selection(IList<FoodSource> curPopulation, int maxOutputPopSize)
        {
            var sortedDescByFuncPopulation = curPopulation.OrderByDescending(chr => chr.FuncValue).ToList();

            List<(FoodSource chr, int accWeight)> chrWithAccWList = new(sortedDescByFuncPopulation.Count);
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
            var distBetweenPoints = maxAccWeight / (double)maxOutputPopSize;
            var seedPoint = ContinuousUniform.Sample(Random, 0, distBetweenPoints);
            var randomPoints = Enumerable.Range(0, maxOutputPopSize).Select(i => seedPoint + i * distBetweenPoints)
                .ToList();
            List<FoodSource> selectedChromosomes = new();

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