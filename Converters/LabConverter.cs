using System;
using System.Drawing;

namespace pixellab.Converters
{
    public static class LabConverter
    {
        // RGB -> LAB
        public static (double L, double A, double B) FromRgb(Color color)
        {
            double r = color.R / 255.0; double g = color.G / 255.0; double bl = color.B / 255.0;

            r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
            g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
            bl = (bl > 0.04045) ? Math.Pow((bl + 0.055) / 1.055, 2.4) : bl / 12.92;

            r *= 100; g *= 100; bl *= 100;

            double X = r * 0.4124 + g * 0.3576 + bl * 0.1805;
            double Y = r * 0.2126 + g * 0.7152 + bl * 0.0722;
            double Z = r * 0.0193 + g * 0.1192 + bl * 0.9505;

            X /= 95.047; Y /= 100.0; Z /= 108.883;

            X = (X > 0.008856) ? Math.Pow(X, 1.0 / 3.0) : (7.787 * X) + (16.0 / 116.0);
            Y = (Y > 0.008856) ? Math.Pow(Y, 1.0 / 3.0) : (7.787 * Y) + (16.0 / 116.0);
            Z = (Z > 0.008856) ? Math.Pow(Z, 1.0 / 3.0) : (7.787 * Z) + (16.0 / 116.0);

            double L = (116.0 * Y) - 16.0;
            double a = 500.0 * (X - Y);
            double b = 200.0 * (Y - Z);

            return (L, a, b);
        }

        // LAB -> RGB
        public static Color ToRgb(double L, double a, double b)
        {
            double y = (L + 16.0) / 116.0;
            double x = a / 500.0 + y;
            double z = y - b / 200.0;

            x = (Math.Pow(x, 3) > 0.008856) ? Math.Pow(x, 3) : (x - 16.0 / 116.0) / 7.787;
            y = (Math.Pow(y, 3) > 0.008856) ? Math.Pow(y, 3) : (y - 16.0 / 116.0) / 7.787;
            z = (Math.Pow(z, 3) > 0.008856) ? Math.Pow(z, 3) : (z - 16.0 / 116.0) / 7.787;

            x *= 95.047; y *= 100.0; z *= 108.883;

            double r = x * 3.2406 + y * -1.5372 + z * -0.4986;
            double g = x * -0.9689 + y * 1.8758 + z * 0.0415;
            double bl = x * 0.0557 + y * -0.2040 + z * 1.0570;

            r /= 100.0; g /= 100.0; bl /= 100.0;

            r = (r > 0.0031308) ? 1.055 * Math.Pow(r, 1.0 / 2.4) - 0.055 : 12.92 * r;
            g = (g > 0.0031308) ? 1.055 * Math.Pow(g, 1.0 / 2.4) - 0.055 : 12.92 * g;
            bl = (bl > 0.0031308) ? 1.055 * Math.Pow(bl, 1.0 / 2.4) - 0.055 : 12.92 * bl;

            int R = Math.Max(0, Math.Min(255, Convert.ToInt32(r * 255)));
            int G = Math.Max(0, Math.Min(255, Convert.ToInt32(g * 255)));
            int B = Math.Max(0, Math.Min(255, Convert.ToInt32(bl * 255)));

            return Color.FromArgb(255, R, G, B);
        }
    }
}