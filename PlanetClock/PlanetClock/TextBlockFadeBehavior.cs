using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace PlanetClock
{
    public class TextBlockFadeBehavior : Behavior<TextBlock>
    {
        public static readonly DependencyProperty BindedTextProperty =
            DependencyProperty.Register("BindedText", typeof(string), typeof(TextBlockFadeBehavior), new PropertyMetadata(TextBlock.TextProperty.DefaultMetadata.DefaultValue, OnTextChanged));

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
            base.OnAttached();

            AssociatedObject.Text = BindedText;
        }

        static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (TextBlockFadeBehavior)d;
            if (@this.AssociatedObject == null) return;

            var storyboard = Animations.CreateFade(@this.AssociatedObject, @this.TotalSpan, (string)e.NewValue);
            storyboard.Begin();
        }
    }
}
