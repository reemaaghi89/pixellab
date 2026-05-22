using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using pixellab.Converters;

namespace pixellab.Renderers
{
    public static class LabRenderer
    {
        private const int L_Steps = 15;   // زيادة الخطوات لنعومة أكثر
        private const int Segments = 30;  // دقة دائرية أعلى

        // كلاس داخلي صغير لحفظ بيانات الوجه وعمقه قبل الرسم
        private class LabFace
        {
            public PointF[] Points { get; set; }
            public Color FaceColor { get; set; }
            public double AverageDepth { get; set; }

            public (double L, double a, double b)[] LabVertices { get; set; }
        }

        // دالة إسقاط محدثة ترجع النقطة ثنائية الأبعاد مع عمقها الـ Z
        private static (PointF Point, double ZDepth) ProjectWithDepth(double x, double y, double z, int cx, int cy, double angleX, double angleY, float zoom)
        {
            double radX = angleX * Math.PI / 180.0; 
            double radY = angleY * Math.PI / 180.0;
            
            double cosX = Math.Cos(radX), sinX = Math.Sin(radX);
            double cosY = Math.Cos(radY), sinY = Math.Sin(radY);
            
            double rY = y * cosX - z * sinX;
            double rZ = y * sinX + z * cosX;
            double rX = x * cosY + rZ * sinY;
            
            // حساب العمق الفعلي للنقطة بالنسبة لعين المشاهد
            double depthZ = -x * sinY + rZ * cosY; 

            return (new PointF((float)(cx + rX * zoom), (float)(cy - rY * zoom)), depthZ);
        }

        public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom, Color selectedColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int cx = width / 2;
            int cy = height / 2;

            // قائمة لتجميع كافة أوجه المغزل قبل رندرتها
            List<LabFace> facesList = new List<LabFace>();

            for (int i = 0; i < L_Steps; i++)
            {
                for (int j = 0; j < Segments; j++)
                {
                    int nextJ = (j + 1) % Segments;
                    
                    var l1 = GetLabAt(i, j);
                    var l2 = GetLabAt(i, nextJ);
                    var l3 = GetLabAt(i + 1, nextJ);
                    var l4 = GetLabAt(i + 1, j);

                    // حساب النقاط والعمق لكل رأس من رؤوس الوجه الرباعي
                    var proj0 = ProjectWithDepth(l1.a, l1.L - 50, l1.b, cx, cy, angleX, angleY, zoom);
                    var proj1 = ProjectWithDepth(l2.a, l2.L - 50, l2.b, cx, cy, angleX, angleY, zoom);
                    var proj2 = ProjectWithDepth(l3.a, l3.L - 50, l3.b, cx, cy, angleX, angleY, zoom);
                    var proj3 = ProjectWithDepth(l4.a, l4.L - 50, l4.b, cx, cy, angleX, angleY, zoom);

                    // حساب متوسط عمق الوجه بالكامل
                    double avgDepth = (proj0.ZDepth + proj1.ZDepth + proj2.ZDepth + proj3.ZDepth) / 4.0;

                    // استخراج اللون للوجه بناءً على إحداثيات Lab الوسطية
                    Color faceColor = LabConverter.ToRgb((l1.L + l3.L) / 2, (l1.a + l3.a) / 2, (l1.b + l3.b) / 2);

                    facesList.Add(new LabFace
                    {
                        Points = new PointF[] { proj0.Point, proj1.Point, proj2.Point, proj3.Point },
                        FaceColor = faceColor,
                        AverageDepth = avgDepth,
                        LabVertices = new (double L, double a, double b)[] { l1, l2, l3, l4 }
                    });
                    // 1. تحويل اللون المختار إلى إحداثيات Lab
                    var lab = LabConverter.FromRgb(selectedColor);

                    // 2. مطابقة معادلة الموقع (Mapping) مع نفس منطق دالة GetLabAt
                    // بما أنكِ في GetLabAt استخدمتِ: Radius = Sin(...) * 80.0
                    double radius = Math.Sin(lab.L / 100.0 * Math.PI) * 80.0;
                    double angle = Math.Atan2(lab.B, lab.A); // الـ B هي المحور الثالث في Lab

                    double xPos = radius * Math.Cos(angle);
                    double zPos = radius * Math.Sin(angle);
                    double yPos = lab.L - 50; 

                    var target = ProjectWithDepth(xPos, yPos, zPos, cx, cy, angleX, angleY, zoom);

                    // // 4. رسم المؤشر الذهبي (بشرط أن يكون داخل حدود الرؤية)
                    // using (Brush b = new SolidBrush(selectedColor))
                    //     g.FillEllipse(b, target.Point.X - 7, target.Point.Y - 7, 14, 14);

                    // using (Pen goldPen = new Pen(Color.Gold, 2f))
                    //     g.DrawEllipse(goldPen, target.Point.X - 9, target.Point.Y - 9, 18, 18);
                }
            }

            // السحر هنا: فرز الأوجه من الأبعد إلى الأقرب (من العمق الأصغر للأكبر)
            facesList.Sort((a, b) => a.AverageDepth.CompareTo(b.AverageDepth));

            // الآن نرسم الأوجه بالترتيب الصحيح لكي تحجب الأوجه الأمامية الأوجه الخلفية بشكل طبيعي
            foreach (var face in facesList)
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(face.Points);
                    using (SolidBrush b = new SolidBrush(face.FaceColor))
                    {
                        g.FillPath(b, path);
                    }
                    using (Pen p = new Pen(Color.FromArgb(20, Color.White), 0.2f))
                    {
                        g.DrawPath(p, path);
                    }
                }
            }
        }

        public static Color GetColorAtPointDirect(Point mousePt, int width, int height, double angleX, double angleY, float zoom)
        {
            int cx = width / 2;
            int cy = height / 2;

            // سنقوم بإعادة بناء الأوجه محلياً هنا باستخدام نفس المنطق الموجود في Render
            for (int i = 0; i < L_Steps; i++)
            {
                for (int j = 0; j < Segments; j++)
                {
                    int nextJ = (j + 1) % Segments;
                    
                    // استدعاء دالتك الأصلية التي تولد الإحداثيات
                    var l1 = GetLabAt(i, j); 
                    var l2 = GetLabAt(i, nextJ);
                    var l3 = GetLabAt(i + 1, nextJ);
                    var l4 = GetLabAt(i + 1, j);

                    // استخدام دالة الإسقاط التي أرسلتِها أنتِ للتو
                    var p0 = ProjectWithDepth(l1.a, l1.L - 50, l1.b, cx, cy, angleX, angleY, zoom);
                    var p1 = ProjectWithDepth(l2.a, l2.L - 50, l2.b, cx, cy, angleX, angleY, zoom);
                    var p2 = ProjectWithDepth(l3.a, l3.L - 50, l3.b, cx, cy, angleX, angleY, zoom);
                    var p3 = ProjectWithDepth(l4.a, l4.L - 50, l4.b, cx, cy, angleX, angleY, zoom);

                    // فحص المثلث الأول (p0, p1, p2)
                    if (IsMouseInTriangle(mousePt, p0.Point, p1.Point, p2.Point))
                    {
                        return LabConverter.ToRgb((l1.L + l2.L + l3.L)/3, (l1.a + l2.a + l3.a)/3, (l1.b + l2.b + l3.b)/3);
                    }
                    
                    // فحص المثلث الثاني (p0, p2, p3)
                    if (IsMouseInTriangle(mousePt, p0.Point, p2.Point, p3.Point))
                    {
                        return LabConverter.ToRgb((l1.L + l3.L + l4.L)/3, (l1.a + l3.a + l4.a)/3, (l1.b + l3.b + l4.b)/3);
                    }
                }
            }
            return Color.Transparent;
        }

        // دالة مساعدة بسيطة جداً لفحص الماوس
        private static bool IsMouseInTriangle(Point p, PointF a, PointF b, PointF c)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(new PointF[] { a, b, c });
                return path.IsVisible(p);
            }
        }
      
        // دالة مساعدة لحساب الاستيفاء بدقة
        private static (bool IsInside, double w1, double w2, double w3) GetBarycentricInterpolation(Point p, PointF a, PointF b, PointF c)
        {
            double det = (b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y);
            double w1 = ((b.Y - c.Y) * (p.X - c.X) + (c.X - b.X) * (p.Y - c.Y)) / det;
            double w2 = ((c.Y - a.Y) * (p.X - c.X) + (a.X - c.X) * (p.Y - c.Y)) / det;
            double w3 = 1 - w1 - w2;
            return (w1 >= 0 && w2 >= 0 && w3 >= 0, w1, w2, w3);
        }

        private static (double L, double a, double b) GetLabAt(int i, int j)
        {
            double L = i * (100.0 / L_Steps);
            double radius = Math.Sin(i * Math.PI / L_Steps) * 80.0;
            double angle = j * (360.0 / Segments) * Math.PI / 180.0;
            return (L, radius * Math.Cos(angle), radius * Math.Sin(angle));
        }
       
    }
    
}