// using System;
// using System.Drawing;
// using System.Drawing.Drawing2D;

// namespace pixellab.Renderers
// {
//     public static class RgbCubeRenderer
//     {
//         // تابع الإسقاط الهندسي من 3D إلى 2D لشاشة العرض
//         private static PointF Project(double x, double y, double z, int cx, int cy, double angleX, double angleY, float zoom)
//         {
//             double radX = angleX * Math.PI / 180.0;
//             double radY = angleY * Math.PI / 180.0;

//             double cosX = Math.Cos(radX), sinX = Math.Sin(radX);
//             double cosY = Math.Cos(radY), sinY = Math.Sin(radY);

//             double rY = y * cosX - z * sinX;
//             double rZ = y * sinX + z * cosX;
//             double rX = x * cosY + rZ * sinY;

//             // حساب الإحداثيات على الشاشة مع أخذ الزوم ومركز الرسم بعين الاعتبار
//             float screenX = (float)(cx + rX * 2.5 * zoom);
//             float screenY = (float)(cy - rY * 2.5 * zoom);

//             return new PointF(screenX, screenY);
//         }

//         // التابع المسؤول عن رسم شبكة النقاط المنتظمة للفضاء اللوني (الطلب الرابع)
//         public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom, Color selectedColor)
//         {
//             g.SmoothingMode = SmoothingMode.AntiAlias;
//             g.Clear(Color.FromArgb(20, 20, 20)); // خلفية داكنة مريحة للعين

//             int cx = width / 2;
//             int cy = height / 2;

//             // رسم المحاور الإحداثية الثلاثية الأبعاد (R, G, B) لتوضيح الفضاء الأكاديمي
//             PointF origin = Project(-50, -50, -50, cx, cy, angleX, angleY, zoom);
//             PointF axisR = Project(60, -50, -50, cx, cy, angleX, angleY, zoom);
//             PointF axisG = Project(-50, 60, -50, cx, cy, angleX, angleY, zoom);
//             PointF axisB = Project(-50, -50, 60, cx, cy, angleX, angleY, zoom);

//             using (Pen rPen = new Pen(Color.Red, 2f)) g.DrawLine(rPen, origin, axisR);
//             using (Pen gPen = new Pen(Color.Lime, 2f)) g.DrawLine(gPen, origin, axisG);
//             using (Pen bPen = new Pen(Color.Blue, 2f)) g.DrawLine(bPen, origin, axisB);

//             // توليد شبكة نقاط منتظمة بمعدل 8 خطوات لكل محور (8x8x8 = 512 نقطة ملونة خفيفة ومثالية للأداء)
//             int steps = 8; 
//             for (int r = 0; r < steps; r++)
//             {
//                 for (int gVal = 0; gVal < steps; gVal++)
//                 {
//                     for (int b = 0; b < steps; b++)
//                     {
//                         // 1. حساب قيم الألوان الحقيقية الصافية (من 0 لـ 255)
//                         int cR = (r * 255) / (steps - 1);
//                         int cG = (gVal * 255) / (steps - 1);
//                         int cB = (b * 255) / (steps - 1);
//                         Color dotColor = Color.FromArgb(cR, cG, cB);

//                         // 2. تحويل القيم الإحداثية لتتمركّز حول نقطة الأصل (من -50 إلى 50)
//                         double x = ((cR / 255.0) * 100.0) - 50.0;
//                         double y = ((cG / 255.0) * 100.0) - 50.0;
//                         double z = ((cB / 255.0) * 100.0) - 50.0;

//                         // 3. إسقاط النقطة ورسمها كدائرة صغيرة ملونة بلونها الحقيقي
//                         PointF pt = Project(x, y, z, cx, cy, angleX, angleY, zoom);
//                         using (Brush brush = new SolidBrush(dotColor))
//                         {
//                             g.FillEllipse(brush, pt.X - 3, pt.Y - 3, 6, 6);
//                         }
//                     }
//                 }
//             }

//             // 4. رسم المؤشر المشع فوق اللون المختار حالياً (الطلب الخامس - المزامنة)
//             double pX = ((selectedColor.R / 255.0) * 100.0) - 50.0;
//             double pY = ((selectedColor.G / 255.0) * 100.0) - 50.0;
//             double pZ = ((selectedColor.B / 255.0) * 100.0) - 50.0;

//             PointF targetPt = Project(pX, pY, pZ, cx, cy, angleX, angleY, zoom);

//             // رسم هالة ذهبية مشعة حول النقطة المحددة لتظهر بوضوح للدكتور أثناء المناقشة
//             using (Brush b = new SolidBrush(selectedColor)) g.FillEllipse(b, targetPt.X - 7, targetPt.Y - 7, 14, 14);
//             using (Pen goldPen = new Pen(Color.Gold, 2f)) g.DrawEllipse(goldPen, targetPt.X - 9, targetPt.Y - 9, 18, 18);
//         }

//         // التابع السحري الجديد لالتقاط اللون الحقيقي بدقة متناهية بناءً على خوارزمية المسافة الأقرب
//         public static Color GetColorAtPointDirect(Point mousePt, int width, int height, double angleX, double angleY, float zoom)
//         {
//             int cx = width / 2;
//             int cy = height / 2;

//             Color closestColor = Color.Transparent;
//             double minDistance = double.MaxValue;
//             double selectionRadius = 15.0; // بكسلات الأمان (نصف قطر حلقة الالتقاط حول الماوس)

//             int steps = 8;
//             for (int r = 0; r < steps; r++)
//             {
//                 for (int gVal = 0; gVal < steps; gVal++)
//                 {
//                     for (int b = 0; b < steps; b++)
//                     {
//                         int cR = (r * 255) / (steps - 1);
//                         int cG = (gVal * 255) / (steps - 1);
//                         int cB = (b * 255) / (steps - 1);

//                         double x = ((cR / 255.0) * 100.0) - 50.0;
//                         double y = ((cG / 255.0) * 100.0) - 50.0;
//                         double z = ((cB / 255.0) * 100.0) - 50.0;

//                         // إسقاط النقطة لمعرفة موقعها الحالي على الشاشة
//                         PointF pt = Project(x, y, z, cx, cy, angleX, angleY, zoom);

//                         // حساب المسافة الإقليدية بين نقرة الماوس وموقع النقطة المستهدفة
//                         double dist = Math.Sqrt(Math.Pow(mousePt.X - pt.X, 2) + Math.Pow(mousePt.Y - pt.Y, 2));

//                         // إذا كانت النقطة أقرب من كل ما سبق وضمن حدود الأمان، نحتفظ بها
//                         if (dist < minDistance && dist <= selectionRadius)
//                         {
//                             minDistance = dist;
//                             closestColor = Color.FromArgb(cR, cG, cB);
//                         }
//                     }
//                 }
//             }

//             return closestColor;
//         }
//     }
// }
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace pixellab.Renderers
{
    public static class RgbCubeRenderer
    {
        private static (PointF Point, double ZDepth) ProjectWithDepth(double x, double y, double z, int cx, int cy, double angleX, double angleY, float zoom)
        {
            double radX = angleX * Math.PI / 180.0;
            double radY = angleY * Math.PI / 180.0;

            double cosX = Math.Cos(radX), sinX = Math.Sin(radX);
            double cosY = Math.Cos(radY), sinY = Math.Sin(radY);

            // تدوير حول المحاور الثلاثية
            double rY = y * cosX - z * sinX;
            double rZ = y * sinX + z * cosX;
            double rX = x * cosY + rZ * sinY;
            double depthZ = -x * sinY + rZ * cosY;

            // إسقاط على الشاشة مع أخذ الزوم بعين الاعتبار
            float screenX = (float)(cx + rX * 2.5 * zoom);
            float screenY = (float)(cy - rY * 2.5 * zoom);

            return (new PointF(screenX, screenY), depthZ);
        }

        private static PointF Project(double x, double y, double z, int cx, int cy, double angleX, double angleY, float zoom)
        {
            return ProjectWithDepth(x, y, z, cx, cy, angleX, angleY, zoom).Point;
        }

        public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom, Color selectedColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = width / 2;
            int cy = height / 2;

            PointF[] pts = new PointF[8];
            double[] vertexDepths = new double[8];
            double[,] vertices = {
                {-50, -50, -50}, { 50, -50, -50}, { 50,  50, -50}, {-50,  50, -50}, 
                {-50, -50,  50}, { 50, -50,  50}, { 50,  50,  50}, {-50,  50,  50} 
            };

            for (int i = 0; i < 8; i++)
            {
                var projection = ProjectWithDepth(vertices[i, 0], vertices[i, 1], vertices[i, 2], cx, cy, angleX, angleY, zoom);
                pts[i] = projection.Point;
                vertexDepths[i] = projection.ZDepth;
            }

            int[][] faces = {
                new int[] {0, 1, 2, 3}, // الخلفي
                new int[] {4, 5, 6, 7}, // الأمامي
                new int[] {0, 1, 5, 4}, // السفلي
                new int[] {2, 3, 7, 6}, // العلوي
                new int[] {0, 3, 7, 4}, // الأيسر
                new int[] {1, 2, 6, 5}  // الأيمن
            };

            Color[][] faceGradientColors = {
                new Color[] { Color.Black, Color.Red, Color.Yellow, Color.Green },     
                new Color[] { Color.Blue, Color.Magenta, Color.White, Color.Cyan },    
                new Color[] { Color.Black, Color.Red, Color.Magenta, Color.Blue },     
                new Color[] { Color.Yellow, Color.Green, Color.Cyan, Color.White },    
                new Color[] { Color.Black, Color.Green, Color.Cyan, Color.Blue },     
                new Color[] { Color.Red, Color.Yellow, Color.White, Color.Magenta }    
            };

            // ترتيب الأوجه حسب العمق (Painter's Algorithm) لضمان عدم تداخل الأوجه الخلفية
            List<int> faceIndices = new List<int> { 0, 1, 2, 3, 4, 5 };
            faceIndices.Sort((a, b) => {
                double depthA = (vertexDepths[faces[a][0]] + vertexDepths[faces[a][1]] + vertexDepths[faces[a][2]] + vertexDepths[faces[a][3]]) / 4.0;
                double depthB = (vertexDepths[faces[b][0]] + vertexDepths[faces[b][1]] + vertexDepths[faces[b][2]] + vertexDepths[faces[b][3]]) / 4.0;
                return depthA.CompareTo(depthB); // من الأبعد إلى الأقرب
            });

            foreach (int i in faceIndices)
            {
                PointF[] facePoints = { pts[faces[i][0]], pts[faces[i][1]], pts[faces[i][2]], pts[faces[i][3]] };
                
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(facePoints);
                    using (PathGradientBrush pgb = new PathGradientBrush(path))
                    {
                        pgb.CenterColor = Color.FromArgb(
                            (faceGradientColors[i][0].R + faceGradientColors[i][1].R + faceGradientColors[i][2].R + faceGradientColors[i][3].R) / 4,
                            (faceGradientColors[i][0].G + faceGradientColors[i][1].G + faceGradientColors[i][2].G + faceGradientColors[i][3].G) / 4,
                            (faceGradientColors[i][0].B + faceGradientColors[i][1].B + faceGradientColors[i][2].B + faceGradientColors[i][3].B) / 4
                        );
                        pgb.SurroundColors = faceGradientColors[i];
                        g.FillPolygon(pgb, facePoints);
                    }
                }

                // رسم خطوط حدود الأوجه لرؤية هندسية واضحة للمكعب
                using (Pen p = new Pen(Color.FromArgb(70, Color.White), 1f))
                {
                    g.DrawPolygon(p, facePoints);
                }
            }

            // حساب موقع المؤشر المشع المشتق من الألوان المدخلة وتحديث مكانه فوق الرأس تماماً
            double pX = ((selectedColor.R / 255.0) * 100.0) - 50.0;
            double pY = ((selectedColor.G / 255.0) * 100.0) - 50.0;
            double pZ = ((selectedColor.B / 255.0) * 100.0) - 50.0;

            PointF targetPt = Project(pX, pY, pZ, cx, cy, angleX, angleY, zoom);

            // رسم دائرة لونيّة تمثل مكان اللون المختار حالياً داخل الفضاء ثلاثي الأبعاد
            using (Brush b = new SolidBrush(selectedColor)) g.FillEllipse(b, targetPt.X - 7, targetPt.Y - 7, 14, 14);
            using (Pen goldPen = new Pen(Color.Gold, 2f)) g.DrawEllipse(goldPen, targetPt.X - 9, targetPt.Y - 9, 18, 18);
        }

        // 🌟 دالة التقاط اللون المطورة لحل مشكلة التدوير مية بالمية
        public static Color GetColorAtPointDirect(Point mousePt, int width, int height, double angleX, double angleY, float zoom)
        {
            int cx = width / 2;
            int cy = height / 2;

            PointF[] pts = new PointF[8];
            double[] vertexDepths = new double[8];
            double[,] vertices = {
                {-50, -50, -50}, { 50, -50, -50}, { 50,  50, -50}, {-50,  50, -50}, 
                {-50, -50,  50}, { 50, -50,  50}, { 50,  50,  50}, {-50,  50,  50} 
            };

            for (int i = 0; i < 8; i++)
            {
                var projection = ProjectWithDepth(vertices[i, 0], vertices[i, 1], vertices[i, 2], cx, cy, angleX, angleY, zoom);
                pts[i] = projection.Point;
                vertexDepths[i] = projection.ZDepth;
            }

            int[][] faces = {
                new int[] {0, 1, 2, 3}, new int[] {4, 5, 6, 7}, new int[] {0, 1, 5, 4}, 
                new int[] {2, 3, 7, 6}, new int[] {0, 3, 7, 4}, new int[] {1, 2, 6, 5}
            };

            List<int> faceIndices = new List<int> { 0, 1, 2, 3, 4, 5 };
            faceIndices.Sort((a, b) => {
                double depthA = (vertexDepths[faces[a][0]] + vertexDepths[faces[a][1]] + vertexDepths[faces[a][2]] + vertexDepths[faces[a][3]]) / 4.0;
                double depthB = (vertexDepths[faces[b][0]] + vertexDepths[faces[b][1]] + vertexDepths[faces[b][2]] + vertexDepths[faces[b][3]]) / 4.0;
                return depthA.CompareTo(depthB);
            });

            // نفحص الأوجه من الأقرب للعين (الترتيب العكسي للقائمة المفرزة)
            for (int j = faceIndices.Count - 1; j >= 0; j--)
            {
                int i = faceIndices[j];
                PointF p0 = pts[faces[i][0]], p1 = pts[faces[i][1]], p2 = pts[faces[i][2]], p3 = pts[faces[i][3]];

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(new PointF[] { p0, p1, p2, p3 });

                    if (path.IsVisible(mousePt))
                    {
                        // 🌟 استخدام الـ Ray-Casting أو الإسقاط المحلي الدقيق للوجه المختار
                        // حساب نسب توضع النقطة داخل المضلع بشكل دقيق جداً يحاكي فضاء الـ RGB الحقيقي للمكعب
                        double u = CalculateAdvancedBilinearRatio(mousePt, p0, p1, p2, p3, out double v);

                        u = Math.Max(0.0, Math.Min(1.0, u));
                        v = Math.Max(0.0, Math.Min(1.0, v));

                        // جلب الإحداثيات الـ 3D الأساسية للرؤوس الأربعة للوجه الحالي ومطابقتها مع فضاء RGB الكوني [0, 255]
                        double[,] faceVerts3D = {
                            { vertices[faces[i][0], 0], vertices[faces[i][0], 1], vertices[faces[i][0], 2] },
                            { vertices[faces[i][1], 0], vertices[faces[i][1], 1], vertices[faces[i][1], 2] },
                            { vertices[faces[i][2], 0], vertices[faces[i][2], 1], vertices[faces[i][2], 2] },
                            { vertices[faces[i][3], 0], vertices[faces[i][3], 1], vertices[faces[i][3], 2] }
                        };

                        // القيام بالاستيفاء مباشرة في الفضاء ثلاثي الأبعاد لنحصل على أدق نقطة لونية (X, Y, Z) داخل المكعب
                        double x3D = (1 - u) * ((1 - v) * faceVerts3D[0, 0] + v * faceVerts3D[3, 0]) + u * ((1 - v) * faceVerts3D[1, 0] + v * faceVerts3D[2, 0]);
                        double y3D = (1 - u) * ((1 - v) * faceVerts3D[0, 1] + v * faceVerts3D[3, 1]) + u * ((1 - v) * faceVerts3D[1, 1] + v * faceVerts3D[2, 1]);
                        double z3D = (1 - u) * ((1 - v) * faceVerts3D[0, 2] + v * faceVerts3D[3, 2]) + u * ((1 - v) * faceVerts3D[1, 2] + v * faceVerts3D[2, 2]);

                        // تحويل القيم الحسابية من النطاق [-50, 50] إلى النطاق اللوني [0, 255] بدقة مطلقة
                        int r = (int)Math.Round(((x3D + 50.0) / 100.0) * 255.0);
                        int gVal = (int)Math.Round(((y3D + 50.0) / 100.0) * 255.0);
                        int b = (int)Math.Round(((z3D + 50.0) / 100.0) * 255.0);

                        return Color.FromArgb(
                            Math.Max(0, Math.Min(255, r)),
                            Math.Max(0, Math.Min(255, gVal)),
                            Math.Max(0, Math.Min(255, b))
                        );
                    }
                }
            }
            return Color.Transparent;
        }

        // 🌟 تابع حسابي متطور لإيجاد الإحداثيات الدقيقة للنقطة داخل المضلع الرباعي المنحرف ثلاثي الأبعاد
        private static double CalculateAdvancedBilinearRatio(Point pt, PointF p0, PointF p1, PointF p2, PointF p3, out double v)
        {
            // إسقاط محلي مبني على مسافات النقطة التناسبية بالنسبة للمحاور الأربعة الملتوية هندسياً
            double d1 = DistanceToLine(pt, p0, p3);
            double d2 = DistanceToLine(pt, p1, p2);
            double u = (d1 + d2 > 0) ? d1 / (d1 + d2) : 0;

            double d3 = DistanceToLine(pt, p0, p1);
            double d4 = DistanceToLine(pt, p3, p2);
            v = (d3 + d4 > 0) ? d3 / (d3 + d4) : 0;

            return u;
        }

        private static double DistanceToLine(Point p, PointF l1, PointF l2)
        {
            double num = Math.Abs((l2.Y - l1.Y) * p.X - (l2.X - l1.X) * p.Y + l2.X * l1.Y - l2.Y * l1.X);
            double den = Math.Sqrt(Math.Pow(l2.Y - l1.Y, 2) + Math.Pow(l2.X - l1.X, 2));
            return den == 0 ? 0 : num / den;
        }
    }
}