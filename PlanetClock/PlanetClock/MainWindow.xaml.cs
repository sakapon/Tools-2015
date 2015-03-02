using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KLibrary.Labs.Reactive.Models;

namespace PlanetClock
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const double π = Math.PI;
        const double HourRadius = 100;

        public IObservableProperty<Vector> HourTranslate { get; private set; }
        public IObservableProperty<double> SecondAngle { get; private set; }

        public MainWindow()
        {
            HourTranslate = ObservableProperty.Create<Vector>();
            SecondAngle = ObservableProperty.Create<double>();

            InitializeComponent();

            var appModel = (AppModel)DataContext;

            appModel.Hour
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(h => AnimationHelper.CreateUpdateTextFadeAnimation(HourText, h.ToString(), TimeSpan.FromSeconds(0.4)).Begin(this));
            appModel.Minute
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(m => AnimationHelper.CreateUpdateTextFadeAnimation(MinuteText, m.ToString("D2"), TimeSpan.FromSeconds(0.4)).Begin(this));

            appModel.HourInDouble
                .Select(ToHourTranslateVector)
                .Subscribe(HourTranslate);
            HourTranslate.Value = ToHourTranslateVector(appModel.HourInDouble.Value);
            appModel.SecondInDouble
                .Select(s => s * 360 / 60)
                .Subscribe(SecondAngle);
            SecondAngle.Value = appModel.SecondInDouble.Value * 360 / 60;

            MouseLeftButtonDown += (o, e) => DragMove();
        }

        static Vector ToHourTranslateVector(double hour)
        {
            var hourAngle = hour * 2 * π / 12;
            return new Vector(
                HourRadius * Math.Sin(hourAngle),
                -HourRadius * Math.Cos(hourAngle)
            );
        }
    }
}
