using System;
using System.Drawing;
using System.Drawing.Imaging;
using pixellab.Converters;
namespace pixellab
{
    public static class ImageProcessor
    {
        // تحويل الصورة إلى تدرجات الرمادي (Grayscale) بأعلى أداء برميجي
        public static Bitmap ConvertToGrayscale(Bitmap sourceImage)
        {
            // إنشاء نسخة مخصصة للمعالجة لعدم اللعب بالصورة الأصلية
            Bitmap processedBitmap = new Bitmap(sourceImage);
            
            // تحديد أبعاد الصورة بالكامل لقفل الذاكرة
            Rectangle imageBounds = new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height);

            // قفل بكسلات الصورة في الذاكرة العشوائية (RAM) بشكل مباشر لقراءتها والتعديل عليها
            BitmapData bitmapData = processedBitmap.LockBits(imageBounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            // استخدام البلوك غير الآمن (unsafe) للتعامل مع المؤشرات الفيزيائية للبكسلات
            unsafe
            {
                // مؤشر بايت يشير إلى بداية مصفوفة بكسلات الصورة في الذاكرة
                byte* pixelPointer = (byte*)bitmapData.Scan0;

                // حساب إجمالي عدد البايتات المحجوزة للصورة في الذاكرة
                int totalBytesCount = bitmapData.Stride * processedBitmap.Height;

                // المرور على المصفوفة بقفزات بمقدار 4 بايت (لأن كل بكسل يتكون من 4 قنوات: B, G, R, Alpha)
                for (int byteIndex = 0; byteIndex < totalBytesCount; byteIndex += 4)
                {
                    // قراءة قيم القنوات اللونية للبكسل الحالي من الذاكرة (الترتيب الفيزيائي هو BGR)
                    byte blueChannel  = pixelPointer[byteIndex];
                    byte greenChannel = pixelPointer[byteIndex + 1];
                    byte redChannel   = pixelPointer[byteIndex + 2];

                    // حساب القيمة الرمادية باستخدام المعايير الأكاديمية الدقيقة لتوافق العين البشرية
                    byte grayscaleValue = (byte)(0.299 * redChannel + 0.587 * greenChannel + 0.114 * blueChannel);

                    // حقن القيمة الرمادية الجديدة داخل القنوات الثلاثة ليصبح اللون رمادياً
                    pixelPointer[byteIndex]     = grayscaleValue; // تعديل قناة الأزرق
                    pixelPointer[byteIndex + 1] = grayscaleValue; // تعديل قناة الأخضر
                    pixelPointer[byteIndex + 2] = grayscaleValue; // تعديل قناة الأحمر
                    
                    // ملاحظة: pixelPointer[byteIndex + 3] يمثل الشفافية (Alpha) نتركه كما هو دون تغيير
                }
            }

            // فك قفل الصورة من الذاكرة فوراً لتجنب حدوث تسريب للذاكرة (Memory Leak)
            processedBitmap.UnlockBits(bitmapData);

            return processedBitmap;
        }

        public static Bitmap ConvertToBlackAndWhite(Bitmap sourceImage, byte thresholdValue = 128)
        {
            // إنشاء نسخة مخصصة للمعالجة لعدم التعديل على الأصل
            Bitmap processedBitmap = new Bitmap(sourceImage);
            
            // تحديد أبعاد الصورة بالكامل لقفل الذاكرة
            Rectangle imageBounds = new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height);

            // قفل البكسلات في الذاكرة العشوائية (RAM) بشكل مباشر لقراءتها والتعديل عليها
            BitmapData bitmapData = processedBitmap.LockBits(imageBounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            // استخدام البلوك غير الآمن (unsafe) للتعامل مع المؤشرات الفيزيائية للبكسلات
            unsafe
            {
                byte* pixelPointer = (byte*)bitmapData.Scan0;
                int totalBytesCount = bitmapData.Stride * processedBitmap.Height;

                // المرور على مصفوفة البكسلات بقفزات بمقدار 4 بايت
                for (int byteIndex = 0; byteIndex < totalBytesCount; byteIndex += 4)
                {
                    // قراءة قيم القنوات اللونية للبكسل الحالي (BGR)
                    byte blueChannel  = pixelPointer[byteIndex];
                    byte greenChannel = pixelPointer[byteIndex + 1];
                    byte redChannel   = pixelPointer[byteIndex + 2];

                    // أولاً: نحسب القيمة الرمادية للبكسل لنعرف مدى شدة إضاءته
                    byte intensity = (byte)(0.299 * redChannel + 0.587 * greenChannel + 0.114 * blueChannel);

                    // ثانياً: مقارنة شدة الإضاءة مع قيمة العتبة (Threshold)
                    // إذا كان البكسل أفتح من العتبة يصير أبيض (255)، وإذا أغمق يصير أسود (0)
                    byte finalColor = (intensity >= thresholdValue) ? (byte)255 : (byte)0;

                    // حقن اللون الجديد (أبيض أو أسود) في القنوات الثلاثة
                    pixelPointer[byteIndex]     = finalColor; // Blue
                    pixelPointer[byteIndex + 1] = finalColor; // Green
                    pixelPointer[byteIndex + 2] = finalColor; // Red
                }
            }

            // فك قفل الصورة من الذاكرة فوراً
            processedBitmap.UnlockBits(bitmapData);

            return processedBitmap;
        }

        
        // تقليل عدد الألوان وتكميم الصورة (Color Quantization / Posterization) باستخدام LockBits
        public static Bitmap QuantizeImageColors(Bitmap sourceImage, int colorLevelsCount = 4)
        {
            // إنشاء نسخة مخصصة للمعالجة لعدم التعديل على الأصل
            Bitmap processedBitmap = new Bitmap(sourceImage);
            
            // تحديد أبعاد الصورة بالكامل لقفل الذاكرة
            Rectangle imageBounds = new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height);

            // حساب طول الخطوة اللونية بناءً على عدد المستويات المطلوبة (الافتراضي 4 مستويات)
            int quantizationStep = 256 / colorLevelsCount;

            // قفل البكسلات في الذاكرة العشوائية (RAM) بشكل مباشر لقراءتها والتعديل عليها
            BitmapData bitmapData = processedBitmap.LockBits(imageBounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            // استخدام البلوك غير الآمن (unsafe) للتعامل مع المؤشرات الفيزيائية
            unsafe
            {
                byte* pixelPointer = (byte*)bitmapData.Scan0;
                int totalBytesCount = bitmapData.Stride * processedBitmap.Height;

                // المرور على مصفوفة بكسلات الصورة بالكامل
                for (int byteIndex = 0; byteIndex < totalBytesCount; byteIndex += 4)
                {
                    // قراءة قيم القنوات اللونية الحالية للبكسل (BGR)
                    byte originalBlue  = pixelPointer[byteIndex];
                    byte originalGreen = pixelPointer[byteIndex + 1];
                    byte originalRed   = pixelPointer[byteIndex + 2];

                    // تطبيق معادلة التكميم وتقليل الألوان عبر القسمة الصحيحة ثم الضرب بالخطوة
                    byte quantizedBlue  = (byte)((originalBlue / quantizationStep) * quantizationStep);
                    byte quantizedGreen = (byte)((originalGreen / quantizationStep) * quantizationStep);
                    byte quantizedRed   = (byte)((originalRed / quantizationStep) * quantizationStep);

                    // حقن قيم الألوان المكممة والجديدة داخل الذاكرة مباشرة
                    pixelPointer[byteIndex]     = quantizedBlue;  // تعديل الأزرق
                    pixelPointer[byteIndex + 1] = quantizedGreen; // تعديل الأخضر
                    pixelPointer[byteIndex + 2] = quantizedRed;   // تعديل الأحمر
                }
            }

            // فك قفل الصورة من الذاكرة فوراً
            processedBitmap.UnlockBits(bitmapData);

            return processedBitmap;
        }

        public static Bitmap ApplyRGBFast(
    Bitmap sourceImage,
    int redShift,
    int greenShift,
    int blueShift)
{
    Bitmap processedBitmap =
        new Bitmap(sourceImage);

    Rectangle imageBounds =
        new Rectangle(
            0,
            0,
            processedBitmap.Width,
            processedBitmap.Height);

    BitmapData bitmapData =
        processedBitmap.LockBits(
            imageBounds,
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppRgb);

    unsafe
    {
        byte* pixelPointer =
            (byte*)bitmapData.Scan0;

        int totalBytesCount =
            bitmapData.Stride *
            processedBitmap.Height;

        for (int byteIndex = 0;
            byteIndex < totalBytesCount;
            byteIndex += 4)
        {
            int blue =
                pixelPointer[byteIndex] +
                blueShift;

            int green =
                pixelPointer[byteIndex + 1] +
                greenShift;

            int red =
                pixelPointer[byteIndex + 2] +
                redShift;

            pixelPointer[byteIndex] =
                (byte)Math.Max(
                    0,
                    Math.Min(255, blue));

            pixelPointer[byteIndex + 1] =
                (byte)Math.Max(
                    0,
                    Math.Min(255, green));

            pixelPointer[byteIndex + 2] =
                (byte)Math.Max(
                    0,
                    Math.Min(255, red));
        }
    }

    processedBitmap.UnlockBits(bitmapData);

    return processedBitmap;
}

        public static Bitmap ApplyHSVAdjustment(
    Bitmap sourceImage,
    double hueShift,
    double saturationShift,
    double valueShift)
{
    Bitmap processedBitmap =
        new Bitmap(sourceImage);

    Rectangle imageBounds =
        new Rectangle(
            0,
            0,
            processedBitmap.Width,
            processedBitmap.Height);

    BitmapData bitmapData =
        processedBitmap.LockBits(
            imageBounds,
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppRgb);

    unsafe
    {
        byte* pixelPointer =
            (byte*)bitmapData.Scan0;

        int totalBytesCount =
            bitmapData.Stride *
            processedBitmap.Height;

        for (int byteIndex = 0;
            byteIndex < totalBytesCount;
            byteIndex += 4)
        {
            Color originalColor =
                Color.FromArgb(
                    pixelPointer[byteIndex + 2],
                    pixelPointer[byteIndex + 1],
                    pixelPointer[byteIndex]);

            var hsv =
                HsvConverter.FromRgb(originalColor);

            double h =
                (hsv.Hue + hueShift) % 360;

            if (h < 0)
                h += 360;

            double s =
                hsv.Saturation +
                saturationShift;

            double v =
                hsv.Value +
                valueShift;

            s = Math.Max(0,
                Math.Min(1, s));

            v = Math.Max(0,
                Math.Min(1, v));

            Color newColor =
                HsvConverter.ToRgb(h, s, v);

            pixelPointer[byteIndex] =
                newColor.B;

            pixelPointer[byteIndex + 1] =
                newColor.G;

            pixelPointer[byteIndex + 2] =
                newColor.R;
        }
    }

    processedBitmap.UnlockBits(bitmapData);

    return processedBitmap;
}

    public static Bitmap ApplyCMYKAdjustment(
    Bitmap sourceImage,
    double cyanShift,
    double magentaShift,
    double yellowShift,
    double blackShift)
{
    Bitmap processedBitmap =
        new Bitmap(sourceImage);

    Rectangle imageBounds =
        new Rectangle(
            0,
            0,
            processedBitmap.Width,
            processedBitmap.Height);

    BitmapData bitmapData =
        processedBitmap.LockBits(
            imageBounds,
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppRgb);

    unsafe
    {
        byte* pixelPointer =
            (byte*)bitmapData.Scan0;

        int totalBytesCount =
            bitmapData.Stride *
            processedBitmap.Height;

        for (int byteIndex = 0;
            byteIndex < totalBytesCount;
            byteIndex += 4)
        {
            Color originalColor =
                Color.FromArgb(
                    pixelPointer[byteIndex + 2],
                    pixelPointer[byteIndex + 1],
                    pixelPointer[byteIndex]);

            var cmyk =
                CmykConverter.FromRgb(
                    originalColor);

            double c =
                cmyk.C + cyanShift;

            double m =
                cmyk.M + magentaShift;

            double y =
                cmyk.Y + yellowShift;

            double k =
                cmyk.K + blackShift;

            c = Math.Max(0,
                Math.Min(1, c));

            m = Math.Max(0,
                Math.Min(1, m));

            y = Math.Max(0,
                Math.Min(1, y));

            k = Math.Max(0,
                Math.Min(1, k));

            Color newColor =
                CmykConverter.ToRgb(
                    c,
                    m,
                    y,
                    k);

            pixelPointer[byteIndex] =
                newColor.B;

            pixelPointer[byteIndex + 1] =
                newColor.G;

            pixelPointer[byteIndex + 2] =
                newColor.R;
        }
    }

    processedBitmap.UnlockBits(bitmapData);

    return processedBitmap;
}

public static Bitmap ApplyLABAdjustment(
    Bitmap sourceImage,
    double lShift,
    double aShift,
    double bShift)
{
    Bitmap processedBitmap =
        new Bitmap(sourceImage);

    Rectangle imageBounds =
        new Rectangle(
            0,
            0,
            processedBitmap.Width,
            processedBitmap.Height);

    BitmapData bitmapData =
        processedBitmap.LockBits(
            imageBounds,
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppRgb);

    unsafe
    {
        byte* pixelPointer =
            (byte*)bitmapData.Scan0;

        int totalBytesCount =
            bitmapData.Stride *
            processedBitmap.Height;

        for (int byteIndex = 0;
            byteIndex < totalBytesCount;
            byteIndex += 4)
        {
            Color originalColor =
                Color.FromArgb(
                    pixelPointer[byteIndex + 2],
                    pixelPointer[byteIndex + 1],
                    pixelPointer[byteIndex]);

            var lab =
                LabConverter.FromRgb(
                    originalColor);

            double l =
                lab.L + lShift;

            double a =
                lab.A + aShift;

            double b =
                lab.B + bShift;

            // clamp
            l = Math.Max(0,
                Math.Min(100, l));

            a = Math.Max(-128,
                Math.Min(127, a));

            b = Math.Max(-128,
                Math.Min(127, b));

            Color newColor =
                LabConverter.ToRgb(
                    l,
                    a,
                    b);

            pixelPointer[byteIndex] =
                newColor.B;

            pixelPointer[byteIndex + 1] =
                newColor.G;

            pixelPointer[byteIndex + 2] =
                newColor.R;
        }
    }

    processedBitmap.UnlockBits(bitmapData);

    return processedBitmap;
}

public static Bitmap ApplyYUVAdjustment(
    Bitmap sourceImage,
    double yShift,
    double uShift,
    double vShift)
{
    Bitmap processedBitmap =
        new Bitmap(sourceImage);

    Rectangle imageBounds =
        new Rectangle(
            0,
            0,
            processedBitmap.Width,
            processedBitmap.Height);

    BitmapData bitmapData =
        processedBitmap.LockBits(
            imageBounds,
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppRgb);

    unsafe
    {
        byte* pixelPointer =
            (byte*)bitmapData.Scan0;

        int totalBytesCount =
            bitmapData.Stride *
            processedBitmap.Height;

        for (int byteIndex = 0;
            byteIndex < totalBytesCount;
            byteIndex += 4)
        {
            Color originalColor =
                Color.FromArgb(
                    pixelPointer[byteIndex + 2],
                    pixelPointer[byteIndex + 1],
                    pixelPointer[byteIndex]);

            var yuv =
                YuvConverter.FromRgb(
                    originalColor);

            double y =
                yuv.Y + yShift;

            double u =
                yuv.U + uShift;

            double v =
                yuv.V + vShift;

            Color newColor =
                YuvConverter.ToRgb(
                    y,
                    u,
                    v);

            pixelPointer[byteIndex] =
                newColor.B;

            pixelPointer[byteIndex + 1] =
                newColor.G;

            pixelPointer[byteIndex + 2] =
                newColor.R;
        }
    }

    processedBitmap.UnlockBits(bitmapData);

    return processedBitmap;
}
public static Bitmap ApplyYCbCrAdjustment(
    Bitmap sourceImage,
    double yShift,
    double cbShift,
    double crShift)
{
    Bitmap processedBitmap =
        new Bitmap(sourceImage);

    Rectangle imageBounds =
        new Rectangle(
            0,
            0,
            processedBitmap.Width,
            processedBitmap.Height);

    BitmapData bitmapData =
        processedBitmap.LockBits(
            imageBounds,
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppRgb);

    unsafe
    {
        byte* pixelPointer =
            (byte*)bitmapData.Scan0;

        int totalBytesCount =
            bitmapData.Stride *
            processedBitmap.Height;

        for (int byteIndex = 0;
            byteIndex < totalBytesCount;
            byteIndex += 4)
        {
            Color originalColor =
                Color.FromArgb(
                    pixelPointer[byteIndex + 2],
                    pixelPointer[byteIndex + 1],
                    pixelPointer[byteIndex]);

            var ycbcr =
                YcbcrConverter.FromRgb(
                    originalColor);

            double y =
                ycbcr.Y + yShift;

            double cb =
                ycbcr.Cb + cbShift;

            double cr =
                ycbcr.Cr + crShift;

            Color newColor =
                YcbcrConverter.ToRgb(
                    y,
                    cb,
                    cr);

            pixelPointer[byteIndex] =
                newColor.B;

            pixelPointer[byteIndex + 1] =
                newColor.G;

            pixelPointer[byteIndex + 2] =
                newColor.R;
        }
    }

    processedBitmap.UnlockBits(bitmapData);

    return processedBitmap;
}
    }
}