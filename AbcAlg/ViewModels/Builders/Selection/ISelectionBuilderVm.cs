using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AbcAlg.Models.BeesAlg;
using AbcAlg.Models.BeesAlg.Selection;
using AbcAlg.ViewModels.Builders.TempUpdaters;

namespace AbcAlg.ViewModels.Builders.Selection
{
    public interface ISelectionBuilderVm : INotifyPropertyChanged
    {
        ISelectionAlg Build(BeesAlgSettings settings);

        public string SelectionAlgName { get; }
    }

    public abstract class SelectionBuilderVm : ISelectionBuilderVm
    {
        protected Random Random { get; }

        protected SelectionBuilderVm(Random? random = null)
        {
            Random = random ?? new Random();
        }

        public abstract ISelectionAlg Build(BeesAlgSettings settings);

        public abstract string SelectionAlgName { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }


    public class RouletteSelectionBuilderVm : SelectionBuilderVm
    {
        public RouletteSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override RouletteSelection Build(BeesAlgSettings _) => new(Random);

        public override string SelectionAlgName => "Пропорциональный отбор";
    }

    public class SusSelectionBuilderVm : SelectionBuilderVm
    {
        public SusSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override SusSelection Build(BeesAlgSettings _) => new(Random);

        public override string SelectionAlgName => "Стохастический универсальный отбор";
    }

    public class TournamentSelectionBuilderVm : SelectionBuilderVm
    {
        public int TournamentSize { get; set; } = 3;

        public TournamentSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override TournamentSelection Build(BeesAlgSettings _) => new(Random) { TournamentSize = TournamentSize };

        public override string SelectionAlgName => "Турнирный отбор";
    }


    public abstract class SimAnnealingSelectionBuilderVm : SelectionBuilderVm
    {
        public List<ITempUpdaterBuilderVm> TempUpdaters { get; }

        public ITempUpdaterBuilderVm SelectedTempUpdater { get; set; }

        protected SimAnnealingSelectionBuilderVm(Random? random = null) : base(random)
        {
            TempUpdaters = new List<ITempUpdaterBuilderVm> { new SimpleTempUpdaterBuilderVm() };
            SelectedTempUpdater = TempUpdaters.First();
        }
    }

    public class SimAnnealingByBestSelectionBuilderVm : SimAnnealingSelectionBuilderVm
    {
        public SimAnnealingByBestSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override SimAnnealingByBestSelection Build(BeesAlgSettings settings) =>
            new(SelectedTempUpdater.Build(settings), Random);

        public override string SelectionAlgName => "Имитация отжига по лучшему";
    }

    public class SimAnnealingByBestExtSelectionBuilderVm : SimAnnealingSelectionBuilderVm
    {
        public SimAnnealingByBestExtSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override SimAnnealingByBestExtendedSelection Build(BeesAlgSettings settings) =>
            new(SelectedTempUpdater.Build(settings), Random);

        public override string SelectionAlgName => "Имитация отжига по лучшему (расширенная)";
    }

    public class SimAnnealingByRndElSelectionBuilderVm : SimAnnealingSelectionBuilderVm
    {
        public SimAnnealingByRndElSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override SimAnnealingByRndElSelection Build(BeesAlgSettings settings) =>
            new(SelectedTempUpdater.Build(settings), Random);

        public override string SelectionAlgName => "Имитация отжига по случайному";
    }

    public class SimAnnealingByRndElExtSelectionBuilderVm : SimAnnealingSelectionBuilderVm
    {
        public SimAnnealingByRndElExtSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override SimAnnealingByRndElExtSelection Build(BeesAlgSettings settings) =>
            new(SelectedTempUpdater.Build(settings), Random);

        public override string SelectionAlgName => "Имитация отжига по случайному (расширенная)";
    }

    public class SimAnnealingByRndPairsSelectionBuilderVm : SimAnnealingSelectionBuilderVm
    {
        public SimAnnealingByRndPairsSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override SimAnnealingByRndPairsSelection Build(BeesAlgSettings settings) =>
            new(SelectedTempUpdater.Build(settings), Random);

        public override string SelectionAlgName => "Имитация отжига по всем случайным парам";
    }
}