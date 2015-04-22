using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using KLibrary.Labs.ObservableModel;

namespace VisionPlate
{
    public abstract class Dispatchable
    {
        SynchronizationContext _uiContext = SynchronizationContext.Current;

        protected void InvokeOnUIThread(Action action)
        {
            if (action == null) return;
            if (_uiContext == null) return;

            _uiContext.Send(o => action(), null);
        }

        protected void InvokeOnUIThreadAsync(Action action)
        {
            if (action == null) return;
            if (_uiContext == null) return;

            _uiContext.Post(o => action(), null);
        }
    }

    public class AppModel : Dispatchable
    {
        ISettableProperty<WriteableBitmap> _VideoBitmap;
        public IGetOnlyProperty<WriteableBitmap> VideoBitmap { get; private set; }

        public ISettableProperty<object> SwitchToNextDevice { get; private set; }
        public ISettableProperty<object> StopVideo { get; private set; }

        Int32Rect _bitmapRect;
        int _bitmapStride;

        public AppModel()
        {
            _VideoBitmap = ObservableProperty.CreateSettable<WriteableBitmap>(null);
            VideoBitmap = _VideoBitmap.ToGetOnlyMask();

            SwitchToNextDevice = ObservableProperty.CreateSettable<object>(null, true);
            StopVideo = ObservableProperty.CreateSettable<object>(null, true);

            Task.Run(() => InitializeDevice());
        }

        void InitializeDevice()
        {
            var filters = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (filters.Count == 0) return;

            var filter = filters[0];
            var capture = new VideoCaptureDevice(filter.MonikerString);
            // 指定された幅に最も近い VideoCapabilities。
            capture.VideoResolution = capture.VideoCapabilities.OrderBy(c => Math.Abs(c.FrameSize.Width - 960)).FirstOrDefault();
            capture.NewFrame += (o, e) => InvokeOnUIThread(() => capture_NewFrame(o, e));

            StopVideo.Subscribe(_ => capture.SignalToStop());
            capture.Start();
        }

        void capture_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var bitmap = eventArgs.Frame;

            if (_VideoBitmap.Value == null)
            {
                _VideoBitmap.Value = new WriteableBitmap(bitmap.Width, bitmap.Height, 96.0, 96.0, PixelFormats.Rgb24, null);
                _bitmapRect = new Int32Rect(0, 0, bitmap.Width, bitmap.Height);
                _bitmapStride = 3 * bitmap.Width;
            }

            var bitmapBytes = ToBytes(bitmap);
            Array.Reverse(bitmapBytes);
            // BMP のヘッダーの 54 バイトはフッターとなり、無視されます。
            // 左右が反転します。
            _VideoBitmap.Value.WritePixels(_bitmapRect, bitmapBytes, _bitmapStride, 0);

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
