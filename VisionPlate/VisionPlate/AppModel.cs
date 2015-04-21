using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using KLibrary.Labs.ObservableModel;

namespace VisionPlate
{
    public class AppModel
    {
        ISettableProperty<WriteableBitmap> _VideoBitmap;
        public IGetOnlyProperty<WriteableBitmap> VideoBitmap { get; private set; }

        public AppModel()
        {
            _VideoBitmap = ObservableProperty.CreateSettable<WriteableBitmap>(null);
            VideoBitmap = _VideoBitmap.ToGetOnlyMask();
        }
    }
}
