using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace pixellab.Renderers
{
    public class LabColor
    {
        public double L { get; set; }
        public double A { get; set; }
        public double B { get; set; }

        public LabColor(double l, double a, double b)
        {
            L = l;
            A = a;
            B = b;
        }
    }

    public struct LabMeshFace
    {
        public PointF P1, P2, P3, P4;
        public Color FaceColor;
        public double AvgDepth;
    }

    public static class LabSpaceRenderer
    {
        private static (PointF Point, double ZDepth) ProjectWithDepth(double x, double y, double z, int cx, int cy, double angleX, double angleY, float zoom)
        {
            double radX = angleX * Math.PI / 180.0;
            double radY = angleY * Math.PI / 180.0;

            double cosX = Math.Cos(radX), sinX = Math.Sin(radX);
            double cosY = Math.Cos(radY), sinY = Math.Sin(radY);

            // تدوير فضاء ثلاثي الأبعاد
            double rY = y * cosX - z * sinX;
            double rZ = y * sinX + z * cosX;
            double rX = x * cosY + rZ * sinY;
            double depthZ = -x * sinY + rZ * cosY;

            // تحجيم وتكبير متناسق لإبراز التواء المجسم وشطحاته الجانبية الأكاديمية
            float screenX = (float)(cx + rX * 2.8 * zoom);
            float screenY = (float)(cy - rY * 2.8 * zoom);

            return (new PointF(screenX, screenY), depthZ);
        }

        public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom, Color selectedColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = width / 2;
            int cy = height / 2;

            // 🌟 استراتيجية بناء القشرة الخارجية لمكعب RGB داخل فضاء Lab ليعطي الالتواء والانحناء الحقيقي مية بالمية
            int steps = 15; // دقة التقسيم الشبكي للمجسم
            List<LabMeshFace> meshFaces = new List<LabMeshFace>();

            // دالة مساعدة لتوليد الأوجه الخارجية فقط عبر مسح الأسطح الستة لمكعب الألوان
            Action<int, int, int, int, int, int> buildFace = (rStart, rEnd, gStart, gEnd, bStart, bEnd) =>
            {
                for (int i = 0; i < steps; i++)
                {
                    for (int j = 0; j < steps; j++)
                    {
                        // حساب إحداثيات الزوايا الأربعة لكل مربع صغير على القشرة الخارجية
                        Color c1 = GetMeshColor(i, j, steps, rStart, rEnd, gStart, gEnd, bStart, bEnd);
                        Color c2 = GetMeshColor(i + 1, j, steps, rStart, rEnd, gStart, gEnd, bStart, bEnd);
                        Color c3 = GetMeshColor(i + 1, j + 1, steps, rStart, rEnd, gStart, gEnd, bStart, bEnd);
                        Color c4 = GetMeshColor(i, j + 1, steps, rStart, rEnd, gStart, gEnd, bStart, bEnd);

                        var lab1 = RgbToLabInternal(c1);
                        var lab2 = RgbToLabInternal(c2);
                        var lab3 = RgbToLabInternal(c3);
                        var lab4 = RgbToLabInternal(c4);

                        // إسقاط النقاط هندسياً (X=a, Y=b, Z=L) متمحوراً حول المنتصف
                        var p1 = ProjectWithDepth(lab1.A, lab1.B, lab1.L - 50.0, cx, cy, angleX, angleY, zoom);
                        var p2 = ProjectWithDepth(lab2.A, lab2.B, lab2.L - 50.0, cx, cy, angleX, angleY, zoom);
                        var p3 = ProjectWithDepth(lab3.A, lab3.B, lab3.L - 50.0, cx, cy, angleX, angleY, zoom);
                        var p4 = ProjectWithDepth(lab4.A, lab4.B, lab4.L - 50.0, cx, cy, angleX, angleY, zoom);

                        double avgDepth = (p1.ZDepth + p2.ZDepth + p3.ZDepth + p4.ZDepth) / 4.0;

                        meshFaces.Add(new LabMeshFace
                        {
                            P1 = p1.Point, P2 = p2.Point, P3 = p3.Point, P4 = p4.Point,
                            FaceColor = c1,
                            AvgDepth = avgDepth
                        });
                    }
                }
            };

            // بناء الأوجه الستة المحيطية للمكعب لتوليد الانحناء الحقيقي الفخم للاب
            buildFace(0, 255, 0, 255, 0, 0);     // السطح السفلي (الأسود والأزرق والأخضر)
            buildFace(0, 255, 0, 255, 255, 255); // السطح العلوي
            buildFace(0, 0, 0, 255, 0, 255);     // الجوانب المحيطية
            buildFace(255, 255, 0, 255, 0, 255);
            buildFace(0, 255, 0, 0, 0, 255);
            buildFace(0, 255, 255, 255, 0, 255);

            // ترتيب الأوجه بالعمق (Painter's Algorithm) لمنع أي تداخل بصري أثناء الدوران والتصفح
            meshFaces.Sort((f1, f2) => f1.AvgDepth.CompareTo(f2.AvgDepth));

            // رسم الأوجه المنحنية الحقيقية الملتفة
            foreach (var face in meshFaces)
            {
                PointF[] pts = { face.P1, face.P2, face.P3, face.P4 };
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(pts);
                    using (SolidBrush br = new SolidBrush(Color.FromArgb(235, face.FaceColor)))
                    {
                        g.FillPolygon(br, pts);
                    }
                }

                // خطوط الهيكل الشبكي الأكاديمي لإبراز التواء المجسم الفريد وميلانه
                using (Pen gridPen = new Pen(Color.FromArgb(25, Color.White), 1f))
                {
                    g.DrawPolygon(gridPen, pts);
                }
            }

            // 🌟 2. مزامنة موقع المؤشر اللحظي الذهبي بداخل مجسم الـ Lab الملتف الحقيقي
            LabColor myLab = RgbToLabInternal(selectedColor);
            double targetX = myLab.A;
            double targetY = myLab.B;
            double targetZ = myLab.L - 50.0;

            var targetProj = ProjectWithDepth(targetX, targetY, targetZ, cx, cy, angleX, angleY, zoom);
            PointF targetPt = targetProj.Point;

            using (Brush bBrush = new SolidBrush(selectedColor)) g.FillEllipse(bBrush, targetPt.X - 7, targetPt.Y - 7, 14, 14);
            using (Pen goldPen = new Pen(Color.Gold, 2f)) g.DrawEllipse(goldPen, targetPt.X - 9, targetPt.Y - 9, 18, 18);
        }

        private static Color GetMeshColor(int i, int j, int steps, int rS, int rE, int gS, int gE, int bS, int bE)
        {
            int r = rS + (rE - rS) * i / steps;
            int g = gS + (gE - gS) * j / steps;
            int b = bS + (bE - bS) * i / steps;
            
            if (rS == rE && gS != gE && bS != bE) b = bS + (bE - bS) * j / steps;
            if (gS == gE && rS != rE && bS != bE) b = bS + (bE - bS) * j / steps;

            return Color.FromArgb(Math.Max(0, Math.Min(255, r)), Math.Max(0, Math.Min(255, g)), Math.Max(0, Math.Min(255, b)));
        }

        public static Color GetColorAtPointDirect(Point mousePt, int width, int height, double angleX, double angleY, float zoom)
        {
            int cx = width / 2;
            int cy = height / 2;

            double minDistance = double.MaxValue;
            Color bestColor = Color.Transparent;

            // مسح ذكي وسريع لالتقاط الماوس المتزامن داخل المنحنى الحقيقي
            for (int r = 0; r <= 255; r += 12)
            {
                for (int gVal = 0; gVal <= 255; gVal += 12)
                {
                    for (int b = 0; b <= 255; b += 12)
                    {
                        Color c = Color.FromArgb(r, gVal, b);
                        var lab = RgbToLabInternal(c);

                        var proj = ProjectWithDepth(lab.A, lab.B, lab.L - 50.0, cx, cy, angleX, angleY, zoom);

                        double dx = mousePt.X - proj.Point.X;
                        double dy = mousePt.Y - proj.Point.Y;
                        double dist = dx * dx + dy * dy;

                        if (dist < minDistance && dist < 140)
                        {
                            minDistance = dist;
                            bestColor = c;
                        }
                    }
                }
            }
            return bestColor;
        }

        private static LabColor RgbToLabInternal(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
            g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
            b = (b > 0.04045) ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92;

            double x = (r * 0.4124 + g * 0.3576 + b * 0.1805) / 0.95047;
            double y = (r * 0.2126 + g * 0.7152 + b * 0.0722) / 1.00000;
            double z = (r * 0.0193 + g * 0.1192 + b * 0.9505) / 1.08883;

            x = (x > 0.008856) ? Math.Pow(x, 1.0 / 3.0) : (7.787 * x) + (16.0 / 116.0);
            y = (y > 0.008856) ? Math.Pow(y, 1.0 / 3.0) : (7.787 * y) + (16.0 / 116.0);
            z = (z > 0.008856) ? Math.Pow(z, 1.0 / 3.0) : (7.787 * z) + (16.0 / 116.0);

            double L = (116.0 * y) - 16.0;
            double A = 500.0 * (x - y);
            double B = 200.0 * (y - z);

            return new LabColor(L, A, B);
        }
    }
}