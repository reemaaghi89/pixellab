using System;
using System.Drawing;

namespace pixellab.Converters
{
    public static class YcbcrConverter
    {
        // RGB -> YCbCr
        public static (double Y, double Cb, double Cr) FromRgb(Color color)
        {
            int r = color.R; int g = color.G; int b = color.B;

            double y = 16 + (65.481 * r + 128.553 * g + 24.966 * b) / 256d;
            double cb = 128 + (-37.797 * r - 74.203 * g + 112.0 * b) / 256d;
            double cr = 128 + (112.0 * r - 93.786 * g - 18.214 * b) / 256d;

            return (y, cb, cr);
        }

        // YCbCr -> RGB
        public static Color ToRgb(double y, double cb, double cr)
        {
            double y_Y = y - 16;
            double cb_Y = cb - 128;
            double cr_Y = cr - 128;

            int r = Convert.ToInt32(1.164 * y_Y + 1.596 * cr_Y);
            int g = Convert.ToInt32(1.164 * y_Y - 0.392 * cb_Y - 0.813 * cr_Y);
            int b = Convert.ToInt32(1.164 * y_Y + 2.017 * cb_Y);

            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));

            return Color.FromArgb(255, r, g, b);
        }
    }
}