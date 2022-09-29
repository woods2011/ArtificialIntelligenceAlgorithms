using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GeneticAlg.Models;
using GeneticAlg.Models.GeneticAlg;
using GeneticAlg.ViewModels.Builders;
using Jace;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using SharedCL;
using SharedCL.Extension;
using SharedWPF.Commands;
using SharedWPF.ViewModels;

namespace GeneticAlg.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DialogService _dialogService = new();

        private CancellationTokenSource _tokenSource = new();
        private PlotModel _plotPlotModel = new();

        private GeneticAlgorithmBuilderVm _savedAlgBuilderVm;
        private List<List<Chromosome>>? _currentResultWithFullProgress;


        public Chromosome? LastOptimizationResult { get; private set; }

        public GeneticAlgorithmBuilderVm AlgBuilderVm { get; }


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
        public int GenerationsPerTick { get; set; } = 10;


        public string LastOptimizationLog { get; private set; } = String.Empty;
        public int MaxLogSize { get; set; } = 500;


        public MessageViewModel MessageViewModel { get; } = new();


        public MainViewModel()
        {
            AlgBuilderVm = new GeneticAlgorithmBuilderVm(new Random(17)) {MessageViewModel = MessageViewModel};
            _savedAlgBuilderVm = AlgBuilderVm;

            PlotModel = new PlotModel {Title = "Динамика работы алгоритма"};
            _tokenSource.Cancel();
        }


        public ICommand FindMinCommand =>
            new ActionCommand(_ => FindMin(), _ => !String.IsNullOrEmpty(AlgBuilderVm.SelectedFunction));

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
                DrawPlot(algorithm.ObjectiveFunction);
            }
            catch (Exception e) when (e is VariableNotDefinedException or ParseException or InvalidOperationException)
            {
                MessageViewModel.Message = e.Message;
            }
        }

        private void DrawLog()
        {
            if (_currentResultWithFullProgress is null) return;

            var (stringBuilder, curGeneration, divider) = (
                new StringBuilder(),
                0,
                Math.Max(1, Math.Ceiling(_currentResultWithFullProgress.Count / (double) MaxLogSize))
            );

            foreach (var generationList in _currentResultWithFullProgress)
            {
                if (curGeneration % divider == 0 || curGeneration == _currentResultWithFullProgress.Count - 1)
                    stringBuilder.Append(
                        $"Generation: {curGeneration}; Best Result - {generationList.MinBy(chromosome => chromosome.FuncValue)}{Environment.NewLine}");

                curGeneration++;
            }

            LastOptimizationLog = stringBuilder.ToString();
        }

        private void DrawPlot(Func<double, double, double> func)
        {
            if (_currentResultWithFullProgress is null) return;

            PlotModel = new PlotModel {Title = "Динамика работы алгоритма"};
            PlotModel.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.Right, Title = "Поколение",
                Palette = OxyPalettes.Rainbow(_currentResultWithFullProgress.Count)
            });

            var scatterSeries = new ScatterSeries {MarkerType = MarkerType.Cross};
            scatterSeries.Points.AddRange(
                _currentResultWithFullProgress.SelectMany((list, i) =>
                    list.Select(chromosome => new ScatterPoint(chromosome.X1, chromosome.X2, 4, i))).ToList());

            var xx = ArrayBuilder.CreateVector(AlgBuilderVm.X1Bounds.A, AlgBuilderVm.X1Bounds.B, 100);
            var yy = ArrayBuilder.CreateVector(AlgBuilderVm.X2Bounds.A, AlgBuilderVm.X2Bounds.B, 100);
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
                    point.Size = 4;
                    if (acc % (_savedAlgBuilderVm.PopulationSize * GenerationsPerTick) == 0)
                    {
                        if (_tokenSource.Token.IsCancellationRequested) return;
                        var taskDelay = Task.Delay(TimeSpan.FromSeconds(1.0 / DrawSpeed), _tokenSource.Token);
                        PlotModel.InvalidatePlot(true);
                        await taskDelay;
                        await Task.Delay(50);
                    }

                    acc++;
                }

                scatterSeries.Points.Last().Size = 5;
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
                    (curGenerationChromosomes, curGeneration) =>
                        new
                        {
                            Generation = curGeneration,
                            BestResult = curGenerationChromosomes.MinBy(chr => chr.FuncValue),
                            AllChromosomes = curGenerationChromosomes
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