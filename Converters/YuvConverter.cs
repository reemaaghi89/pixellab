using System;
using System.Drawing;

namespace pixellab.Converters
{
    public static class YuvConverter
    {
        // RGB -> YUV
        public static (double Y, double U, double V) FromRgb(Color color)
        {
            int r = color.R; int g = color.G; int b = color.B;

            double y = 0.299 * r + 0.587 * g + 0.114 * b;
            double u = -0.14713 * r - 0.28886 * g + 0.436 * b;
            double v = 0.615 * r - 0.51499 * g - 0.10001 * b;

            return (y, u, v);
        }

        // YUV -> RGB
        public static Color ToRgb(double y, double u, double v)
        {
            int r = Convert.ToInt32(y + 1.13983 * v);
            int g = Convert.ToInt32(y - 0.39465 * u - 0.58060 * v);
            int b = Convert.ToInt32(y + 2.03211 * u);

            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));

            return Color.FromArgb(255, r, g, b);
        }
    }
}