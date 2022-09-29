using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BfoAlg.Models.BfoAlg
{
    public class Bacterium : IComparable<Bacterium>
    {
        public double[] X { get; set; }
        public int Dim { get; }

        public double FuncValue { get; set; }
        public double Health { get; set; } = 0.0;


        public Bacterium(int dim) : this(new double[dim])
        {
        }

        public Bacterium(double[] x) => (X, Dim) = (x.ToArray(), x.Length);

        public Bacterium(double[] x, double funcValue) : this(x) => FuncValue = funcValue;

        public Bacterium(double[] x, Func<double[], double> function) : this(x, function(x))
        {
        }

        
        public void EvalFunction(Func<double[], double> function) => FuncValue = function(X);


        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"F(x1");

            for (var dim = 1; dim < X.Length; dim++)
            {
                stringBuilder.Append($",x{dim + 1}");
            }

            stringBuilder.Append($") = {FuncValue} in ");

            for (var dim = 0; dim < X.Length; dim++)
            {
                stringBuilder.Append($"X{dim + 1} = {X[dim]}; ");
            }

            return stringBuilder.ToString();
        }

        public Bacterium DeepCopy() => new(X.ToArray(), FuncValue);

        public Bacterium ShallowCopy() => (Bacterium) MemberwiseClone();
        

        public int CompareTo(Bacterium? other)
        {
            if (other is null) return -1;

            if (Health < other.Health) return -1;
            return Health > other.Health ? 1 : 0;
        }
    }


    public static class BacteriumExtensions
    {
        public static void EvalFunction(this ICollection<Bacterium> colony, Func<double[], double> function)
        {
            foreach (var bacterium in colony) bacterium.FuncValue = function(bacterium.X);
        }

        public static void ResetHealth(this ICollection<Bacterium> colony)
        {
            foreach (var bacterium in colony) bacterium.Health = 0.0;
        }

        public static IEnumerable<Bacterium> ShallowCopy(this IEnumerable<Bacterium> colony) =>
            colony.Select(bacterium => bacterium.ShallowCopy());
        
        public static IEnumerable<Bacterium> DeepCopy(this IEnumerable<Bacterium> colony) =>
            colony.Select(bacterium => bacterium.DeepCopy());
    }
}