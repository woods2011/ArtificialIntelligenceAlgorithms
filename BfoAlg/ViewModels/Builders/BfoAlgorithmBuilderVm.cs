using System;
using System.ComponentModel;
using BfoAlg.Models.BfoAlg;
using Newtonsoft.Json;
using SharedWPF.ViewModels;

namespace BfoAlg.ViewModels.Builders
{
    public class BfoAlgorithmBuilderVm : INotifyPropertyChanged
    {
        private readonly Random _random;

        [JsonIgnore]
        public MessageViewModel? MessageViewModel { get; init; } = new();

        public BfoAlgSettingsBuilderVm BfoAlgSettingsBuilderVm { get; } = new();


        public BfoAlgorithmBuilderVm(Random? random = null)
        {
            _random = random ?? new Random();
        }

        public BfoAlgorithm Build() => new(BfoAlgSettingsBuilderVm.Build(), _random);


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}