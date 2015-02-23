using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PlanetClock
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            HourLayer.SetAffineTransform();
            SecondLayer.SetAffineTransform();

            var translate = ((TransformGroup)HourLayer.RenderTransform).Children.OfType<TranslateTransform>().Single();
            translate.X = 0;
            translate.Y = -110;

            MouseLeftButtonDown += (o, e) => DragMove();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var secondAnimation = CreateRotationAnimation(SecondLayer, 6 * GetCurrentSecond(), TimeSpan.FromMinutes(1));
            secondAnimation.Begin();
        }

        static double GetCurrentSecond()
        {
            var now = DateTime.Now;
            return now.Second + now.Millisecond / 1000.0;
        }

        static Storyboard CreateRotationAnimation(UIElement element, double initialAngle, TimeSpan interval)
        {
            var storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever,
            };

            var angleFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(angleFrames, element);
            Storyboard.SetTargetProperty(angleFrames, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
            angleFrames.KeyFrames.Add(new EasingDoubleKeyFrame(initialAngle, TimeSpan.Zero));
            angleFrames.KeyFrames.Add(new EasingDoubleKeyFrame(initialAngle + 360, interval));
            storyboard.Children.Add(angleFrames);

            return storyboard;
        }
    }
}
