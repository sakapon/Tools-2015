using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        VideoInput _selectedVideoInput;
        IDisposable _selectedVideoFrame;

        Int32Rect _bitmapRect;
        int _bitmapStride;

        public AppModel()
        {
            _VideoBitmap = ObservableProperty.CreateSettable<WriteableBitmap>(null);
            VideoBitmap = _VideoBitmap.ToGetOnlyMask();

            SwitchDevice = ObservableProperty.CreateSettable<object>(null, true);
            SwitchDevice.Subscribe(_ => StartDevice((_selectedDeviceIndex + 1) % _deviceInfoes.Count));

            StopVideo = ObservableProperty.CreateSettable<object>(null, true);
            StopVideo.Subscribe(_ =>
            {
                if (_selectedVideoInput != null) _selectedVideoInput.StopAsync();
            });

            Task.Run(() => InitializeDevice());
        }

        void InitializeDevice()
        {
            _deviceInfoes = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (_deviceInfoes.Count == 0) return;

            StartDevice(0);
        }

        void StartDevice(int deviceIndex)
        {
            if (_selectedVideoFrame != null) _selectedVideoFrame.Dispose();
            if (_selectedVideoInput != null) _selectedVideoInput.Dispose();

            _selectedDeviceIndex = deviceIndex;
            var deviceInfo = _deviceInfoes[_selectedDeviceIndex];
            _selectedVideoInput = new VideoInput(deviceInfo.MonikerString, new Size(640, 480));
            _selectedVideoFrame = _selectedVideoInput.FrameArrived.Subscribe(OnFrameArrived);
        }

        void OnFrameArrived(System.Drawing.Bitmap bitmap)
        {
            if (_VideoBitmap.Value == null)
            {
                InvokeOnInitialThread(() => _VideoBitmap.Value = new WriteableBitmap(bitmap.Width, bitmap.Height, 96.0, 96.0, PixelFormats.Rgb24, null));
                _bitmapRect = new Int32Rect(0, 0, bitmap.Width, bitmap.Height);
                _bitmapStride = 3 * bitmap.Width;
            }

            var bitmapBytes = ToBytes(bitmap);
            Array.Reverse(bitmapBytes);

            // BMP のヘッダーの 54 バイトはフッターとなり、無視されます。
            // 左右が反転します。
            InvokeOnInitialThreadAsync(() => _VideoBitmap.Value.WritePixels(_bitmapRect, bitmapBytes, _bitmapStride, 0));

            // BitmapFrame を使う方法。
            //TheImage.Source = ToBitmapFrame(eventArgs.Frame);
        }

        static BitmapFrame ToBitmapFrame(System.Drawing.Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        static byte[] ToBytes(System.Drawing.Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }
    }
}
