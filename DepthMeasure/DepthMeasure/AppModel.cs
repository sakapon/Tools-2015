using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using KLibrary.Labs.ObservableModel;
using Microsoft.Kinect;

namespace DepthMeasure
{
    public class AppModel
    {
        static readonly TimeSpan FramesInterval = TimeSpan.FromSeconds(1 / 30.0);
        static readonly ColorBitmapInfo ColorBitmapInfo = BitmapInfo.ForColor(ColorImageFormat.RgbResolution640x480Fps30);
        static readonly DepthBitmapInfo DepthBitmapInfo = BitmapInfo.ForDepth(DepthImageFormat.Resolution640x480Fps30);

        public IGetOnlyProperty<WriteableBitmap> ColorBitmap { get; private set; }

        public ISettableProperty<Point> SelectedPosition { get; private set; }
        public IGetOnlyProperty<int> SelectedDepth { get; private set; }

        public AppModel()
        {
            var kinect = new AsyncKinectManager();
            ColorBitmap = kinect.Sensor
                .ObserveOn(SynchronizationContext.Current)
                .Select(sensor => sensor != null ? ColorBitmapInfo.CreateBitmap() : null)
                .ToGetOnly(null);
            kinect.SensorConnected
                .Subscribe(sensor =>
                {
                    sensor.ColorStream.Enable(ColorBitmapInfo.Format);
                    sensor.DepthStream.Enable(DepthBitmapInfo.Format);

                    try
                    {
                        sensor.Start();
                    }
                    catch (Exception ex)
                    {
                        // センサーが他のプロセスに既に使用されている場合に発生します。
                        Debug.WriteLine(ex);
                    }
                });
            kinect.SensorDisconnected
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
                .Subscribe(_ => ColorBitmapInfo.WritePixels(ColorBitmap.Value, _.ColorData));

            var colorDepthMap = frameData
                .Where(_ => _.DepthData != null)
                .Select(_ =>
                {
                    var map = new DepthImagePoint[ColorBitmapInfo.PixelsCount];
                    kinect.Sensor.Value.CoordinateMapper.MapColorFrameToDepthFrame(ColorBitmapInfo.Format, DepthBitmapInfo.Format, _.DepthData, map);
                    return map;
                })
                .ToGetOnly(new DepthImagePoint[ColorBitmapInfo.PixelsCount]);

            SelectedPosition = ObservableProperty.CreateSettable(new Point(ColorBitmapInfo.Width / 2, ColorBitmapInfo.Height / 2));

            SelectedDepth = ObservableProperty.CreateGetOnly(() => colorDepthMap.Value[PositionToPixelIndex(SelectedPosition.Value)].Depth);
            colorDepthMap.Subscribe(SelectedDepth);
            SelectedPosition.Subscribe(SelectedDepth);
        }

        static readonly Func<Point, int> PositionToPixelIndex = p => (int)p.X + ColorBitmapInfo.Width * (int)p.Y;
    }
}
