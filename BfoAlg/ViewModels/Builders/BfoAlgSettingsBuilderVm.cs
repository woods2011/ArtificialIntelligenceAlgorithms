using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BfoAlg.Models.BfoAlg;
using Newtonsoft.Json;
using SharedCL;
using SharedWPF.ViewModels;

namespace BfoAlg.ViewModels.Builders
{
    public class BfoAlgSettingsBuilderVm : INotifyPropertyChanged
    {
        private int _colonySize = 26;
        public BoundsVm X1Bounds { get; } = new();
        public BoundsVm X2Bounds { get; } = new();

        public int ColonySize
        {
            get => _colonySize;
            set => _colonySize = value / 2 * 2;
        }

        public int NumChemotacticLoops { get; set; } = 15;
        public int NumSwimLoops { get; set; } = 10;
        public int NumReproduceElimLoops { get; set; } = 10;
        public int NumElimDispLoops { get; set; } = 6;
        public double ProbElimDisp { get; set; } = 0.25;
        public double StepSize { get; set; } = 0.075;


        [JsonProperty("Objective Function")]
        public string SelectedFunction { get; set; }

        [JsonIgnore]
        public List<string> ObjectiveFunctions { get; }

        public BfoAlgSettingsBuilderVm()
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

        public BfoAlgSettings Build()
        {
            var x1Bounds = X1Bounds.A + 1e-15 < X1Bounds.B
                ? X1Bounds.ToValTuple()
                : throw new InvalidOperationException($"{nameof(X1Bounds)}: B <= A");
            var x2Bounds = X2Bounds.A + 1e-15 < X2Bounds.B
                ? X2Bounds.ToValTuple()
                : throw new InvalidOperationException($"{nameof(X2Bounds)}: B <= A");

            return new BfoAlgSettings(new[] { x1Bounds, x2Bounds })
            {
                ColonySize = ColonySize,
                NumChemotacticLoops = NumChemotacticLoops,
                NumSwimLoops = NumSwimLoops,
                NumReproduceElimLoops = NumReproduceElimLoops,
                NumElimDispLoops = NumElimDispLoops,
                ProbElimDisp = ProbElimDisp,
                StepSize = StepSize,
                ObjectiveFunction = FunctionParser.ParseFunction(SelectedFunction, 2)
            };
        }


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}