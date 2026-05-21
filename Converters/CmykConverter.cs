using System;
using System.Drawing;

namespace pixellab.Converters
{
    public static class CmykConverter
    {
        // RGB -> CMYK
        public static (double C, double M, double Y, double K) FromRgb(Color color)
        {
            double r = color.R / 255d;
            double g = color.G / 255d;
            double b = color.B / 255d;

            double k = 1d - Math.Max(r, Math.Max(g, b));
            double c = (k == 1d) ? 0 : (1d - r - k) / (1d - k);
            double m = (k == 1d) ? 0 : (1d - g - k) / (1d - k);
            double y = (k == 1d) ? 0 : (1d - b - k) / (1d - k);

            return (c, m, y, k);
        }

        // CMYK -> RGB
        public static Color ToRgb(double c, double m, double y, double k)
        {
            int r = Convert.ToInt32(255 * (1 - c) * (1 - k));
            int g = Convert.ToInt32(255 * (1 - m) * (1 - k));
            int b = Convert.ToInt32(255 * (1 - y) * (1 - k));
            return Color.FromArgb(255, r, g, b);
        }
    }
}