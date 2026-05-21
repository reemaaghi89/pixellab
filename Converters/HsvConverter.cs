using System;
using System.Drawing;

namespace pixellab.Converters
{
    public static class HsvConverter
    {
        // RGB -> HSV
        public static (double Hue, double Saturation, double Value) FromRgb(Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            double hue = color.GetHue();
            double saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            double value = max / 255d;

            return (hue, saturation, value);
        }

        // HSV -> RGB 
        public static Color ToRgb(double h, double s, double v)
            {
            int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            double f = h / 60 - Math.Floor(h / 60);

            v = v * 255;
            int vInt = Convert.ToInt32(v);
            int p = Convert.ToInt32(v * (1 - s));
            int q = Convert.ToInt32(v * (1 - f * s));
            int t = Convert.ToInt32(v * (1 - (1 - f) * s));

            if (hi == 0) return Color.FromArgb(255, vInt, t, p);
            else if (hi == 1) return Color.FromArgb(255, q, vInt, p);
            else if (hi == 2) return Color.FromArgb(255, p, vInt, t);
            else if (hi == 3) return Color.FromArgb(255, p, q, vInt);
            else if (hi == 4) return Color.FromArgb(255, t, p, vInt);
            else return Color.FromArgb(255, vInt, p, q);
        }
    }
}