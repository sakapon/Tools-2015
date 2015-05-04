using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Video.DirectShow;
using KLibrary.Labs.ObservableModel;

namespace VisionPlate
{
    public class AppModel : DispatchableBase
    {
        public ISettableProperty<BitmapFrame> VideoBitmap { get; private set; }

        public ISettableProperty<object> SwitchDevice { get; private set; }
        public ISettableProperty<object> StopVideo { get; private set; }

        FilterInfoCollection _deviceInfoes;
        int _selectedDeviceIndex;
        VideoCaptureDevice2 _selectedVideoDevice;
        IDisposable _selectedVideoFrame;

        public AppModel()
        {
            VideoBitmap = ObservableProperty.CreateSettable<BitmapFrame>(null);

            SwitchDevice = ObservableProperty.CreateSettable<object>(null, true);
            SwitchDevice.ObserveOn(Scheduler.Default)
                .Subscribe(_ => StartDevice((_selectedDeviceIndex + 1) % _deviceInfoes.Count));

            StopVideo = ObservableProperty.CreateSettable<object>(null, true);
            StopVideo.Subscribe(_ =>
            {
                if (_selectedVideoDevice != null) _selectedVideoDevice.StopAsync();
            });

            Task.Run(() => InitializeDevice());
        }

        void InitializeDevice()
        {
            _deviceInfoes = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (_deviceInfoes.Count == 0) return;

            StartDevice(0);
        }

        async void StartDevice(int deviceIndex)
        {
            if (_selectedVideoFrame != null) _selectedVideoFrame.Dispose();
            if (_selectedVideoDevice != null) _selectedVideoDevice.Dispose();
            VideoBitmap.Value = null;

            // 連続してデバイスを操作すると失敗することがあるため、待機します。
            await Task.Delay(200);

            _selectedDeviceIndex = deviceIndex;
            var deviceInfo = _deviceInfoes[_selectedDeviceIndex];
            _selectedVideoDevice = new VideoCaptureDevice2(deviceInfo.MonikerString, new Size(640, 480));
            _selectedVideoFrame = _selectedVideoDevice.FrameArrived.Select(DrawingHelper.ToBitmapFrame).Subscribe(VideoBitmap);
        }
    }
}
