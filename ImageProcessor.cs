using System;
using System.Drawing;
using System.Drawing.Imaging;
using pixellab.Converters;

namespace pixellab
{
    public static class ImageProcessor
    {
        public static unsafe Bitmap ConvertToGrayscale(Bitmap source)
        {
            Bitmap bmp = new Bitmap(source);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* ptr = (byte*)data.Scan0;
            int bytes = data.Stride * bmp.Height;
            for (int i = 0; i < bytes; i += 4)
            {
                byte gray = (byte)(0.299 * ptr[i + 2] + 0.587 * ptr[i + 1] + 0.114 * ptr[i]);
                ptr[i] = ptr[i + 1] = ptr[i + 2] = gray;
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        public static unsafe Bitmap ConvertToBlackAndWhite(Bitmap source, byte threshold = 128)
        {
            Bitmap bmp = new Bitmap(source);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* ptr = (byte*)data.Scan0;
            int bytes = data.Stride * bmp.Height;
            for (int i = 0; i < bytes; i += 4)
            {
                byte intensity = (byte)(0.299 * ptr[i + 2] + 0.587 * ptr[i + 1] + 0.114 * ptr[i]);
                byte val = (intensity >= threshold) ? (byte)255 : (byte)0;
                ptr[i] = ptr[i + 1] = ptr[i + 2] = val;
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        public static unsafe Bitmap QuantizeImageColors(Bitmap source, int levels = 4)
        {
            Bitmap bmp = new Bitmap(source);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* ptr = (byte*)data.Scan0;
            int bytes = data.Stride * bmp.Height;
            int step = 256 / levels;
            for (int i = 0; i < bytes; i += 4)
            {
                ptr[i] = (byte)((ptr[i] / step) * step);
                ptr[i + 1] = (byte)((ptr[i + 1] / step) * step);
                ptr[i + 2] = (byte)((ptr[i + 2] / step) * step);
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        public static unsafe Bitmap ApplyRGBFast(Bitmap source, int r, int g, int b)
        {
            Bitmap bmp = new Bitmap(source);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* ptr = (byte*)data.Scan0;
            int bytes = data.Stride * bmp.Height;
            for (int i = 0; i < bytes; i += 4)
            {
                ptr[i + 2] = (byte)Math.Max(0, Math.Min(255, ptr[i + 2] + r));
                ptr[i + 1] = (byte)Math.Max(0, Math.Min(255, ptr[i + 1] + g));
                ptr[i] = (byte)Math.Max(0, Math.Min(255, ptr[i] + b));
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        public static unsafe Bitmap ApplyHSVAdjustment(Bitmap source, double hS, double sS, double vS)
        {
            Bitmap bmp = new Bitmap(source);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* ptr = (byte*)data.Scan0;
            int bytes = data.Stride * bmp.Height;
            for (int i = 0; i < bytes; i += 4)
            {
                var hsv = HsvConverter.FromRgb(Color.FromArgb(ptr[i + 2], ptr[i + 1], ptr[i]));
                double h = (hsv.Hue + hS) % 360; if (h < 0) h += 360;
                var nc = HsvConverter.ToRgb(h, Math.Max(0, Math.Min(1, hsv.Saturation + sS)), Math.Max(0, Math.Min(1, hsv.Value + vS)));
                ptr[i] = nc.B; ptr[i + 1] = nc.G; ptr[i + 2] = nc.R;
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        public static unsafe Bitmap ApplyCMYKAdjustment(Bitmap source, double cS, double mS, double yS, double kS)
        {
            Bitmap bmp = new Bitmap(source);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* ptr = (byte*)data.Scan0;
            int bytes = data.Stride * bmp.Height;
            for (int i = 0; i < bytes; i += 4)
            {
                var cmyk = CmykConverter.FromRgb(Color.FromArgb(ptr[i + 2], ptr[i + 1], ptr[i]));
                var nc = CmykConverter.ToRgb(Math.Max(0, Math.Min(1, cmyk.C + cS)), Math.Max(0, Math.Min(1, cmyk.M + mS)), Math.Max(0, Math.Min(1, cmyk.Y + yS)), Math.Max(0, Math.Min(1, cmyk.K + kS)));
                ptr[i] = nc.B; ptr[i + 1] = nc.G; ptr[i + 2] = nc.R;
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        public static unsafe Bitmap ApplyLABAdjustment(Bitmap source, double lS, double aS, double bS)
        {
            Bitmap bmp = new Bitmap(source);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* ptr = (byte*)data.Scan0;
            int bytes = data.Stride * bmp.Height;
            for (int i = 0; i < bytes; i += 4)
            {
                var lab = LabConverter.FromRgb(Color.FromArgb(ptr[i + 2], ptr[i + 1], ptr[i]));
                var nc = LabConverter.ToRgb(Math.Max(0, Math.Min(100, lab.L + lS)), Math.Max(-128, Math.Min(127, lab.A + aS)), Math.Max(-128, Math.Min(127, lab.B + bS)));
                ptr[i] = nc.B; ptr[i + 1] = nc.G; ptr[i + 2] = nc.R;
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        public static unsafe Bitmap ApplyYUVAdjustment(Bitmap source, double yS, double uS, double vS)
        {
            Bitmap bmp = new Bitmap(source);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* ptr = (byte*)data.Scan0;
            int bytes = data.Stride * bmp.Height;
            for (int i = 0; i < bytes; i += 4)
            {
                var yuv = YuvConverter.FromRgb(Color.FromArgb(ptr[i + 2], ptr[i + 1], ptr[i]));
                var nc = YuvConverter.ToRgb(yuv.Y + yS, yuv.U + uS, yuv.V + vS);
                ptr[i] = nc.B; ptr[i + 1] = nc.G; ptr[i + 2] = nc.R;
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        public static unsafe Bitmap ApplyYCbCrAdjustment(Bitmap source, double yS, double cbS, double crS)
        {
            Bitmap bmp = new Bitmap(source);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            byte* ptr = (byte*)data.Scan0;
            int bytes = data.Stride * bmp.Height;
            for (int i = 0; i < bytes; i += 4)
            {
                var ycbcr = YcbcrConverter.FromRgb(Color.FromArgb(ptr[i + 2], ptr[i + 1], ptr[i]));
                var nc = YcbcrConverter.ToRgb(ycbcr.Y + yS, ycbcr.Cb + cbS, ycbcr.Cr + crS);
                ptr[i] = nc.B; ptr[i + 1] = nc.G; ptr[i + 2] = nc.R;
            }
            bmp.UnlockBits(data);
            return bmp;
        }
    }
}