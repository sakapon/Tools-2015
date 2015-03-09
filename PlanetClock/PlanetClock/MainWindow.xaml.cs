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

        public AppModel AppModel { get; private set; }
        public IObservableGetProperty<Vector> HourTranslate { get; private set; }

        public MainWindow()
        {
            AppModel = new AppModel();
            HourTranslate = AppModel.HourInDouble.ToGetProperty(HourToTranslate);

            InitializeComponent();

            MouseLeftButtonDown += (o, e) => DragMove();
        }

        static Vector HourToTranslate(double hour)
        {
            var hourAngle = hour * (2 * π / 12);
            return HourRadius * new Vector(Math.Sin(hourAngle), -Math.Cos(hourAngle));
        }

        public static readonly Func<double, double> SecondToAngle = s => s * (360 / 60);
    }
}
