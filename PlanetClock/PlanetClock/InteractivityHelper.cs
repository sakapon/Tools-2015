using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace PlanetClock
{
    public static class InteractivityHelper
    {
        public static void SetAffineTransform(this UIElement element)
        {
            if (element.RenderTransform == null || element.RenderTransform == MatrixTransform.Identity)
            {
                element.RenderTransformOrigin = new Point(0.5, 0.5);

                var transform = new TransformGroup();
                transform.Children.Add(new ScaleTransform());
                transform.Children.Add(new SkewTransform());
                transform.Children.Add(new RotateTransform());
                transform.Children.Add(new TranslateTransform());
                element.RenderTransform = transform;
            }
            else if (!(element.RenderTransform is TransformGroup))
            {
                throw new InvalidOperationException("RenderTransform プロパティの設定を解除してください。");
            }
        }
    }
}
