using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AbcAlg.Models.BeesAlg;
using Newtonsoft.Json;
using SharedCL;
using SharedWPF.ViewModels;

namespace AbcAlg.ViewModels.Builders
{
    public class BeesAlgSettingsBuilderVm : INotifyPropertyChanged
    {
        private int _foodSourcesCount = 20;

        public BoundsVm X1Bounds { get; } = new();
        public BoundsVm X2Bounds { get; } = new();


        public int MaxIterationsCount { get; set; } = 50;

        public int FoodSourcesCount
        {
            get => _foodSourcesCount;
            set
            {
                _foodSourcesCount = value;
                MaxNumOfVisits = _foodSourcesCount * 2;
            }
        }

        public int MaxNumOfVisits { get; set; } = 40;

        [JsonProperty("Objective Function")]
        public string SelectedFunction { get; set; }

        [JsonIgnore]
        public List<string> ObjectiveFunctions { get; }

        public BeesAlgSettingsBuilderVm()
        {
            ObjectiveFunctions = new List<string>()
            {
                "x1^2 + 3*x2^2 + 2*x1*x2",
                "100*(x2 - x1^2)^2 + (1 - x1)^2",
                "-12*x2 + 4*x1^2 + 4*x2^2 - 4*x1*x2",
                "418,9829 * 2 - x1 * sin(sqrt(abs(x1))) - x2 * sin(sqrt(abs(x2)))",
                "(x1 - 2)^4 + (x1 - 2*x2)^2",
                "4*(x1 - 5)^2 + (x2 - 6)^2"
            };
            SelectedFunction = ObjectiveFunctions.FirstOrDefault(s => s.Equals("418,9829 * 2 - x1 * sin(sqrt(abs(x1))) - x2 * sin(sqrt(abs(x2)))")) ??
                               ObjectiveFunctions.FirstOrDefault () ?? String.Empty;
        }

        public BeesAlgSettings Build() => new()
        {
            ObjectiveFunction = FunctionParser.ParseFunction2D(SelectedFunction),
            X1Bounds = X1Bounds.A + 1e-15 < X1Bounds.B
                ? X1Bounds.ToValTuple()
                : throw new InvalidOperationException($"{nameof(X1Bounds)}: B <= A"),
            X2Bounds = X2Bounds.A + 1e-15 < X2Bounds.B
                ? X2Bounds.ToValTuple()
                : throw new InvalidOperationException($"{nameof(X2Bounds)}: B <= A"),
            FoodSourcesCount = FoodSourcesCount,
            MaxIterationsCount = MaxIterationsCount,
            MaxNumOfVisits = MaxNumOfVisits
        };


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}