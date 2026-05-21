using System;
using System.Drawing;

namespace pixellab
{
    class CustomColorConverter
    {
        // 1. تحويل من RGB إلى HSV
        public static void RGBtoHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        // 2. تحويل من RGB إلى CMYK
        public static void RGBtoCMYK(Color color, out double c, out double m, out double y, out double k)
        {
            double r = color.R / 255d;
            double g = color.G / 255d;
            double b = color.B / 255d;

            k = 1d - Math.Max(r, Math.Max(g, b));

            if (k == 1d)
            {
                c = 0; m = 0; y = 0;
            }
            else
            {
                c = (1d - r - k) / (1d - k);
                m = (1d - g - k) / (1d - k);
                y = (1d - b - k) / (1d - k);
            }
        }

        // 3. تحويل من RGB إلى YUV
        public static void RGBtoYUV(Color color, out double y, out double u, out double v)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            y = 0.299 * r + 0.587 * g + 0.114 * b;
            u = -0.14713 * r - 0.28886 * g + 0.436 * b;
            v = 0.615 * r - 0.51499 * g - 0.10001 * b;
        }

        // 4. تحويل من RGB إلى YCBCR
        public static void RGBtoYCbCr(Color color, out double y, out double cb, out double cr)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            y = 16 + (65.481 * r + 128.553 * g + 24.966 * b) / 256d;
            cb = 128 + (-37.797 * r - 74.203 * g + 112.0 * b) / 256d;
            cr = 128 + (112.0 * r - 93.786 * g - 18.214 * b) / 256d;
        }

        // 5. متطلب إضافي محترف: التحويل من RGB إلى LAB اللوني الشهير
        public static void RGBtoLAB(Color color, out double L, out double a, out double b)
        {
            // أ: أولاً نحول من RGB إلى فضاء XYZ الوسيط
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double bl = color.B / 255.0;

            r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
            g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
            bl = (bl > 0.04045) ? Math.Pow((bl + 0.055) / 1.055, 2.4) : bl / 12.92;

            r *= 100; g *= 100; bl *= 100;

            double X = r * 0.4124 + g * 0.3576 + bl * 0.1805;
            double Y = r * 0.2126 + g * 0.7152 + bl * 0.0722;
            double Z = r * 0.0193 + g * 0.1192 + bl * 0.9505;

            // ب: من فضاء XYZ إلى الفضاء النهائي LAB بناءً على النقطة المرجعية الإضاءية D65
            X /= 95.047; Y /= 100.0; Z /= 108.883;

            X = (X > 0.008856) ? Math.Pow(X, 1.0 / 3.0) : (7.787 * X) + (16.0 / 116.0);
            Y = (Y > 0.008856) ? Math.Pow(Y, 1.0 / 3.0) : (7.787 * Y) + (16.0 / 116.0);
            Z = (Z > 0.008856) ? Math.Pow(Z, 1.0 / 3.0) : (7.787 * Z) + (16.0 / 116.0);

            L = (116.0 * Y) - 16.0;
            a = 500.0 * (X - Y);
            b = 200.0 * (Y - Z);
        }
    }
}