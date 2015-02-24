﻿using System;
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

namespace PlanetClock
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const double π = Math.PI;
        const double HourRadius = 110;

        public MainWindow()
        {
            InitializeComponent();

            HourLayer.SetAffineTransform();
            SecondLayer.SetAffineTransform();
            var hourTranslate = HourLayer.GetAffineTransform<TranslateTransform>();
            Action<DateTime> setHourAngle = dt =>
            {
                var hourAngle = GetHours(dt) * 2 * π / 12;
                hourTranslate.X = HourRadius * Math.Sin(hourAngle);
                hourTranslate.Y = -HourRadius * Math.Cos(hourAngle);
            };

            var appModel = (AppModel)DataContext;
            appModel.JustMinutesArrived
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(setHourAngle);
            setHourAngle(DateTime.Now);

            MouseLeftButtonDown += (o, e) => DragMove();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var secondAnimation = CreateRotationAnimation(SecondLayer, GetSeconds(DateTime.Now) * 360 / 60, TimeSpan.FromMinutes(1));
            secondAnimation.Begin(this);
        }

        static double GetHours(DateTime dt)
        {
            return dt.Hour + dt.Minute / 60.0;
        }

        static double GetSeconds(DateTime dt)
        {
            return dt.Second + dt.Millisecond / 1000.0;
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
