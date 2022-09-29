using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using SharedCL.Extension;

namespace BfoAlg.Models.BfoAlg
{
    public class BfoAlgorithm
    {
        private const double Eps = 0.00001;
        private readonly Random _random;

        private readonly BfoAlgSettings _algSettings;
        private readonly Bacterium[] _colony;
        public Bacterium BestSolution { get; private set; }

        public Func<double[], double> ObjectiveFunction { get; }


        public BfoAlgorithm(BfoAlgSettings algSettings, Random? randomSource = null)
        {
            ObjectiveFunction = algSettings.ObjectiveFunction;
            _algSettings = algSettings;
            _random = randomSource ?? new Random();

            _colony = GenerateInitSolution();
            BestSolution = _colony.MinBy(bacterium => bacterium.FuncValue).DeepCopy();
        }


        private Bacterium[] GenerateInitSolution() => Enumerable.Range(0, _algSettings.ColonySize)
            .Select(_ => GenerateNewBacterium())
            .ToArray();

        private Bacterium GenerateNewBacterium()
        {
            return new Bacterium(
                Enumerable.Range(0, _algSettings.Dim)
                    .Select(dim =>
                        ContinuousUniform.Sample(_random, _algSettings.XBounds[dim].a, _algSettings.XBounds[dim].b))
                    .ToArray(),
                ObjectiveFunction);
        }


        public Bacterium FindMin()
        {
            for (var l = 0; l < _algSettings.NumElimDispLoops; l++) // Eliminate-disperse steps
            {
                for (var k = 0; k < _algSettings.NumReproduceElimLoops; k++) // Reproduce-eliminate steps
                {
                    for (var i = 0; i < _algSettings.ColonySize; i++) _colony[i].Health = 0.0;

                    for (var j = 0; j < _algSettings.NumChemotacticLoops; j++) // Chemotactic steps
                    {
                        for (var i = 0; i < _algSettings.ColonySize; i++)
                        {
                            var tumbleVector = new double[_algSettings.Dim];
                            for (var d = 0; d < _algSettings.Dim; d++)
                                tumbleVector[d] = ContinuousUniform.Sample(_random, -1, 1);

                            var tumbleVNorm = 0.0;
                            for (var d = 0; d < _algSettings.Dim; d++) tumbleVNorm += tumbleVector[d] * tumbleVector[d];

                            for (var d = 0; d < _algSettings.Dim; d++)
                            {
                                _colony[i].X[d] = Math.Clamp(
                                    _colony[i].X[d] + (_algSettings.StepSize * tumbleVector[d]) /
                                    Math.Sqrt(tumbleVNorm),
                                    _algSettings.XBounds[d].a, _algSettings.XBounds[d].b);
                            }

                            _colony[i].EvalFunction(ObjectiveFunction);

                            var m = 0;
                            while (m < _algSettings.NumSwimLoops) // Swim steps
                            {
                                var tempPosition = new double[_algSettings.Dim];
                                _colony[i].X.CopyTo(tempPosition, 0);

                                for (var d = 0; d < _algSettings.Dim; d++)
                                {
                                    _colony[i].X[d] = Math.Clamp(
                                        _colony[i].X[d] + (_algSettings.StepSize * tumbleVector[d]) /
                                        Math.Sqrt(tumbleVNorm),
                                        _algSettings.XBounds[d].a, _algSettings.XBounds[d].b);
                                }

                                var nextFuncValue = ObjectiveFunction(_colony[i].X);

                                if (nextFuncValue > _colony[i].FuncValue + 1e-15)
                                {
                                    _colony[i].X = tempPosition;
                                    break;
                                }

                                _colony[i].FuncValue = nextFuncValue;
                                m++;
                            }

                            if (_colony[i].FuncValue < BestSolution.FuncValue) BestSolution = _colony[i].DeepCopy();

                            _colony[i].Health += _colony[i].FuncValue;
                        }
                    }

                    Array.Sort(_colony);
                    for (var left = 0; left < _algSettings.ColonySize / 2; left++)
                        _colony[left + _algSettings.ColonySize / 2] = _colony[left].DeepCopy();
                }

                for (var i = 0; i < _algSettings.ColonySize; i++)
                {
                    if (_random.NextDouble() > _algSettings.ProbElimDisp) continue;

                    _colony[i] = GenerateNewBacterium();
                    if (_colony[i].FuncValue < BestSolution.FuncValue) BestSolution = _colony[i].DeepCopy();
                }
            }

            return BestSolution;
        }

        public IEnumerable<List<Bacterium>> FindMinFunctionAndSaveSteps()
        {
            yield return _colony.DeepCopy().ToList();

            for (var l = 0; l < _algSettings.NumElimDispLoops; l++) // Eliminate-disperse loop
            {
                for (var k = 0; k < _algSettings.NumReproduceElimLoops; k++) // Reproduce-eliminate loop
                {
                    _colony.ResetHealth();

                    for (var j = 0; j < _algSettings.NumChemotacticLoops; j++) // Chemotactic loop
                    {
                        for (var i = 0; i < _algSettings.ColonySize; i++)
                        {
                            var tumbleVector = new double[_algSettings.Dim];
                            for (var d = 0; d < _algSettings.Dim; d++)
                                tumbleVector[d] = ContinuousUniform.Sample(_random, -1, 1);

                            var tumbleVNorm = 0.0;
                            for (var d = 0; d < _algSettings.Dim; d++) tumbleVNorm += tumbleVector[d] * tumbleVector[d];
                            tumbleVNorm = Math.Sqrt(tumbleVNorm);

                            for (var d = 0; d < _algSettings.Dim; d++)
                            {
                                _colony[i].X[d] = Math.Clamp(
                                    _colony[i].X[d] + (_algSettings.StepSize * tumbleVector[d]) / tumbleVNorm,
                                    _algSettings.XBounds[d].a, _algSettings.XBounds[d].b);
                            }

                            _colony[i].EvalFunction(ObjectiveFunction);

                            var m = 0;
                            while (m < _algSettings.NumSwimLoops) // Swim loop
                            {
                                var tempPosition = new double[_algSettings.Dim];
                                _colony[i].X.CopyTo(tempPosition, 0);

                                for (var d = 0; d < _algSettings.Dim; d++)
                                {
                                    _colony[i].X[d] = Math.Clamp(
                                        _colony[i].X[d] + (_algSettings.StepSize * tumbleVector[d]) / tumbleVNorm,
                                        _algSettings.XBounds[d].a, _algSettings.XBounds[d].b);
                                }

                                var nextFuncValue = ObjectiveFunction(_colony[i].X);

                                if (nextFuncValue > _colony[i].FuncValue + 1e-15)
                                {
                                    _colony[i].X = tempPosition;
                                    break;
                                }

                                _colony[i].FuncValue = nextFuncValue;
                                m++;
                            }

                            if (_colony[i].FuncValue < BestSolution.FuncValue) BestSolution = _colony[i].DeepCopy();

                            _colony[i].Health += _colony[i].FuncValue;
                        }

                        yield return _colony.DeepCopy().ToList();
                    }

                    Array.Sort(_colony);
                    for (var left = 0; left < _algSettings.ColonySize / 2; left++)
                        _colony[left + _algSettings.ColonySize / 2] = _colony[left].DeepCopy();
                }

                for (var i = 0; i < _algSettings.ColonySize; i++)
                {
                    if (!(_random.NextDouble() < _algSettings.ProbElimDisp)) continue;

                    _colony[i] = GenerateNewBacterium();
                    if (_colony[i].FuncValue < BestSolution.FuncValue) BestSolution = _colony[i].DeepCopy();
                }
            }
        }
    }
}