using System;
using System.Collections.Generic;
using System.Linq;
using KLibrary.Labs.Reactive;
using KLibrary.Labs.Reactive.Models;

namespace PlanetClock
{
    public class AppModel
    {
        public IObservable<DateTime> JustMinutesArrived { get; private set; }
        public IObservableGetProperty<int> Hour { get; private set; }
        public IObservableGetProperty<int> Minute { get; private set; }

        public AppModel()
        {
            JustMinutesArrived = new PeriodicTimer2(TimeSpan.FromMinutes(1), GetNextJustTime);
            Hour = JustMinutesArrived
                .Map(dt => dt.Hour)
                .ToGetProperty(DateTime.Now.Hour);
            Minute = JustMinutesArrived
                .Map(dt => dt.Minute)
                .ToGetProperty(DateTime.Now.Minute);
        }

        static DateTime GetNextJustTime()
        {
            var n = DateTime.Now.AddMinutes(1);
            return new DateTime(n.Year, n.Month, n.Day, n.Hour, n.Minute, 0);
        }
    }
}
