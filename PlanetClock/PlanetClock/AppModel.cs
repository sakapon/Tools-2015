using System;
using System.Collections.Generic;
using System.Linq;
using KLibrary.Labs.Reactive;
using KLibrary.Labs.Reactive.Models;

namespace PlanetClock
{
    public class AppModel
    {
        public IObservable<DateTime> JustTicksArrived { get; private set; }
        public IObservableGetProperty<DateTime> JustTicks { get; private set; }

        public IObservableGetProperty<int> Hour { get; private set; }
        public IObservableGetProperty<int> Minute { get; private set; }

        public IObservableGetProperty<double> HourInDouble { get; private set; }
        public IObservableGetProperty<double> SecondInDouble { get; private set; }

        public AppModel()
        {
            var initialTime = DateTime.Now;

            JustTicksArrived = new PeriodicTimer2(TimeSpan.FromSeconds(0.1), () => GetNextJustTicks(initialTime));
            JustTicks = JustTicksArrived.ToGetProperty(GetJustTicks(initialTime));

            Hour = JustTicks
                .Map(dt => dt.Hour)
                .ToGetProperty(initialTime.Hour);
            Minute = JustTicks
                .Map(dt => dt.Minute)
                .ToGetProperty(initialTime.Minute);

            HourInDouble = JustTicks
                .Filter(dt => dt.Second == 0 && dt.Millisecond == 0)
                .Map(dt => dt.Hour + dt.Minute / 60.0)
                .ToGetProperty(initialTime.Hour + initialTime.Minute / 60.0);
            SecondInDouble = JustTicks
                .Map(dt => dt.Second + dt.Millisecond / 1000.0)
                .ToGetProperty(initialTime.Second + initialTime.Millisecond / 1000.0);
        }

        static DateTime GetJustTicks(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, (dt.Millisecond / 100) * 100);
        }

        static DateTime GetNextJustTicks(DateTime dt)
        {
            return GetJustTicks(dt).AddSeconds(0.1);
        }
    }
}
