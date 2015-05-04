using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using KLibrary.Labs.ObservableModel;

namespace VisionPlate
{
    public class AppModel : DispatchableBase
    {
        static readonly Size BitmapSize = new Size(640, 480);

        // 指定された解像度に最も近いものを探します。
        static readonly Func<VideoCapabilities[], VideoCapabilities> GetResolution = cs => cs
            .OrderBy(c => Math.Abs(c.FrameSize.Width - BitmapSize.Width))
            .ThenBy(c => Math.Abs(c.FrameSize.Height - BitmapSize.Height))
            .FirstOrDefault();

        public ISettableProperty<object> SwitchDevice { get; private set; }
        public ISettableProperty<object> ReverseBitmap { get; private set; }

        public ISettableProperty<BitmapFrame> VideoBitmap { get; private set; }

        public ISettableProperty<bool> IsRunning { get; private set; }
        public IGetOnlyProperty<int> SelectedDeviceIndex { get; private set; }
        public IGetOnlyProperty<int> BitmapScaleX { get; private set; }

        VideoCaptureDevice[] _devices;
        IDisposable _videoFrameSubscription;

        public AppModel()
        {
            SwitchDevice = ObservableProperty.CreateSettable<object>(null, true);
            ReverseBitmap = ObservableProperty.CreateSettable<object>(null, true);

            VideoBitmap = ObservableProperty.CreateSettable<BitmapFrame>(null);
            IsRunning = ObservableProperty.CreateSettable(false);

            var oldNewIndexes = SwitchDevice
                .Select(_ => new
                {
                    OldValue = SelectedDeviceIndex.Value,
                    NewValue = (SelectedDeviceIndex.Value + 1) % _devices.Length
                })
                .ToGetOnly(null);
            SelectedDeviceIndex = oldNewIndexes
                .Select(_ => _.NewValue)
                .ToGetOnly(0);

            BitmapScaleX = ReverseBitmap
                .Select(_ => -1 * BitmapScaleX.Value)
                .ToGetOnly(-1);

            _devices = new FilterInfoCollection(FilterCategory.VideoInputDevice)
                .Cast<FilterInfo>()
                .Select(f => new VideoCaptureDevice(f.MonikerString))
                .Do(d => d.VideoResolution = GetResolution(d.VideoCapabilities))
                .ToArray();
            if (_devices.Length == 0) return;

            IsRunning
                .ObserveOn(Scheduler.Default)
                .Subscribe(b =>
                {
                    if (b)
                    {
                        StartDevice(SelectedDeviceIndex.Value);
                    }
                    else
                    {
                        StopDevice(SelectedDeviceIndex.Value);
                    }
                });

            oldNewIndexes
                .Where(_ => IsRunning.Value)
                .ObserveOn(Scheduler.Default)
                .Subscribe(_ =>
                {
                    StopDevice(_.OldValue);
                    // 連続してデバイスを操作すると失敗することがあるため、待機します。
                    Thread.Sleep(200);
                    StartDevice(_.NewValue);
                });
        }

        void StartDevice(int index)
        {
            var device = _devices[index];

            _videoFrameSubscription = Observable.FromEventPattern<NewFrameEventArgs>(device, "NewFrame")
                .Select(_ => _.EventArgs.Frame)
                .Select(DrawingHelper.ToBitmapFrame)
                .Subscribe(VideoBitmap);
            device.Start();
        }

        void StopDevice(int index)
        {
            var device = _devices[index];

            // VideoCaptureDevice を停止させないとアプリケーションが終了しないようです。
            device.SignalToStop();
            if (_videoFrameSubscription != null) _videoFrameSubscription.Dispose();
            VideoBitmap.Value = null;
        }
    }
}
