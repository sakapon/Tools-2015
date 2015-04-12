using System;
using System.Collections.Generic;
using System.Linq;
using KLibrary.Labs.ObservableModel;

namespace PlanetClock
{
    public class AppModel
    {
        const int TickMilliseconds = 40;
        static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(TickMilliseconds);

        public IGetOnlyProperty<DateTime> JustTicks { get; private set; }

        public AppModel()
        {
            var initialTime = DateTime.Now;

            JustTicks = new PeriodicTimer2(TickInterval, () => GetNextJustTicks(initialTime), 1)
                .ToGetOnly(GetJustTicks(initialTime));
        }

        static readonly Func<DateTime, DateTime> GetJustTicks = dt =>
            new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, (dt.Millisecond / TickMilliseconds) * TickMilliseconds);

        static readonly Func<DateTime, DateTime> GetNextJustTicks = dt =>
            GetJustTicks(dt).Add(TickInterval);
    }
}
