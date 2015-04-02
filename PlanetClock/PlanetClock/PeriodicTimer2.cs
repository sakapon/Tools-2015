using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using KLibrary.Labs.ObservableModel;

namespace PlanetClock
{
    public class PeriodicTimer2 : NotifierBase<DateTime>
    {
        bool _isActive;

        public TimeSpan Interval { get; private set; }
        public double SpeedRatio { get; private set; }
        Func<DateTime> _getInitialTime;

        public PeriodicTimer2(TimeSpan interval, Func<DateTime> getInitialTime, double speedRatio)
        {
            if (getInitialTime == null) throw new ArgumentNullException("getInitialTime");

            Interval = interval;
            SpeedRatio = speedRatio;
            _getInitialTime = getInitialTime;
        }

        protected override void OnObservationStarted()
        {
            _isActive = true;

            Task.Run(() =>
            {
                var nextTime = _getInitialTime();
                var nextTimePoint = nextTime;
                var timeout = (nextTime - DateTime.Now).TotalMilliseconds;
                Thread.Sleep(Math.Max(0, (int)Math.Ceiling(timeout)));
                NotifyNext(nextTimePoint);

                while (_isActive)
                {
                    nextTime += Interval;
                    if (SpeedRatio == 1.0)
                    {
                        nextTimePoint = nextTime;
                    }
                    else
                    {
                        nextTimePoint += TimeSpan.FromTicks((long)(SpeedRatio * Interval.Ticks));
                    }
                    timeout = (nextTime - DateTime.Now).TotalMilliseconds;
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
