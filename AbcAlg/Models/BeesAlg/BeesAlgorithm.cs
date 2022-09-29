using System;
using System.Collections.Generic;
using System.Linq;
using AbcAlg.Models.BeesAlg.NeighborhoodSearching;
using AbcAlg.Models.BeesAlg.Selection;
using AbcAlg.Models.BeesAlg.TempUpdaters;
using MathNet.Numerics.Distributions;
using SharedCL.Extension;

namespace AbcAlg.Models.BeesAlg
{
    public class BeesAlgorithm
    {
        private const double Eps = 0.00001;

        private readonly Random _random;
        private readonly BeesAlgSettings _algSettings;

        private readonly int _foodSourcesCount;
        private int _scoutsCount;
        private List<FoodSource> _scouts = new();
        private List<FoodSource> _workers = new();
        private FoodSource _best = new();

        private readonly INeighborhoodSearchingAlg _neighborhoodSearchingStrategy;
        private readonly ISelectionAlg _selectionStrategy;
        private readonly ISelectionAlg? _scoutsSelectionStrategy;

        public Func<double, double, double> ObjectiveFunction { get; }


        public BeesAlgorithm(BeesAlgSettings algSettings, Random? randomSource = null)
        {
            ObjectiveFunction = algSettings.ObjectiveFunction;
            _algSettings = algSettings;
            _scoutsCount = algSettings.FoodSourcesCount;
            _foodSourcesCount = algSettings.FoodSourcesCount;
            _random = randomSource ?? new Random();

            _neighborhoodSearchingStrategy = new RndNborAndBestNborNs(
                algSettings.X1Bounds, algSettings.X2Bounds,
                algSettings.ObjectiveFunction, 0.5, _random);

            _selectionStrategy = new SimAnnealingByRndElExtSelection(
                new SimpleTempUpdater(100, algSettings.MaxIterationsCount, 1.0), _random);

            _scoutsSelectionStrategy = new SimAnnealingByBestSelection(
                new SimpleTempUpdater(100, algSettings.MaxIterationsCount, 1.0), _random);
        }

        public BeesAlgorithm(BeesAlgSettings algSettings, INeighborhoodSearchingAlg neighborhoodSearchingStrategy,
            ISelectionAlg selectionStrategy, ISelectionAlg? scoutsSelectionStrategy = null, Random? randomSource = null)
        {
            ObjectiveFunction = algSettings.ObjectiveFunction;
            _algSettings = algSettings;
            _scoutsCount = algSettings.FoodSourcesCount;
            _foodSourcesCount = algSettings.FoodSourcesCount;
            _random = randomSource ?? new Random();

            _neighborhoodSearchingStrategy = neighborhoodSearchingStrategy;
            _selectionStrategy = selectionStrategy;
            _scoutsSelectionStrategy = scoutsSelectionStrategy;
        }


        private List<FoodSource> GenerateScouts() => Enumerable.Range(0, _scoutsCount)
            .Select(_ => new FoodSource(
                x1: ContinuousUniform.Sample(_random, _algSettings.X1Bounds.a, _algSettings.X1Bounds.b),
                x2: ContinuousUniform.Sample(_random, _algSettings.X2Bounds.a, _algSettings.X2Bounds.b),
                ObjectiveFunction))
            .ToList();

        private void AlgorithmBody()
        {
            // Employed Bee Phase
            _workers = _neighborhoodSearchingStrategy.Search(_workers);
            _best = _workers.MinBy(chr => chr.FuncValue);
            _workers.RemoveAll(chr => chr.NumberOfVisits >= _algSettings.MaxNumOfVisits);
            if (!_workers.Contains(_best))
            {
                _best.NumberOfVisits = 0;
                _workers.Add(_best);
            }
            
            // Scouts Bee Phase
            _scoutsCount = Math.Max(0, _foodSourcesCount - _workers.Count);
            if (_scoutsCount > 0)
            {
                _scouts = GenerateScouts();
                _best = _scouts.Concat(_workers).MinBy(chr => chr.FuncValue);

                _workers.AddRange(_scoutsSelectionStrategy switch
                {
                    null => _scouts,
                    SimAnnealingByBestSelectionA selectionByRef =>
                        selectionByRef.Selection(_scouts, _best, _scoutsCount),
                    _ => _scoutsSelectionStrategy.Selection(_scouts, _scoutsCount)
                });
                _best = _workers.MinBy(chr => chr.FuncValue);
            }

            // Onlooker Bee Phase
            _workers = _workers.Count >= 2 ? _selectionStrategy.Selection(_workers, _workers.Count) : _workers;
            if (!_workers.Contains(_best))
                _workers[_workers.IndexOf(_workers.MaxBy(chr => chr.FuncValue))] = _best;
        }


        public FoodSource FindMinFunction()
        {
            _workers = GenerateScouts();
            _best = _workers.MinBy(chr => chr.FuncValue);
            
            for (var i = 0; i < _algSettings.MaxIterationsCount; i++)
            {
                AlgorithmBody();
            }

            return _best;
        }

        public IEnumerable<List<FoodSource>> FindMinFunctionAndSave()
        {
            _workers = GenerateScouts();
            _best = _workers.MinBy(chr => chr.FuncValue);
            yield return _workers.Select(chr => chr.ShallowCopy()).ToList();
            
            for (var i = 0; i < _algSettings.MaxIterationsCount; i++)
            {
                AlgorithmBody();
                yield return _workers.Select(chr => chr.ShallowCopy()).ToList();
            }
        }
    }
}


// var newWorkers2 = _workers.Select(chr => new Chromosome
// {
//     X1 = _best.X1 + ContinuousUniform.Sample(_random, -1, 1) * (chr.X1 - _best.X1),
//     X2 = _best.X2 + ContinuousUniform.Sample(_random, -1, 1) * (chr.X2 - _best.X2)
// }).ToList();
// newWorkers2.EvalFunction(ObjectiveFunction);

// var sumFunc = newWorkers.Sum(chr => chr.FuncValue);
// var normalizedWorkers = newWorkers.Select(chr =>
//     new { Chromosome = chr, NormalizedFunc = chr.FuncValue / sumFunc });
// _workers = normalizedWorkers.SelectMany(chrWithNormValue =>
//     Enumerable.Repeat(chrWithNormValue.Chromosome, chrWithNormValue.NormalizedFunc switch
//     {
//         _ => _random.NextDouble() > 0.3 ? 0 : 1
//     })).ToList();