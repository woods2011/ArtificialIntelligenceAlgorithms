using System.ComponentModel;

namespace SharedWPF.ViewModels
{
    public class BoundsVm : INotifyPropertyChanged
    {
        public double A { get; set; } = -500;
        public double B { get; set; } = 500;

        public (double a, double b) ToValTuple() => (A, B);

        public void Deconstruct(out double a, out double b) => (a, b) = (A, B);

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}