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
        ISettableProperty<WriteableBitmap> _VideoBitmap;
        public IGetOnlyProperty<WriteableBitmap> VideoBitmap { get; private set; }

        public ISettableProperty<object> SwitchDevice { get; private set; }
        public ISettableProperty<object> StopVideo { get; private set; }

        FilterInfoCollection _deviceInfoes;
        int _selectedDeviceIndex;
        VideoCaptureDevice2 _selectedVideoDevice;
        IDisposable _selectedVideoFrame;

        Int32Rect _bitmapRect;
        int _bitmapStride;

        public AppModel()
        {
            _VideoBitmap = ObservableProperty.CreateSettable<WriteableBitmap>(null);
            VideoBitmap = _VideoBitmap.ToGetOnlyMask();

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
            _VideoBitmap.Value = null;

            // 連続してデバイスを操作すると失敗することがあるため、待機します。
            await Task.Delay(200);

            _selectedDeviceIndex = deviceIndex;
            var deviceInfo = _deviceInfoes[_selectedDeviceIndex];
            _selectedVideoDevice = new VideoCaptureDevice2(deviceInfo.MonikerString, new Size(640, 480));
            _selectedVideoFrame = _selectedVideoDevice.FrameArrived.Subscribe(OnFrameArrived);
            // BitmapFrame を使う方法。
            //_selectedVideoFrame = _selectedVideoDevice.FrameArrived.Select(DrawingHelper.ToBitmapFrame).Subscribe(_VideoBitmap);
        }

        void OnFrameArrived(System.Drawing.Bitmap bitmap)
        {
            if (_VideoBitmap.Value == null)
            {
                InvokeOnContext(() => _VideoBitmap.Value = new WriteableBitmap(bitmap.Width, bitmap.Height, 96.0, 96.0, PixelFormats.Rgb24, null));
                _bitmapRect = new Int32Rect(0, 0, bitmap.Width, bitmap.Height);
                _bitmapStride = 3 * bitmap.Width;
            }

            var bitmapBytes = DrawingHelper.ToBytes(bitmap);
            Array.Reverse(bitmapBytes);

            // BMP のヘッダーの 54 バイトはフッターとなり、無視されます。
            // 左右が反転します。
            var b = _VideoBitmap.Value;
            if (b != null) InvokeOnContextAsync(() => b.WritePixels(_bitmapRect, bitmapBytes, _bitmapStride, 0));
        }
    }
}
