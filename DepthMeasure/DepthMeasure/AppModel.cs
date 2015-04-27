using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KLibrary.Labs.ObservableModel;
using Microsoft.Kinect;

namespace DepthMeasure
{
    public class AppModel
    {
        const double Frequency = 30;
        static readonly TimeSpan FramesInterval = TimeSpan.FromSeconds(1 / Frequency);

        const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
        const DepthImageFormat DepthFormat = DepthImageFormat.Resolution640x480Fps30;

        Int32Rect _colorRect;
        int _colorStride;

        public ISettableProperty<WriteableBitmap> ColorBitmap { get; private set; }
        ISettableProperty<DepthImagePoint[]> _colorToDepth;

        public ISettableProperty<Point> SelectedPosition { get; private set; }
        public IGetOnlyProperty<int> SelectedDepth { get; private set; }

        public AppModel()
        {
            ColorBitmap = ObservableProperty.CreateSettable<WriteableBitmap>(null);
            _colorToDepth = ObservableProperty.CreateSettable(new DepthImagePoint[640 * 480]);

            SelectedPosition = ObservableProperty.CreateSettable(new Point(320, 240));
            SelectedDepth = ObservableProperty.CreateGetOnly(() => _colorToDepth.Value[(int)SelectedPosition.Value.X + 640 * (int)SelectedPosition.Value.Y].Depth);

            _colorToDepth.Subscribe(SelectedDepth);
            SelectedPosition.Subscribe(SelectedDepth);

            var kinect = new AsyncKinectManager();
            kinect.SensorConnected
                .Do(sensor =>
                {
                    sensor.ColorStream.Enable(ColorFormat);
                    sensor.DepthStream.Enable(DepthFormat);

                    _colorRect = new Int32Rect(0, 0, sensor.ColorStream.FrameWidth, sensor.ColorStream.FrameHeight);
                    _colorStride = sensor.ColorStream.FrameBytesPerPixel * sensor.ColorStream.FrameWidth;

                    try
                    {
                        sensor.Start();
                    }
                    catch (Exception ex)
                    {
                        // センサーが他のプロセスに既に使用されている場合に発生します。
                        Debug.WriteLine(ex);
                    }
                })
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ => ColorBitmap.Value = new WriteableBitmap(_colorRect.Width, _colorRect.Height, 96.0, 96.0, PixelFormats.Bgr32, null));
            kinect.SensorDisconnected
                .Do(_ => ColorBitmap.Value = null)
                .Subscribe(sensor => sensor.Stop());
            kinect.Initialize();

            var frameData = Observable.Interval(FramesInterval)
                .Select(_ => new
                {
                    ColorData = kinect.Sensor.Value.GetColorData(FramesInterval),
                    DepthData = kinect.Sensor.Value.GetDepthData(FramesInterval),
                })
                .ToGetOnly(null);

            frameData
                .Where(_ => _.ColorData != null)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ =>
                {
                    var b = ColorBitmap.Value;
                    if (b != null) b.WritePixels(_colorRect, _.ColorData, _colorStride, 0);
                });

            frameData
                .Where(_ => _.DepthData != null)
                .Select(_ =>
                {
                    var cd = new DepthImagePoint[_colorRect.Width * _colorRect.Height];
                    kinect.Sensor.Value.CoordinateMapper.MapColorFrameToDepthFrame(ColorFormat, DepthFormat, _.DepthData, cd);
                    return cd;
                })
                .Subscribe(_colorToDepth);
        }
    }
}
