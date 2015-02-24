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
        public IObservableGetProperty<DateTime> JustMinutes { get; private set; }

        public IObservableGetProperty<int> Hour { get; private set; }
        public IObservableGetProperty<int> Minute { get; private set; }

        public AppModel()
        {
            var initialTime = DateTime.Now;

            JustMinutesArrived = new PeriodicTimer2(TimeSpan.FromMinutes(1), () => GetNextJustTime(initialTime));
            JustMinutes = JustMinutesArrived.ToGetProperty(GetJustTime(initialTime));

            Hour = JustMinutes
                .Map(dt => dt.Hour)
                .ToGetProperty(initialTime.Hour);
            Minute = JustMinutes
                .Map(dt => dt.Minute)
                .ToGetProperty(initialTime.Minute);
        }

        static DateTime GetJustTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }

        static DateTime GetNextJustTime(DateTime dt)
        {
            return GetJustTime(dt.AddMinutes(1));
        }
    }
}
