using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KLibrary.Labs.Reactive.Models;

namespace PlanetClock
{
    public class MainViewModel
    {
        const double π = Math.PI;
        const double HourRadius = 100;

        public AppModel AppModel { get; private set; }
        public IObservableGetProperty<Vector> HourTranslate { get; private set; }

        public MainViewModel()
        {
            AppModel = new AppModel();
            HourTranslate = AppModel.HourInDouble.ToGetProperty(HourToTranslate);
        }

        static Vector HourToTranslate(double hour)
        {
            var hourAngle = hour * (2 * π / 12);
            return HourRadius * new Vector(Math.Sin(hourAngle), -Math.Cos(hourAngle));
        }

        public static readonly Func<double, double> SecondToAngle = s => s * (360 / 60);
    }
}
