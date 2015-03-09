using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KLibrary.Labs.Reactive;
using KLibrary.Labs.Reactive.Models;

namespace PlanetClock
{
    public class MainViewModel
    {
        const double π = Math.PI;
        const double HourRadius = 100;

        public AppModel AppModel { get; private set; }

        public IObservableGetProperty<int> Hour { get; private set; }
        public IObservableGetProperty<int> Minute { get; private set; }

        public IObservableGetProperty<double> HourInDouble { get; private set; }
        public IObservableGetProperty<double> SecondInDouble { get; private set; }

        public IObservableGetProperty<Vector> HourTranslate { get; private set; }

        public MainViewModel()
        {
            AppModel = new AppModel();

            Hour = AppModel.JustTicks.ToGetProperty(dt => dt.Hour);
            Minute = AppModel.JustTicks.ToGetProperty(dt => dt.Minute);

            HourInDouble = AppModel.JustTicks
                .Filter(dt => dt.Second == 0 && dt.Millisecond == 0)
                .Map(ToHourInDouble)
                .ToGetProperty(ToHourInDouble(AppModel.JustTicks.Value));
            SecondInDouble = AppModel.JustTicks
                .ToGetProperty(ToSecondInDouble);

            HourTranslate = HourInDouble.ToGetProperty(HourToTranslate);
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
