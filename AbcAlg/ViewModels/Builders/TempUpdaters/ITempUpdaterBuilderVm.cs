using System.ComponentModel;
using AbcAlg.Models.BeesAlg;
using AbcAlg.Models.BeesAlg.TempUpdaters;

namespace AbcAlg.ViewModels.Builders.TempUpdaters
{
    public interface ITempUpdaterBuilderVm : INotifyPropertyChanged
    {
        ITempUpdater Build(BeesAlgSettings settings);
    }

    public abstract class TempUpdaterBuilderVm : ITempUpdaterBuilderVm
    {
        public double InitTemp { get; set; } = 100.0;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public abstract ITempUpdater Build(BeesAlgSettings settings);
    }
    
    public class SimpleTempUpdaterBuilderVm : TempUpdaterBuilderVm
    {
        public double Alpha { get; set; } = 1.0;
            
        public override ITempUpdater Build(BeesAlgSettings settings) =>
            new SimpleTempUpdater(InitTemp, settings.MaxIterationsCount, Alpha);
    }
}