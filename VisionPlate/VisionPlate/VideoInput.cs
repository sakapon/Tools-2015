using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using AForge.Video;
using AForge.Video.DirectShow;
using KLibrary.Labs.ObservableModel;

namespace VisionPlate
{
    public class VideoInput : IDisposable
    {
        VideoCaptureDevice _device;

        public IGetOnlyProperty<Bitmap> FrameArrived { get; private set; }

        public VideoInput(string deviceMoniker)
        {
            _device = new VideoCaptureDevice(deviceMoniker);

            FrameArrived = Observable.FromEventPattern<NewFrameEventArgs>(_device, "NewFrame")
                .Select(_ => _.EventArgs.Frame)
                .ToGetOnly(null, true);

            // 指定された幅に最も近い VideoCapabilities。
            _device.VideoResolution = _device.VideoCapabilities.OrderBy(c => Math.Abs(c.FrameSize.Width - 960)).FirstOrDefault();
            _device.Start();
        }

        public void StopAsync()
        {
            if (_device.IsRunning) _device.SignalToStop();
        }

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
