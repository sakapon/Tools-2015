using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using KLibrary.Labs.Reactive;

namespace PlanetClock
{
    public class PeriodicTimer2 : NotifierBase<DateTime>
    {
        bool _isActive;

        public TimeSpan Interval { get; private set; }
        Func<DateTime> _getInitialTime;

        public PeriodicTimer2(TimeSpan interval, Func<DateTime> getInitialTime)
        {
            if (getInitialTime == null) throw new ArgumentNullException("getInitialTime");

            Interval = interval;
            _getInitialTime = getInitialTime;
        }

        protected override void OnObservationStarted()
        {
            _isActive = true;

            Task.Run(() =>
            {
                var nextTimePoint = _getInitialTime();
                var timeout = (nextTimePoint - DateTime.Now).TotalMilliseconds;
                Thread.Sleep(Math.Max(0, (int)Math.Ceiling(timeout)));
                NotifyNext(nextTimePoint);

                while (_isActive)
                {
                    nextTimePoint += Interval;
                    timeout = (nextTimePoint - DateTime.Now).TotalMilliseconds;
                    Thread.Sleep(Math.Max(0, (int)Math.Ceiling(timeout)));
                    NotifyNext(nextTimePoint);
                }

                Debug.WriteLine("The thread for tick is end.");
            });
        }

        protected override void OnObservationStopped()
        {
            _isActive = false;
        }
    }
}
