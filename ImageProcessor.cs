using System;
using System.Drawing;
using System.Drawing.Imaging;

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
    }
}