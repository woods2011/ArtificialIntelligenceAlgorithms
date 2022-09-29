using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BfoAlg.Models.BfoAlg;
using BfoAlg.ViewModels.Builders;
using Jace;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SharedCL;
using SharedCL.Extension;
using SharedWPF.Commands;
using SharedWPF.ViewModels;

namespace BfoAlg.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DialogService _dialogService = new();

        private CancellationTokenSource _tokenSource = new();
        private PlotModel _plotPlotModel = new();

        private BfoAlgorithmBuilderVm _savedAlgBuilderVm;
        private List<List<Bacterium>>? _currentResultWithFullProgress;
        public Bacterium? LastOptimizationResult { get; private set; }

        public BfoAlgorithmBuilderVm AlgBuilderVm { get; }


        public PlotModel PlotModel
        {
            get => _plotPlotModel;
            private set
            {
                _plotPlotModel = value;
                _plotPlotModel.Axes.Add(new LinearAxis {Position = AxisPosition.Bottom, Title = "X1"});
                _plotPlotModel.Axes.Add(new LinearAxis {Position = AxisPosition.Left, Title = "X2"});
            }
        }

        public int DrawSpeed { get; set; } = 10;
        public int IterationsPerTick { get; set; } = 10;


        public string LastOptimizationLog { get; set; } = String.Empty;
        public int MaxLogSize { get; set; } = 500;


        public MessageViewModel MessageViewModel { get; } = new();


        public MainViewModel()
        {
            AlgBuilderVm = new BfoAlgorithmBuilderVm() {MessageViewModel = MessageViewModel};
            _savedAlgBuilderVm = AlgBuilderVm;
            PlotModel = new PlotModel {Title = "F(x1,x2)"};
            _tokenSource.Cancel();
        }


        public ICommand FindMinCommand => new ActionCommand(_ => FindMin(),
            _ => !String.IsNullOrEmpty(AlgBuilderVm.BfoAlgSettingsBuilderVm.SelectedFunction));

        public ICommand AnimatePlotAsyncCommand => new ActionCommand
            (_ => AnimatePlot(), _ => _currentResultWithFullProgress is not null); // Todo: add AsyncCommand

        public ICommand StopAnimatePlotAsyncCommand =>
            new ActionCommand(_ => _tokenSource.Cancel(), _ => !_tokenSource.IsCancellationRequested);

        public ICommand SaveLastResultCommand =>
            new ActionCommand(_ => SaveLastResult(), _ => LastOptimizationResult is not null);

        public ICommand SaveLastResultWithLogCommand =>
            new ActionCommand(_ => SaveLastResultWithLog(), _ => _currentResultWithFullProgress is not null);


        private void FindMin()
        {
            _tokenSource.Cancel();
            _savedAlgBuilderVm = AlgBuilderVm.DeepClone();
            LastOptimizationResult = null;
            _currentResultWithFullProgress = null;

            try
            {
                var algorithm = AlgBuilderVm.Build();
                _currentResultWithFullProgress = algorithm.FindMinFunctionAndSaveSteps().ToList();
                LastOptimizationResult = algorithm.BestSolution;

                DrawLog();
                DrawPlot((x1, x2) => algorithm.ObjectiveFunction(new[] {x1, x2}));
            }
            catch (Exception e) when (e is VariableNotDefinedException or ParseException or InvalidOperationException)
            {
                MessageViewModel.Message = e.Message;
            }
        }


        private void DrawLog()
        {
            if (_currentResultWithFullProgress is null) return;

            var (stringBuilder, curIteration, divider) = (
                new StringBuilder(),
                0,
                Math.Max(1, Math.Ceiling(_currentResultWithFullProgress.Count / (double) MaxLogSize))
            );

            foreach (var iterationsList in _currentResultWithFullProgress)
            {
                if (curIteration % divider == 0 || curIteration == _currentResultWithFullProgress.Count - 1)
                    stringBuilder.Append(
                        $"Iteration: {curIteration}; Best Result - {iterationsList.MinBy(bacterium => bacterium.FuncValue)}{Environment.NewLine}"
                    );

                curIteration++;
            }

            LastOptimizationLog = stringBuilder.ToString();
        }

        private void DrawPlot(Func<double, double, double> func)
        {
            if (_currentResultWithFullProgress is null) return;

            PlotModel = new PlotModel {Title = "F(x1,x2)"};
            PlotModel.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.Right, Title = "Iteration",
                Palette = OxyPalettes.Jet(_currentResultWithFullProgress.Count)
            });

            var scatterSeries = new ScatterSeries {MarkerType = MarkerType.Circle};
            scatterSeries.Points.AddRange(
                _currentResultWithFullProgress.SelectMany((list, i) =>
                    list.Select(bacterium => new ScatterPoint(bacterium.X[0], bacterium.X[1], 1.5, i + 20))).ToList());

            var xx = ArrayBuilder.CreateVector(
                AlgBuilderVm.BfoAlgSettingsBuilderVm.X1Bounds.A,
                AlgBuilderVm.BfoAlgSettingsBuilderVm.X1Bounds.B,
                100);
            var yy = ArrayBuilder.CreateVector(
                AlgBuilderVm.BfoAlgSettingsBuilderVm.X2Bounds.A,
                AlgBuilderVm.BfoAlgSettingsBuilderVm.X2Bounds.B,
                100);
            var peaksData = ArrayBuilder.Evaluate(func, xx, yy);

            var contourSeries = new ContourSeries
            {
                Color = OxyColors.Black, LabelBackground = OxyColors.White,
                ColumnCoordinates = xx, RowCoordinates = yy, Data = peaksData
            };

            PlotModel.Series.Add(contourSeries);
            PlotModel.Series.Add(scatterSeries);
            PlotModel.InvalidatePlot(true);
        }

        private async void AnimatePlot()
        {
            // Todo: add AsyncCommand
            try
            {
                _tokenSource.Cancel();
                _tokenSource = new CancellationTokenSource();

                var scatterSeries = PlotModel.Series.OfType<ScatterSeries>().First();
                scatterSeries.Points.ForEach(point => point.Size = 0);
                PlotModel.InvalidatePlot(true);
                await Task.Delay(TimeSpan.FromSeconds(0.2), _tokenSource.Token);

                var acc = 1;
                foreach (var point in scatterSeries.Points)
                {
                    point.Size = 1.5;
                    if (acc % (_savedAlgBuilderVm.BfoAlgSettingsBuilderVm.ColonySize * IterationsPerTick)
                        == 0)
                    {
                        if (_tokenSource.Token.IsCancellationRequested) return;
                        var taskDelay = Task.Delay(TimeSpan.FromSeconds(1.0 / DrawSpeed), _tokenSource.Token);
                        PlotModel.InvalidatePlot(true);
                        await taskDelay;
                        await Task.Delay(50);
                    }

                    acc++;
                }

                scatterSeries.Points.Last().Size = 3;
                PlotModel.InvalidatePlot(true);
            }
            catch (TaskCanceledException)
            {
            }
        }


        private void SaveLastResult()
        {
            var dialogSettings = new SaveFileDialogSettings()
            {
                Title = "Сохранение Результата Оптимизации",
                FileName = "AllResultsHistory",
                InitialDirectory = $@"{Directory.GetCurrentDirectory()}\OptimizationResults",
                Filter = "JSON file (*.json)|*.json|Text Documents (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            var success = _dialogService.ShowSaveFileDialog(this, dialogSettings);
            if (success is false) return;

            new AppendFileSimpleLogger(dialogSettings.FileName).Log(SerializeLastResult());
        }

        private void SaveLastResultWithLog()
        {
            var dialogSettings = new SaveFileDialogSettings()
            {
                Title = "Сохранение Лога Оптимизации",
                FileName = "LastResultLog",
                InitialDirectory = $@"{Directory.GetCurrentDirectory()}\OptimizationResults",
                Filter = "JSON file (*.json)|*.json|Text Documents (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            var success = _dialogService.ShowSaveFileDialog(this, dialogSettings);
            if (success is false) return;

            var serializedResultLog = JsonConvert.SerializeObject
            (
                _currentResultWithFullProgress?.Select(
                    (curIterationBacteriums, curIteration) =>
                        new
                        {
                            Iteration = curIteration,
                            BestResult = curIterationBacteriums.MinBy(chr => chr.FuncValue),
                            AllBacteriums = curIterationBacteriums
                        }
                ),
                Formatting.Indented, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto}
            );

            new TruncateFileSimpleLogger(dialogSettings.FileName).Log(
                $"{SerializeLastResult()}{Environment.NewLine}{Environment.NewLine}" +
                $"Лог Оптимизации:{Environment.NewLine}{serializedResultLog}");
        }

        private string SerializeLastResult() => JsonConvert.SerializeObject(
            new
            {
                SaveTime = DateTime.Now,
                AlgorithmInput = _savedAlgBuilderVm,
                OptimizationResult = LastOptimizationResult
            },
            Formatting.Indented, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto}
        );


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}