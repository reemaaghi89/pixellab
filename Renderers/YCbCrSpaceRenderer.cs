using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace pixellab.Renderers
{
    public static class YCbCrSpaceRenderer
    {
        private static (PointF Point, double ZDepth) ProjectWithDepth(double x, double y, double z, int cx, int cy, double angleX, double angleY, float zoom)
        {
            double radX = angleX * Math.PI / 180.0;
            double radY = angleY * Math.PI / 180.0;

            double cosX = Math.Cos(radX), sinX = Math.Sin(radX);
            double cosY = Math.Cos(radY), sinY = Math.Sin(radY);

            // تدوير ثلاثي الأبعاد
            double rY = y * cosX - z * sinX;
            double rZ = y * sinX + z * cosX;
            double rX = x * cosY + rZ * sinY;
            double depthZ = -x * sinY + rZ * cosY;

            // ضبط التحجيم ليتناسب متوازي المستطيلات مع مساحة الشاشة بشكل ممتاز
            float screenX = (float)(cx + rX * 2.5 * zoom);
            float screenY = (float)(cy - rY * 2.5 * zoom);

            return (new PointF(screenX, screenY), depthZ);
        }

        public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom, Color selectedColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = width / 2;
            int cy = height / 2;

            // 🌟 توليد رؤوس متوازي المستطيلات الحقيقي عبر تحويل رؤوس مكعب RGB الـ 8 مباشرة
            // الرؤوس بالترتيب في فضاء RGB [0, 255]
            double[,] rgbVertices = {
                {0,   0,   0},   // 0: الأسود
                {255, 0,   0},   // 1: الأحمر
                {255, 255, 0},   // 2: الأصفر
                {0,   255, 0},   // 3: الأخضر
                {0,   0,   255}, // 4: الأزرق
                {255, 0,   255}, // 5: الماجنتا
                {255, 255, 255}, // 6: الأبيض
                {0,   255, 255}  // 7: السيان
            };

            PointF[] pts = new PointF[8];
            double[] vertexDepths = new double[8];
            double[,] ycbcrVertices = new double[8, 3];

            for (int i = 0; i < 8; i++)
            {
                double r = rgbVertices[i, 0];
                double gVal = rgbVertices[i, 1];
                double b = rgbVertices[i, 2];

                // التحويل الرياضي الدقيق لـ YCbCr
                double yVal = 0.299 * r + 0.587 * gVal + 0.114 * b;
                double cb = 128 - 0.168736 * r - 0.331264 * gVal + 0.5 * b;
                double cr = 128 + 0.5 * r - 0.418688 * gVal - 0.081312 * b;

                // مطابقة النطاق ليكون متمحوراً حول المركز [-50, 50] للرسم ثلاثي الأبعاد
                ycbcrVertices[i, 0] = ((cb / 255.0) * 100.0) - 50.0; // محور X
                ycbcrVertices[i, 1] = ((cr / 255.0) * 100.0) - 50.0; // محور Y
                ycbcrVertices[i, 2] = ((yVal / 255.0) * 100.0) - 50.0; // محور Z (الارتفاع الشاقولي للإضاءة)

                var projection = ProjectWithDepth(ycbcrVertices[i, 0], ycbcrVertices[i, 1], ycbcrVertices[i, 2], cx, cy, angleX, angleY, zoom);
                pts[i] = projection.Point;
                vertexDepths[i] = projection.ZDepth;
            }

            // تعريف الأوجه الستة لمتوازي المستطيلات المتكامل (المكعب المائل)
            int[][] faces = {
                new int[] {0, 1, 2, 3}, // الوجه السفلي المتصل بالأسود
                new int[] {4, 5, 6, 7}, // الوجه العلوي المتصل بالسيان/الأبيض
                new int[] {0, 1, 5, 4}, 
                new int[] {2, 3, 7, 6}, 
                new int[] {0, 3, 7, 4}, 
                new int[] {1, 2, 6, 5}  
            };

            // تدرجات الألوان الحقيقية للرؤوس مطابقة تماماً للمرجع الأكاديمي المائل
            Color[][] faceGradientColors = {
                new Color[] { Color.Black, Color.Red, Color.Yellow, Color.Green },
                new Color[] { Color.Blue, Color.Magenta, Color.White, Color.Cyan },
                new Color[] { Color.Black, Color.Red, Color.Magenta, Color.Blue },
                new Color[] { Color.Yellow, Color.Green, Color.Cyan, Color.White },
                new Color[] { Color.Black, Color.Green, Color.Cyan, Color.Blue },
                new Color[] { Color.Red, Color.Yellow, Color.White, Color.Magenta }
            };

            // ترتيب الأوجه حسب العمق (Painter's Algorithm)
            List<int> faceIndices = new List<int> { 0, 1, 2, 3, 4, 5 };
            faceIndices.Sort((a, b) => {
                double depthA = (vertexDepths[faces[a][0]] + vertexDepths[faces[a][1]] + vertexDepths[faces[a][2]] + vertexDepths[faces[a][3]]) / 4.0;
                double depthB = (vertexDepths[faces[b][0]] + vertexDepths[faces[b][1]] + vertexDepths[faces[b][2]] + vertexDepths[faces[b][3]]) / 4.0;
                return depthA.CompareTo(depthB);
            });

            // رسم الأوجه الستة المتوازية والممتلئة بالتدرج اللوني الصحيح
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

                // رسم الخطوط الهيكلية البيضاء الناعمة لتوضيح توازي المستطيلات المائل
                using (Pen p = new Pen(Color.FromArgb(60, Color.White), 1f))
                {
                    g.DrawPolygon(p, facePoints);
                }
            }

            // 🌟 2. حساب وتحديث موقع المؤشر الذهبي المتزامن في الفضاء المائل
            double selY = 0.299 * selectedColor.R + 0.587 * selectedColor.G + 0.114 * selectedColor.B;
            double selCb = 128 - 0.168736 * selectedColor.R - 0.331264 * selectedColor.G + 0.5 * selectedColor.B;
            double selCr = 128 + 0.5 * selectedColor.R - 0.418688 * selectedColor.G - 0.081312 * selectedColor.B;

            double pX = ((selCb / 255.0) * 100.0) - 50.0;
            double pY = ((selCr / 255.0) * 100.0) - 50.0;
            double pZ = ((selY / 255.0) * 100.0) - 50.0;

            var targetProj = ProjectWithDepth(pX, pY, pZ, cx, cy, angleX, angleY, zoom);
            PointF targetPt = targetProj.Point;

            using (Brush bBrush = new SolidBrush(selectedColor)) g.FillEllipse(bBrush, targetPt.X - 7, targetPt.Y - 7, 14, 14);
            using (Pen goldPen = new Pen(Color.Gold, 2f)) g.DrawEllipse(goldPen, targetPt.X - 9, targetPt.Y - 9, 18, 18);
        }
    }
}