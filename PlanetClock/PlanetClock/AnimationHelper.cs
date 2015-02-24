using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
    }
}
