using System;
using System.Drawing;
using System.Windows.Forms; // مشان يتعرف على MouseEventArgs
using pixellab.Converters;

namespace pixellab
{
    public static class AnalyzeClickedPixelClass
    {
        public static (Color? SelectedColor, string Report) AnalyzeClickedPixel(
            Bitmap currentImage, 
            int pictureBoxWidth, 
            int pictureBoxHeight, 
            MouseEventArgs mouseArgs)
        {
            int imgWidth = currentImage.Width;
            int imgHeight = currentImage.Height;

            float ratioX = (float)pictureBoxWidth / imgWidth;
            float ratioY = (float)pictureBoxHeight / imgHeight;
            float ratio = Math.Min(ratioX, ratioY);

            float itemWidth = imgWidth * ratio;
            float itemHeight = imgHeight * ratio;

            float leftMargin = (pictureBoxWidth - itemWidth) / 2;
            float topMargin = (pictureBoxHeight - itemHeight) / 2;

            int pixelX = (int)((mouseArgs.X - leftMargin) / ratio);
            int pixelY = (int)((mouseArgs.Y - topMargin) / ratio);

            if (pixelX >= 0 && pixelX < imgWidth && pixelY >= 0 && pixelY < imgHeight)
            {
                Color pixelColor = currentImage.GetPixel(pixelX, pixelY);

                var (h, s, v) = HsvConverter.FromRgb(pixelColor);
                var (cyan, magenta, yellow, black) = CmykConverter.FromRgb(pixelColor);
                var (y_u, u, v_u) = YuvConverter.FromRgb(pixelColor);
                var (y_cb, cb, cr) = YcbcrConverter.FromRgb(pixelColor);
                var (labL, labA, labB) = LabConverter.FromRgb(pixelColor);

                string reportText = " Color Analysis and Space Synchronization Results:\n" +
                                    "━━━━━━━━━━━━━━━━━━━━━\n" +
                                    $" Real Coordinates: X: {pixelX}, Y: {pixelY}\n\n" +
                                    $" RGB:\n   R = {pixelColor.R}, G = {pixelColor.G}, B = {pixelColor.B}\n\n" +
                                    $" HSV:\n   H = {h:0}°, S = {s * 100:0}%, V = {v * 100:0}%\n\n" +
                                    $" CMYK (Printing System):\n   C = {cyan * 100:0}%, M = {magenta * 100:0}%, Y = {yellow * 100:0}%, K = {black * 100:0}%\n\n" +
                                    $" LAB (Visual Perception Space):\n   L = {labL:0.0}, A = {labA:0.0}, B = {labB:0.0}\n\n" +
                                    $" YUV (Synchronized Broadcasting):\n   Y = {y_u:0.0}, U = {u:0.0}, V = {v_u:0.0}\n\n" +
                                    $" YCbCr (Digital Standard):\n   Y = {y_cb:0.0}, Cb = {cb:0.0}, Cr = {cr:0.0}";

                return (pixelColor, reportText);
               
            }

            return (null, string.Empty);
        }
    }
}