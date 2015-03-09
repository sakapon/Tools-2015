using System;
using System.Collections.Generic;
using System.Linq;
using KLibrary.Labs.Reactive;
using KLibrary.Labs.Reactive.Models;

namespace PlanetClock
{
    public class AppModel
    {
        const int TickMilliseconds = 20;
        static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(TickMilliseconds);

        public IObservable<DateTime> JustTicksArrived { get; private set; }
        public IObservableGetProperty<DateTime> JustTicks { get; private set; }

        public IObservableGetProperty<int> Hour { get; private set; }
        public IObservableGetProperty<int> Minute { get; private set; }

        public IObservableGetProperty<double> HourInDouble { get; private set; }
        public IObservableGetProperty<double> SecondInDouble { get; private set; }

        public AppModel()
        {
            var initialTime = DateTime.Now;

            JustTicksArrived = new PeriodicTimer2(TickInterval, () => GetNextJustTicks(initialTime));
            JustTicks = JustTicksArrived.ToGetProperty(GetJustTicks(initialTime));

            Hour = JustTicks.ToGetProperty(dt => dt.Hour);
            Minute = JustTicks.ToGetProperty(dt => dt.Minute);

            HourInDouble = JustTicks
                .Filter(dt => dt.Second == 0 && dt.Millisecond == 0)
                .Map(ToHourInDouble)
                .ToGetProperty(ToHourInDouble(initialTime));
            SecondInDouble = JustTicks
                .ToGetProperty(ToSecondInDouble);
        }

        static DateTime GetJustTicks(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, (dt.Millisecond / TickMilliseconds) * TickMilliseconds);
        }

        static DateTime GetNextJustTicks(DateTime dt)
        {
            return GetJustTicks(dt).Add(TickInterval);
        }

        static readonly Func<DateTime, double> ToHourInDouble = dt => dt.Hour + dt.Minute / 60.0;
        static readonly Func<DateTime, double> ToSecondInDouble = dt => dt.Second + dt.Millisecond / 1000.0;
    }
}
