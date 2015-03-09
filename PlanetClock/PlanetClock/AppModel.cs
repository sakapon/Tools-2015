using System;
using System.Collections.Generic;
using System.Linq;
using KLibrary.Labs.Reactive.Models;

namespace PlanetClock
{
    public class AppModel
    {
        const int TickMilliseconds = 20;
        static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(TickMilliseconds);

        public IObservable<DateTime> JustTicksArrived { get; private set; }
        public IObservableGetProperty<DateTime> JustTicks { get; private set; }

        public AppModel()
        {
            var initialTime = DateTime.Now;

            JustTicksArrived = new PeriodicTimer2(TickInterval, () => GetNextJustTicks(initialTime));
            JustTicks = JustTicksArrived.ToGetProperty(GetJustTicks(initialTime));
        }

        static DateTime GetJustTicks(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, (dt.Millisecond / TickMilliseconds) * TickMilliseconds);
        }

        static DateTime GetNextJustTicks(DateTime dt)
        {
            return GetJustTicks(dt).Add(TickInterval);
        }
    }
}
