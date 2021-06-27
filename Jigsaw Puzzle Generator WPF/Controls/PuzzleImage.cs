using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Jigsaw_Puzzle_Generator_WPF.Controls
{
    class PuzzleImage : Image
    {
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var source = (BitmapSource)Source;

            // Get the pixel of the source that was hit
            var x = (int)(hitTestParameters.HitPoint.X / ActualWidth * source.PixelWidth);
            var y = (int)(hitTestParameters.HitPoint.Y / ActualHeight * source.PixelHeight);

            // Copy the single pixel into a new byte array representing RGBA
            var pixel = new byte[4];
            source.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

            // Check the alpha (transparency) of the pixel
            // - threshold can be adjusted from 0 to 255
            if (pixel[3] < 10)
                return null;

            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
