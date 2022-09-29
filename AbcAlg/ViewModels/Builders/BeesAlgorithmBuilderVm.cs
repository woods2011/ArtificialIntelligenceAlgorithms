using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AbcAlg.Models.BeesAlg;
using AbcAlg.ViewModels.Builders.NeighborhoodSearching;
using AbcAlg.ViewModels.Builders.Selection;
using Newtonsoft.Json;
using SharedWPF.ViewModels;

namespace AbcAlg.ViewModels.Builders
{
    public class BeesAlgorithmBuilderVm : INotifyPropertyChanged
    {
        private readonly Random _random;

        [JsonIgnore]
        public MessageViewModel? MessageViewModel { get; init; } = new();

        public BeesAlgSettingsBuilderVm BeesAlgSettingsBuilderVm { get; } = new();

        [JsonIgnore]
        public List<ISelectionBuilderVm> WorkersSelectionsList { get; }

        [JsonProperty("Workers Selection Strategy")]
        public ISelectionBuilderVm SelectedWorkersSelection { get; set; }

        [JsonIgnore]
        public List<ISelectionBuilderVm> ScoutsSelectionsList { get; }

        [JsonProperty("Scouts Selection Strategy")]
        public ISelectionBuilderVm SelectedScoutsSelection { get; set; }

        [JsonIgnore]
        public bool IsScoutSelectionEnable { get; set; } = false;

        [JsonIgnore]
        public List<INeighborhoodSearchingBuilderVm> NeighborhoodSearchingList { get; }

        [JsonProperty("Neighborhood Searching Strategy")]
        public INeighborhoodSearchingBuilderVm SelectedNeighborhoodSearching { get; set; }


        public BeesAlgorithmBuilderVm(Random? random = null)
        {
            _random = random ?? new Random();

            NeighborhoodSearchingList = new List<INeighborhoodSearchingBuilderVm>
            {
                new RndNborNsBuilderVm(_random),
                new RndNborAndBestNborNsBuilderVm(_random),
                new UniformNsBuilderVm(_random),
                new GaussianNsBuilderVm(_random)
            };
            SelectedNeighborhoodSearching = NeighborhoodSearchingList.First();

            WorkersSelectionsList = new List<ISelectionBuilderVm>
            {
                new TournamentSelectionBuilderVm(_random),
                new RouletteSelectionBuilderVm(_random),
                new SusSelectionBuilderVm(_random),
                new SimAnnealingByRndElExtSelectionBuilderVm(_random),
                new SimAnnealingByBestExtSelectionBuilderVm(_random),
                new SimAnnealingByRndElSelectionBuilderVm(_random),
                new SimAnnealingByBestSelectionBuilderVm(_random),
                new SimAnnealingByRndPairsSelectionBuilderVm(_random)
            };
            SelectedWorkersSelection = WorkersSelectionsList.First();

            ScoutsSelectionsList = new List<ISelectionBuilderVm>
            {
                new TournamentSelectionBuilderVm(_random),
                new RouletteSelectionBuilderVm(_random),
                new SusSelectionBuilderVm(_random),
                new SimAnnealingByRndElSelectionBuilderVm(_random),
                new SimAnnealingByBestSelectionBuilderVm(_random),
                new SimAnnealingByRndElExtSelectionBuilderVm(_random),
                new SimAnnealingByBestExtSelectionBuilderVm(_random),
                new SimAnnealingByRndPairsSelectionBuilderVm(_random)
            };
            SelectedScoutsSelection = ScoutsSelectionsList.First();
        }

        public BeesAlgorithm Build()
        {
            var beesAlgSettings = BeesAlgSettingsBuilderVm.Build();
            return new BeesAlgorithm(
                beesAlgSettings,
                SelectedNeighborhoodSearching.Build(beesAlgSettings),
                SelectedWorkersSelection.Build(beesAlgSettings),
                IsScoutSelectionEnable ? SelectedScoutsSelection.Build(beesAlgSettings) : null,
                _random);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}