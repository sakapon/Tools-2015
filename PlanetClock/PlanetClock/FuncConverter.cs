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
            return DoFunc(ToFunc, value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DoFunc(FromFunc, value, parameter);
        }

        static object DoFunc(MulticastDelegate func, object value, object parameter)
        {
            if (func == null) return value;

            if (func.Method.ContainsGenericParameters) return Binding.DoNothing;

            var parameterInfoes = func.Method.GetParameters();

            switch (parameterInfoes.Length)
            {
                case 0: return func.DynamicInvoke();
                case 1: return func.DynamicInvoke(value);
                case 2: return func.DynamicInvoke(value, parameter);
                default: return Binding.DoNothing;
            }
        }
    }
}
