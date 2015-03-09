using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace PlanetClock
{
    public class TextBlockFadeBehavior : Behavior<TextBlock>
    {
        // ここでは、PropertyMetadata の PropertyChangedCallback を使用しません。
        public static readonly DependencyProperty BindedTextProperty =
            DependencyProperty.Register("BindedText", typeof(string), typeof(TextBlockFadeBehavior), new PropertyMetadata(TextBlock.TextProperty.DefaultMetadata.DefaultValue));

        public string BindedText
        {
            get { return (string)GetValue(BindedTextProperty); }
            set { SetValue(BindedTextProperty, value); }
        }

        public static readonly DependencyProperty TotalSpanProperty =
            DependencyProperty.Register("TotalSpan", typeof(TimeSpan), typeof(TextBlockFadeBehavior), new PropertyMetadata(TimeSpan.FromSeconds(0.5)));

        public TimeSpan TotalSpan
        {
            get { return (TimeSpan)GetValue(TotalSpanProperty); }
            set { SetValue(TotalSpanProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Text = BindedText;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == BindedTextProperty) OnTextChanged(e);
        }

        void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject == null) return;

            var storyboard = Animations.CreateFade(AssociatedObject, TotalSpan, (string)e.NewValue);
            storyboard.Begin();
        }
    }
}
