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

        IObservable<DateTime> _JustTicksArrived;
        public IObservableGetProperty<DateTime> JustTicks { get; private set; }

        public AppModel()
        {
            var initialTime = DateTime.Now;

            _JustTicksArrived = new PeriodicTimer2(TickInterval, () => GetNextJustTicks(initialTime));
            JustTicks = _JustTicksArrived.ToGetProperty(GetJustTicks(initialTime));
        }

        static readonly Func<DateTime, DateTime> GetJustTicks = dt =>
            new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, (dt.Millisecond / TickMilliseconds) * TickMilliseconds);

        static readonly Func<DateTime, DateTime> GetNextJustTicks = dt =>
            GetJustTicks(dt).Add(TickInterval);
    }
}
