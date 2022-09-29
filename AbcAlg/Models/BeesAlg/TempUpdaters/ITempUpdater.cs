using System;

namespace AbcAlg.Models.BeesAlg.TempUpdaters
{
    public interface ITempUpdater
    {
        public double CurTemp { get; }
        public void UpdateTemperature();
    }

    public abstract class TempUpdater : ITempUpdater
    {
        protected readonly double InitTemp;
        public double CurTemp { get; protected set; }

        protected TempUpdater(double initTemp)
        {
            InitTemp = initTemp;
            CurTemp = initTemp;
        }

        public abstract void UpdateTemperature();
    }
    
    public class SimpleTempUpdater : TempUpdater
    {
        private readonly double _iterationsCount;
        private readonly double _alpha;
        private int _curIter = 0;

        public SimpleTempUpdater(double initTemp, double iterationsCount, double alpha = 1.0) : base(initTemp)
        {
            _iterationsCount = iterationsCount;
            _alpha = alpha;
        }

        public override void UpdateTemperature()
        {
            CurTemp = InitTemp * Math.Pow(1.0 - _curIter / _iterationsCount, _alpha);
            _curIter++;
        }
    }
    
    //ToDo : add several tempUpdaters
}