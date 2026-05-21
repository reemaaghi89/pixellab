using System;
using System.Drawing;
using System.Windows.Forms; // مشان يتعرف على MouseEventArgs
using pixellab.Converters;

namespace pixellab
{
    public static class ColorAnalyzer
    {
        // تابع خارجي شامل: يحسب الإحداثيات، يحدد البكسل، ويولد التقرير الشامل
        public static (Color? SelectedColor, string Report) AnalyzeClickedPixel(
            Bitmap currentImage, 
            int pictureBoxWidth, 
            int pictureBoxHeight, 
            MouseEventArgs mouseArgs)
        {
            // 1. حساب الأبعاد والنسب الهندسية للـ Zoom
            int imgWidth = currentImage.Width;
            int imgHeight = currentImage.Height;

            float ratioX = (float)pictureBoxWidth / imgWidth;
            float ratioY = (float)pictureBoxHeight / imgHeight;
            float ratio = Math.Min(ratioX, ratioY);

            float itemWidth = imgWidth * ratio;
            float itemHeight = imgHeight * ratio;

            float leftMargin = (pictureBoxWidth - itemWidth) / 2;
            float topMargin = (pictureBoxHeight - itemHeight) / 2;

            // 2. تحويل إحداثيات النقرة الفأرية إلى رقم البكسل الحقيقي داخل الصورة
            int pixelX = (int)((mouseArgs.X - leftMargin) / ratio);
            int pixelY = (int)((mouseArgs.Y - topMargin) / ratio);

            // 3. التأكد من أن النقرة وقعت داخل حدود الصورة الفعلية
            if (pixelX >= 0 && pixelX < imgWidth && pixelY >= 0 && pixelY < imgHeight)
            {
                // جلب لون البكسل الحقيقي مباشرة
                Color pixelColor = currentImage.GetPixel(pixelX, pixelY);

                // استدعاء محولات فضاءات الألوان الخاصة بكِ
                var (h, s, v) = HsvConverter.FromRgb(pixelColor);
                var (cyan, magenta, yellow, black) = CmykConverter.FromRgb(pixelColor);
                var (y_u, u, v_u) = YuvConverter.FromRgb(pixelColor);
                var (y_cb, cb, cr) = YcbcrConverter.FromRgb(pixelColor);
                var (labL, labA, labB) = LabConverter.FromRgb(pixelColor);

                // بناء السلسلة النصية المنسقة للتقرير الشامل
                string reportText = "🎨 نتائج تحليل اللون ومزامنة الفضاءات:\n" +
                                    "━━━━━━━━━━━━━━━━━━━━━\n" +
                                    $"📍 الإحداثيات الحقيقية: X: {pixelX}, Y: {pixelY}\n\n" +
                                    $"🔹 RGB:\n   R = {pixelColor.R}, G = {pixelColor.G}, B = {pixelColor.B}\n\n" +
                                    $"🔹 HSV:\n   H = {h:0}°, S = {s * 100:0}%, V = {v * 100:0}%\n\n" +
                                    $"🔹 CMYK (نظام الطباعة):\n   C = {cyan * 100:0}%, M = {magenta * 100:0}%, Y = {yellow * 100:0}%, K = {black * 100:0}%\n\n" +
                                    $"🔹 LAB (فضاء إدراك العين):\n   L = {labL:0.0}, A = {labA:0.0}, B = {labB:0.0}\n\n" +
                                    $"🔹 YUV (بث تماثلي):\n   Y = {y_u:0.0}, U = {u:0.0}, V = {v_u:0.0}\n\n" +
                                    $"🔹 YCbCr (المعيار الرقمي):\n   Y = {y_cb:0.0}, Cb = {cb:0.0}, Cr = {cr:0.0}";

                return (pixelColor, reportText);
            }

            // في حال نقر المستخدم خارج الصورة (بالهامش الرمادي) نرجع قيم فارغة
            return (null, string.Empty);
        }
    }
}