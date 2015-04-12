using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KLibrary.Labs.ObservableModel;

namespace PlanetClock
{
    public class MainViewModel
    {
        const double π = Math.PI;
        const double HourRadius = 100;

        public AppModel AppModel { get; private set; }

        public IGetOnlyProperty<int> Hour { get; private set; }
        public IGetOnlyProperty<int> Minute { get; private set; }

        public IGetOnlyProperty<double> HourInDouble { get; private set; }
        public IGetOnlyProperty<double> SecondInDouble { get; private set; }

        public IGetOnlyProperty<Vector> HourTranslate { get; private set; }

        public MainViewModel()
        {
            AppModel = new AppModel();

            Hour = AppModel.JustTicks.SelectToGetOnly(dt => dt.Hour);
            Minute = AppModel.JustTicks.SelectToGetOnly(dt => dt.Minute);

            HourInDouble = AppModel.JustTicks.SelectToGetOnly(ToHourInDouble);
            SecondInDouble = AppModel.JustTicks.SelectToGetOnly(ToSecondInDouble);

            HourTranslate = HourInDouble.SelectToGetOnly(HourToTranslate);
        }

        static readonly Func<DateTime, double> ToHourInDouble = dt => dt.Hour + dt.Minute / 60.0;
        static readonly Func<DateTime, double> ToSecondInDouble = dt => dt.Second + dt.Millisecond / 1000.0;

        static Vector HourToTranslate(double hour)
        {
            var hourAngle = hour * (2 * π / 12);
            return HourRadius * new Vector(Math.Sin(hourAngle), -Math.Cos(hourAngle));
        }

        public static readonly Func<double, double> SecondToAngle = s => s * (360 / 60);
    }
}
