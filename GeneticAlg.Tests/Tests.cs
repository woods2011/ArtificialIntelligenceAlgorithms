using System;
using System.Linq;
using GeneticAlg.Models.GeneticAlg;
using GeneticAlg.Models.GeneticAlg.Crossover;
using GeneticAlg.Models.GeneticAlg.Mutation;
using GeneticAlg.Models.GeneticAlg.Selection;
using GeneticAlg.ViewModels.Builders;
using GeneticAlg.ViewModels.Builders.Crossover;
using GeneticAlg.ViewModels.Builders.Mutation;
using GeneticAlg.ViewModels.Builders.Selection;
using NUnit.Framework;
using SharedCL;

namespace GeneticAlg.Tests
{
    public class Tests
    {
        private const double Tol = 1e-15;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGeneticAlgModel()
        {
            var random = new Random(17);

            var func = FunctionParser.ParseFunction2D("4*(x1-5)^2 + (x2 - 6)^2");

            var algSettings = new GeneticAlgSettings()
            {
                CountGenerations = 100,
                PopulationSize = 20,
                X1Bounds = (-10, 10),
                X2Bounds = (-10, 10),
            };

            var selection = new TournamentSelection(random)
            {
                TournamentSize = 3
            };
            var crossover = new AlphaCrossover(
                (algSettings.X1Bounds.a, algSettings.X1Bounds.b),
                (algSettings.X2Bounds.a, algSettings.X2Bounds.b),
                random)
            {
                Alpha = 0.5,
                CrossingProbability = 0.98
            };
            var mutation = new UniformMutation(
                (algSettings.X1Bounds.a, algSettings.X1Bounds.b),
                (algSettings.X2Bounds.a, algSettings.X2Bounds.b),
                random)
            {
                MutationProbability = 0.1
            };
            

            var algorithm = new GeneticAlgorithm(func, algSettings, selection, crossover, mutation, random);
            _ = algorithm.FindMinFunctionAndSaveSteps().LastOrDefault();
            var actualResult = algorithm.BestSolution;
            Console.WriteLine(actualResult);

            
            var expectedResult = new Chromosome(
                x1: 5.000009643975474,
                x2: 5.999880870475016,
                func
            );
            

            Assert.That(actualResult.FuncValue, Is.EqualTo(expectedResult.FuncValue).Within(Tol));
            Assert.That(actualResult.X1, Is.EqualTo(expectedResult.X1).Within(Tol));
            Assert.That(actualResult.X2, Is.EqualTo(expectedResult.X2).Within(Tol));
        }

        [Test]
        public void TestGeneticAlgModelViewModel()
        {
            var random = new Random(17);

            var genAlgBuilderVm = new GeneticAlgorithmBuilderVm(random)
            {
                CountGenerations = 100, PopulationSize = 20
            };

            var selectedFunction = genAlgBuilderVm.ObjectiveFunctions.First(s => s.Equals("4*(x1 - 5)^2 + (x2 - 6)^2"));
            genAlgBuilderVm.SelectedFunction = selectedFunction;

            (genAlgBuilderVm.X1Bounds.A, genAlgBuilderVm.X1Bounds.B) = (-10, 10);
            (genAlgBuilderVm.X2Bounds.A, genAlgBuilderVm.X2Bounds.B) = (-10, 10);

            var tournamentSelectionBuilderVm =
                genAlgBuilderVm.Selections.OfType<TournamentSelectionBuilderVm>().First();
            tournamentSelectionBuilderVm.TournamentSize = 3;
            genAlgBuilderVm.SelectedSelection = tournamentSelectionBuilderVm;

            var alphaCrossoverBuilderVm =
                genAlgBuilderVm.Crossovers.OfType<AlphaCrossoverBuilderVm>().First();
            alphaCrossoverBuilderVm.Alpha = 0.5;
            alphaCrossoverBuilderVm.CrossingProbability = 0.98;
            genAlgBuilderVm.SelectedCrossover = alphaCrossoverBuilderVm;

            var uniformMutationBuilderVm =
                genAlgBuilderVm.Mutations.OfType<UniformMutationBuilderVm>().First();
            uniformMutationBuilderVm.MutationProbability = 0.1;
            genAlgBuilderVm.SelectedMutation = uniformMutationBuilderVm;


            var algorithm = genAlgBuilderVm.Build();
            _ = algorithm.FindMinFunctionAndSaveSteps().LastOrDefault();
            var actualResult = algorithm.BestSolution;
            Console.WriteLine(actualResult);


            var expectedResult = new Chromosome(
                x1: 5.000009643975474,
                x2: 5.999880870475016,
                FunctionParser.ParseFunction2D(selectedFunction)
            );

            
            Assert.That(actualResult.X1, Is.EqualTo(expectedResult.X1).Within(Tol));
            Assert.That(actualResult.X2, Is.EqualTo(expectedResult.X2).Within(Tol));
            Assert.That(actualResult.FuncValue, Is.EqualTo(expectedResult.FuncValue).Within(Tol));
        }
    }
}