using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PlanetClock
{
    [ValueConversion(typeof(object), typeof(object))]
    public class FuncConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ToFuncProperty =
            DependencyProperty.Register("ToFunc", typeof(MulticastDelegate), typeof(FuncConverter), new PropertyMetadata(null));

        public static readonly DependencyProperty FromFuncProperty =
            DependencyProperty.Register("FromFunc", typeof(MulticastDelegate), typeof(FuncConverter), new PropertyMetadata(null));

        public MulticastDelegate ToFunc
        {
            get { return (MulticastDelegate)GetValue(ToFuncProperty); }
            set { SetValue(ToFuncProperty, value); }
        }

        public MulticastDelegate FromFunc
        {
            get { return (MulticastDelegate)GetValue(FromFuncProperty); }
            set { SetValue(FromFuncProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ToFunc == null) return Binding.DoNothing;

            return ToFunc.DynamicInvoke(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (FromFunc == null) return Binding.DoNothing;

            return FromFunc.DynamicInvoke(value);
        }
    }
}
