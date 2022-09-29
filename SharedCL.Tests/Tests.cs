using NUnit.Framework;

namespace SharedCL.Tests
{
    public class Tests
    {
        private const double Tol = 1e-15;
        
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestParser()
        {
            var func = FunctionParser.ParseFunction2D(@"4*(x1 - 5)^2 + (x2 - 6)^2");
            Assert.That(func(1, 2), Is.EqualTo(80).Within(Tol));
        }
    }
}