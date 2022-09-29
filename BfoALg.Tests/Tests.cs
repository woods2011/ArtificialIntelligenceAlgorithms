using System;
using System.Linq;
using BfoAlg.Models.BfoAlg;
using BfoAlg.ViewModels.Builders;
using NUnit.Framework;
using SharedCL;

namespace BfoALg.Tests
{
    public class Tests
    {
        private const double Tol = 1e-15;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var random = new Random(17); //17

            const int dim = 2;

            var func = FunctionParser.ParseFunction("4*(x1 - 5)^2 + (x2 - 6)^2", dim);
            //var func = (double[] x) => 4 * Math.Pow(x[0] - 5, 2) + Math.Pow(x[1] - 6, 2);

            var xBounds = Enumerable.Repeat((-10.0, 10.0), dim).ToArray();
            var algSettings = new BfoAlgSettings(xBounds)
            {
                ColonySize = 50,
                NumChemotacticLoops = 20,
                NumSwimLoops = 5,
                NumReproduceElimLoops = 8,
                NumElimDispLoops = 4,
                ProbElimDisp = 0.25,
                StepSize = 0.1,
                ObjectiveFunction = func
            };


            var algorithm = new BfoAlgorithm(algSettings, random);
            _ = algorithm.FindMinFunctionAndSaveSteps().LastOrDefault();
            var actualResult = algorithm.BestSolution;
            Console.WriteLine(actualResult);


            var expectedResult = new Bacterium(
                new[]
                {
                    5.0004667177147075,
                    6.000497038650948
                },
                func
            );


            Assert.That(actualResult.FuncValue, Is.EqualTo(expectedResult.FuncValue).Within(Tol));
            for (var d = 0; d < dim; d++) Assert.That(actualResult.X[d], Is.EqualTo(expectedResult.X[d]).Within(Tol));
        }
    }
}