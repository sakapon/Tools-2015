using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace InkScreen
{
    /// <summary>
    /// Provides the helper methods for controls.
    /// </summary>
    public static class ControlsHelper
    {
        /// <summary>
        /// Makes the window borderless and enables transparency.
        /// Can not be called after the window has been shown.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void SetBorderless(this Window window)
        {
            window.WindowStyle = WindowStyle.None;
            window.AllowsTransparency = true;
        }

        /// <summary>
        /// Relocates and resizes the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="bounds">The location and the size.</param>
        public static void Relocate(this Window window, Int32Rect bounds)
        {
            window.Left = bounds.X;
            window.Top = bounds.Y;
            window.Width = bounds.Width;
            window.Height = bounds.Height;
        }

        public static void FullScreenForAll(this Window window)
        {
            window.Relocate(ScreenHelper.AllScreensBounds);
        }

        public static void FullScreenForPrimary(this Window window)
        {
            window.Relocate(ScreenHelper.PrimaryScreenBounds);
        }
    }

    /// <summary>
    /// Provides the helper methods for screen.
    /// </summary>
    public static class ScreenHelper
    {
        /// <summary>
        /// Gets the bounds of all screens.
        /// </summary>
        /// <value>The bounds of all screens.</value>
        public static Int32Rect AllScreensBounds
        {
            get { return _AllScreensBounds.Value; }
        }

        static readonly Lazy<Int32Rect> _AllScreensBounds = new Lazy<Int32Rect>(() => SystemInformation.VirtualScreen.ToInt32Rect());

        /// <summary>
        /// Gets the bounds of the primary screen.
        /// </summary>
        /// <value>The bounds of the primary screen.</value>
        public static Int32Rect PrimaryScreenBounds
        {
            get { return _PrimaryScreenBounds.Value; }
        }

        static readonly Lazy<Int32Rect> _PrimaryScreenBounds = new Lazy<Int32Rect>(() => Screen.PrimaryScreen.Bounds.ToInt32Rect());

        public static Point GetLeftTop(this Int32Rect rect)
        {
            return new Point(rect.X, rect.Y);
        }

        public static Point GetLeftBottom(this Int32Rect rect)
        {
            return new Point(rect.X, rect.Y + rect.Height - 1);
        }

        public static Point GetRightTop(this Int32Rect rect)
        {
            return new Point(rect.X + rect.Width - 1, rect.Y);
        }

        public static Point GetRightBottom(this Int32Rect rect)
        {
            return new Point(rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
        }
    }

    public static class Convert
    {
        public static Int32Rect ToInt32Rect(this System.Drawing.Rectangle r)
        {
            return new Int32Rect(r.X, r.Y, r.Width, r.Height);
        }
    }
}
