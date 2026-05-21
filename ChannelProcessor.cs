using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace pixellab
{
    public static class ChannelProcessor
    {
        // الفلتر الخارق لتعديل قيم وتعطيل قنوات RGB بالزمن الحقيقي باستخدام LockBits
        public static Bitmap AdjustRgbChannels(Bitmap sourceImage, 
            bool disableRed, int redValue, 
            bool disableGreen, int greenValue, 
            bool disableBlue, int blueValue)
        {
            Bitmap processedBitmap = new Bitmap(sourceImage);
            Rectangle imageBounds = new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height);

            BitmapData bitmapData = processedBitmap.LockBits(imageBounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            unsafe
            {
                byte* pixelPointer = (byte*)bitmapData.Scan0;
                int totalBytesCount = bitmapData.Stride * processedBitmap.Height;

                for (int byteIndex = 0; byteIndex < totalBytesCount; byteIndex += 4)
                {
                    // الحسابات لقناة الأزرق (Blue)
                    int finalBlue = disableBlue ? 0 : pixelPointer[byteIndex] + blueValue;
                    pixelPointer[byteIndex] = (byte)Math.Max(0, Math.Min(255, finalBlue));

                    // الحسابات لقناة الأخضر (Green)
                    int finalGreen = disableGreen ? 0 : pixelPointer[byteIndex + 1] + greenValue;
                    pixelPointer[byteIndex + 1] = (byte)Math.Max(0, Math.Min(255, finalGreen));

                    // الحسابات لقناة الأحمر (Red)
                    int finalRed = disableRed ? 0 : pixelPointer[byteIndex + 2] + redValue;
                    pixelPointer[byteIndex + 2] = (byte)Math.Max(0, Math.Min(255, finalRed));
                }
            }

            processedBitmap.UnlockBits(bitmapData);
            return processedBitmap;
        }
    }
}