using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PlanetClock
{
    public static class AnimationHelper
    {
        public static Storyboard CreateFadeAnimation(UIElement element, double opacity, TimeSpan span)
        {
            var storyboard = new Storyboard();

            var opacityFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(opacityFrames, element);
            Storyboard.SetTargetProperty(opacityFrames, new PropertyPath("(UIElement.Opacity)"));
            opacityFrames.KeyFrames.Add(new EasingDoubleKeyFrame(opacity, span));
            storyboard.Children.Add(opacityFrames);

            return storyboard;
        }

        public static Storyboard CreateUpdateTextFadeAnimation(TextBlock textBlock, string text, TimeSpan totalSpan)
        {
            var storyboard = new Storyboard();

            var opacityFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(opacityFrames, textBlock);
            Storyboard.SetTargetProperty(opacityFrames, new PropertyPath("(UIElement.Opacity)"));
            opacityFrames.KeyFrames.Add(new EasingDoubleKeyFrame(0, TimeSpan.FromTicks(totalSpan.Ticks / 2)));
            opacityFrames.KeyFrames.Add(new EasingDoubleKeyFrame(1, totalSpan));
            storyboard.Children.Add(opacityFrames);

            var textFrames = new StringAnimationUsingKeyFrames();
            Storyboard.SetTarget(textFrames, textBlock);
            Storyboard.SetTargetProperty(textFrames, new PropertyPath("(TextBlock.Text)"));
            textFrames.KeyFrames.Add(new DiscreteStringKeyFrame(text, TimeSpan.FromTicks(totalSpan.Ticks / 2)));
            storyboard.Children.Add(textFrames);

            return storyboard;
        }
    }
}
