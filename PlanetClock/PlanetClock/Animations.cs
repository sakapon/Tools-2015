using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PlanetClock
{
    public static class Animations
    {
        public static Storyboard CreateFade(TextBlock element, TimeSpan totalSpan, string newText)
        {
            var halfSpan = TimeSpan.FromTicks(totalSpan.Ticks / 2);

            var storyboard = new Storyboard();

            var opacityFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(opacityFrames, element);
            Storyboard.SetTargetProperty(opacityFrames, new PropertyPath(UIElement.OpacityProperty));
            opacityFrames.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, TimeSpan.Zero));
            opacityFrames.KeyFrames.Add(new EasingDoubleKeyFrame(0.0, halfSpan));
            opacityFrames.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, totalSpan));
            storyboard.Children.Add(opacityFrames);

            var textFrames = new StringAnimationUsingKeyFrames();
            Storyboard.SetTarget(textFrames, element);
            Storyboard.SetTargetProperty(textFrames, new PropertyPath(TextBlock.TextProperty));
            textFrames.KeyFrames.Add(new DiscreteStringKeyFrame(newText, halfSpan));
            storyboard.Children.Add(textFrames);

            return storyboard;
        }
    }
}
