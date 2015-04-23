using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using AForge.Video;
using AForge.Video.DirectShow;
using KLibrary.Labs.ObservableModel;

namespace VisionPlate
{
    public class VideoInput : IDisposable
    {
        VideoCaptureDevice _device;

        public IGetOnlyProperty<System.Drawing.Bitmap> FrameArrived { get; private set; }

        public VideoInput(string deviceMoniker, Size size)
        {
            _device = new VideoCaptureDevice(deviceMoniker);

            FrameArrived = Observable.FromEventPattern<NewFrameEventArgs>(_device, "NewFrame")
                .Select(_ => _.EventArgs.Frame)
                .ToGetOnly(null, true);

            // 指定された解像度に最も近いものを探します。
            _device.VideoResolution = _device.VideoCapabilities
                .OrderBy(c => Math.Abs(c.FrameSize.Width - size.Width))
                .ThenBy(c => Math.Abs(c.FrameSize.Height - size.Height))
                .FirstOrDefault();
            _device.Start();
        }

        public void StopAsync()
        {
            if (_device.IsRunning) _device.SignalToStop();
        }

        // VideoCaptureDevice を終了させないとアプリケーションが終了せず、したがってデストラクターも呼び出されないようです。
        ~VideoInput()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                StopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
