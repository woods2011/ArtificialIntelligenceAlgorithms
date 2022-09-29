using System;
using System.ComponentModel;
using AbcAlg.Models.BeesAlg;
using AbcAlg.Models.BeesAlg.NeighborhoodSearching;

namespace AbcAlg.ViewModels.Builders.NeighborhoodSearching
{
    public interface INeighborhoodSearchingBuilderVm : INotifyPropertyChanged
    {
        INeighborhoodSearchingAlg Build(BeesAlgSettings settings);
    }

    public abstract class NeighborhoodSearchingBuilderVm : INeighborhoodSearchingBuilderVm
    {
        protected Random Random { get; }

        protected NeighborhoodSearchingBuilderVm(Random? random = null)
        {
            Random = random ?? new Random();
        }

        public abstract INeighborhoodSearchingAlg Build(BeesAlgSettings settings);

        public event PropertyChangedEventHandler? PropertyChanged;
    }


    public class GaussianNsBuilderVm : NeighborhoodSearchingBuilderVm
    {
        public double StdDevPercent { get; set; } = 0.1;

        public GaussianNsBuilderVm(Random? random = null) : base(random)
        {
        }


        public override GaussianNs Build(BeesAlgSettings settings)
            => new(settings.X1Bounds, settings.X2Bounds, settings.ObjectiveFunction, Random)
                { StdDevPercent = StdDevPercent };
    }

    public class UniformNsBuilderVm : NeighborhoodSearchingBuilderVm
    {
        public UniformNsBuilderVm(Random? random = null) : base(random)
        {
        }

        public override UniformNs Build(BeesAlgSettings settings) =>
            new(settings.X1Bounds, settings.X2Bounds, settings.ObjectiveFunction, Random);
    }

    public class RndNborNsBuilderVm : NeighborhoodSearchingBuilderVm
    {
        public RndNborNsBuilderVm(Random? random = null) : base(random)
        {
        }

        public override RndNborNs Build(BeesAlgSettings settings) =>
            new(settings.X1Bounds, settings.X2Bounds, settings.ObjectiveFunction, Random);
    }

    public class RndNborAndBestNborNsBuilderVm : NeighborhoodSearchingBuilderVm
    {
        public double CFactor { get; set; } = 0.5;

        public RndNborAndBestNborNsBuilderVm(Random? random = null) : base(random)
        {
        }

        public override RndNborAndBestNborNs Build(BeesAlgSettings settings) =>
            new(settings.X1Bounds, settings.X2Bounds, settings.ObjectiveFunction, CFactor, Random);
    }
}