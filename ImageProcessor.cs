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
            Bitmap bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppRgb);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;
            int bytes = srcData.Stride * source.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                byte gray = (byte)(0.299 * srcPtr[i + 2] + 0.587 * srcPtr[i + 1] + 0.114 * srcPtr[i]);
                dstPtr[i] = dstPtr[i + 1] = dstPtr[i + 2] = gray;
            }

            source.UnlockBits(srcData);
            bmp.UnlockBits(dstData);
            return bmp;
        }

        public static unsafe Bitmap ConvertToBlackAndWhite(Bitmap source, byte threshold = 128)
        {
            Bitmap bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppRgb);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;
            int bytes = srcData.Stride * source.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                byte intensity = (byte)(0.299 * srcPtr[i + 2] + 0.587 * srcPtr[i + 1] + 0.114 * srcPtr[i]);
                byte val = (intensity >= threshold) ? (byte)255 : (byte)0;
                dstPtr[i] = dstPtr[i + 1] = dstPtr[i + 2] = val;
            }

            source.UnlockBits(srcData);
            bmp.UnlockBits(dstData);
            return bmp;
        }

        public static unsafe Bitmap QuantizeImageColors(Bitmap source, int levels)
        {
            if (levels < 2) levels = 2;
            if (levels > 256) levels = 256;

            Bitmap bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppRgb);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            
            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;
            int bytes = srcData.Stride * source.Height;
            
            float step = 255.0f / (levels - 1);

            for (int i = 0; i < bytes; i += 4)
            {
                for (int channel = 0; channel < 3; channel++)
                {
                    float val = srcPtr[i + channel];
                    int quantizedValue = (int)Math.Round(val / step) * (int)step;
                    dstPtr[i + channel] = (byte)Math.Clamp(quantizedValue, 0, 255);
                }
            }
            
            source.UnlockBits(srcData);
            bmp.UnlockBits(dstData);
            return bmp;
        }
        public static unsafe Bitmap QuantizeImageColorsIndexed(Bitmap source, int levels)
        {
            if (levels < 2) levels = 2;
            if (levels > 256) levels = 256;

            int width = source.Width;
            int height = source.Height;

            Bitmap indexedBmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            ColorPalette palette = indexedBmp.Palette;
            float step = 255.0f / (levels - 1);
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                int r = (int)Math.Round(((i >> 5) & 7) * 255.0 / 7.0);
                int g = (int)Math.Round(((i >> 2) & 7) * 255.0 / 7.0);
                int b = (int)Math.Round((i & 3) * 255.0 / 3.0);

                r = (int)Math.Round(r / step) * (int)step;
                g = (int)Math.Round(g / step) * (int)step;
                b = (int)Math.Round(b / step) * (int)step;

                palette.Entries[i] = Color.FromArgb(
                    Math.Clamp(r, 0, 255),
                    Math.Clamp(g, 0, 255),
                    Math.Clamp(b, 0, 255)
                );
            }
            indexedBmp.Palette = palette;

            BitmapData srcData = source.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = indexedBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;

            int srcStride = srcData.Stride;
            int dstStride = dstData.Stride;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = (y * srcStride) + (x * 4);
                    
                    byte b = srcPtr[srcIndex];
                    byte g = srcPtr[srcIndex + 1];
                    byte r = srcPtr[srcIndex + 2];

                    int quantizedR = (int)Math.Round(r / step) * (int)step;
                    int quantizedG = (int)Math.Round(g / step) * (int)step;
                    int quantizedB = (int)Math.Round(b / step) * (int)step;
                    byte colorIndex = (byte)((((quantizedR * 7 / 255) & 7) << 5) |
                                            (((quantizedG * 7 / 255) & 7) << 2) |
                                            ((quantizedB * 3 / 255) & 3));

                    int dstIndex = (y * dstStride) + x;
                    dstPtr[dstIndex] = colorIndex;
                }
            }

            source.UnlockBits(srcData);
            indexedBmp.UnlockBits(dstData);

            return indexedBmp;
        }
        public static unsafe Bitmap ApplyRGBFast(Bitmap source, int r, int g, int b)
        {
            Bitmap bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppRgb);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;
            int bytes = srcData.Stride * source.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                dstPtr[i + 2] = (byte)Math.Clamp(srcPtr[i + 2] + r, 0, 255);
                dstPtr[i + 1] = (byte)Math.Clamp(srcPtr[i + 1] + g, 0, 255);
                dstPtr[i]     = (byte)Math.Clamp(srcPtr[i] + b, 0, 255);
            }

            source.UnlockBits(srcData);
            bmp.UnlockBits(dstData);
            return bmp;
        }

        public static unsafe Bitmap ApplyHSVAdjustment(Bitmap source, double hS, double sS, double vS)
        {
            Bitmap bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppRgb);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;
            int bytes = srcData.Stride * source.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                var hsv = HsvConverter.FromRgb(Color.FromArgb(srcPtr[i + 2], srcPtr[i + 1], srcPtr[i]));
                double h = (hsv.Hue + hS) % 360; if (h < 0) h += 360;
                var nc = HsvConverter.ToRgb(h, Math.Clamp(hsv.Saturation + sS, 0, 1), Math.Clamp(hsv.Value + vS, 0, 1));
                dstPtr[i] = nc.B; dstPtr[i + 1] = nc.G; dstPtr[i + 2] = nc.R;
            }

            source.UnlockBits(srcData);
            bmp.UnlockBits(dstData);
            return bmp;
        }

        public static unsafe Bitmap ApplyCMYKAdjustment(Bitmap source, double cS, double mS, double yS, double kS)
        {
            Bitmap bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppRgb);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;
            int bytes = srcData.Stride * source.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                var cmyk = CmykConverter.FromRgb(Color.FromArgb(srcPtr[i + 2], srcPtr[i + 1], srcPtr[i]));
                var nc = CmykConverter.ToRgb(Math.Clamp(cmyk.C + cS, 0, 1), Math.Clamp(cmyk.M + mS, 0, 1), Math.Clamp(cmyk.Y + yS, 0, 1), Math.Clamp(cmyk.K + kS, 0, 1));
                dstPtr[i] = nc.B; dstPtr[i + 1] = nc.G; dstPtr[i + 2] = nc.R;
            }

            source.UnlockBits(srcData);
            bmp.UnlockBits(dstData);
            return bmp;
        }

        public static unsafe Bitmap ApplyLABAdjustment(Bitmap source, double lS, double aS, double bS)
        {
            Bitmap bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppRgb);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;
            int bytes = srcData.Stride * source.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                var lab = LabConverter.FromRgb(Color.FromArgb(srcPtr[i + 2], srcPtr[i + 1], srcPtr[i]));
                // ضبط الحدود الرياضية الصارمة لقيم الـ Lab لمنع التخبيص
                var nc = LabConverter.ToRgb(Math.Clamp(lab.L + lS, 0, 100), Math.Clamp(lab.A + aS, -128, 127), Math.Clamp(lab.B + bS, -128, 127));
                dstPtr[i] = nc.B; dstPtr[i + 1] = nc.G; dstPtr[i + 2] = nc.R;
            }

            source.UnlockBits(srcData);
            bmp.UnlockBits(dstData);
            return bmp;
        }

        public static unsafe Bitmap ApplyYUVAdjustment(Bitmap source, double yS, double uS, double vS)
        {
            Bitmap bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppRgb);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;
            int bytes = srcData.Stride * source.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                var yuv = YuvConverter.FromRgb(Color.FromArgb(srcPtr[i + 2], srcPtr[i + 1], srcPtr[i]));
                var nc = YuvConverter.ToRgb(Math.Clamp(yuv.Y + yS, 0, 1), Math.Clamp(yuv.U + uS, -0.436, 0.436), Math.Clamp(yuv.V + vS, -0.615, 0.615));
                dstPtr[i] = nc.B; dstPtr[i + 1] = nc.G; dstPtr[i + 2] = nc.R;
            }

            source.UnlockBits(srcData);
            bmp.UnlockBits(dstData);
            return bmp;
        }

        public static unsafe Bitmap ApplyYCbCrAdjustment(Bitmap source, double yS, double cbS, double crS)
        {
            Bitmap bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppRgb);
            BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData dstData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;
            int bytes = srcData.Stride * source.Height;

            for (int i = 0; i < bytes; i += 4)
            {
                var ycbcr = YcbcrConverter.FromRgb(Color.FromArgb(srcPtr[i + 2], srcPtr[i + 1], srcPtr[i]));
                var nc = YcbcrConverter.ToRgb(Math.Clamp(ycbcr.Y + yS, 0, 255), Math.Clamp(ycbcr.Cb + cbS, 0, 255), Math.Clamp(ycbcr.Cr + crS, 0, 255));
                dstPtr[i] = nc.B; dstPtr[i + 1] = nc.G; dstPtr[i + 2] = nc.R;
            }

            source.UnlockBits(srcData);
            bmp.UnlockBits(dstData);
            return bmp;
        }
    }
}