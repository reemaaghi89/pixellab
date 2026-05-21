using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace pixellab.Renderers
{
    public static class RgbCubeRenderer
    {
        // تابع الإسقاط الرياضي مع أخذ عامل الـ Zoom بعين الاعتبار
        private static PointF Project(double x, double y, double z, int cx, int cy, double angleX, double angleY, float zoom)
        {
            double radX = angleX * Math.PI / 180.0;
            double radY = angleY * Math.PI / 180.0;

            double cosX = Math.Cos(radX), sinX = Math.Sin(radX);
            double cosY = Math.Cos(radY), sinY = Math.Sin(radY);

            // مصفوفات الدوران ثلاثي الأبعاد
            double rY = y * cosX - z * sinX;
            double rZ = y * sinX + z * cosX;
            double rX = x * cosY + rZ * sinY;

            // تطبيق الـ Zoom على الإسقاط
            float screenX = (float)(cx + rX * 0.4 * zoom);
            float screenY = (float)(cy - rY * 0.4 * zoom);

            return new PointF(screenX, screenY);
        }

        public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom, Color selectedColor, Font font)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(20, 20, 20)); // خلفية داكنة فخمة

            int cx = width / 2;
            int cy = height / 2;

            // 1. تعريف الرؤوس الثمانية للمكعب (بين -100 و 100)
            PointF[] pts = new PointF[8];
            double[,] vertices = {
                {-100, -100, -100}, // 0: أسود
                {100, -100, -100},  // 1: أحمر
                {100, 100, -100},   // 2: أصفر
                {-100, 100, -100},  // 3: أخضر
                {-100, -100, 100},  // 4: أزرق
                {100, -100, 100},   // 5: ماجنتا
                {100, 100, 100},    // 6: أبيض
                {-100, 100, 100}    // 7: سيان
            };

            for (int i = 0; i < 8; i++)
            {
                pts[i] = Project(vertices[i, 0], vertices[i, 1], vertices[i, 2], cx, cy, angleX, angleY, zoom);
            }

            // 2. تعريف الأوجه الستة للمكعب بترتيب الرؤوس الصحيح
            int[][] faces = {
                new int[] {0, 1, 2, 3}, // الوجه الخلفي
                new int[] {4, 5, 6, 7}, // الوجه الأمامي
                new int[] {0, 1, 5, 4}, // الوجه السفلي
                new int[] {2, 3, 7, 6}, // الوجه العلوي
                new int[] {0, 3, 7, 4}, // الوجه الأيسر
                new int[] {1, 2, 6, 5}  // الوجه الأيمن
            };

            // ألوان الزوايا المقابلة لكل وجه لدمجها (تحاكي تلوين الأوجه الحقيقي)
            Color[] faceBaseColors = { Color.Red, Color.Magenta, Color.Blue, Color.Yellow, Color.Cyan, Color.Green };

            // 3. رسم الأوجه مصمتة مع التخلص الفوري من الفراشي والأقلام بكل دورة لمنع الـ Memory Leak
            for (int i = 0; i < faces.Length; i++)
            {
                PointF[] facePoints = { pts[faces[i][0]], pts[faces[i][1]], pts[faces[i][2]], pts[faces[i][3]] };
                
                using (PathGradientBrush pgb = new PathGradientBrush(facePoints))
                {
                    pgb.CenterColor = Color.FromArgb(128, 128, 128); 
                    pgb.SurroundColors = new Color[] { faceBaseColors[i], faceBaseColors[i], Color.White, faceBaseColors[i] };
                    
                    g.FillPolygon(pgb, facePoints);
                }

                using (Pen p = new Pen(Color.FromArgb(60, Color.White), 1f))
                {
                    g.DrawPolygon(p, facePoints);
                }
            }

            // 4. حساب وإسقاط موقع اللون المختار حالياً المتزامن
            double pX = ((selectedColor.R / 255.0) * 200.0) - 100.0;
            double pY = ((selectedColor.G / 255.0) * 200.0) - 100.0;
            double pZ = ((selectedColor.B / 255.0) * 200.0) - 100.0;

            PointF targetPt = Project(pX, pY, pZ, cx, cy, angleX, angleY, zoom);

            // 5. رسم المؤشر المشع التفاعلي مع حماية موارد الـ SolidBrush والـ Pen
            using (Brush b = new SolidBrush(selectedColor))
            {
                g.FillEllipse(b, targetPt.X - 8, targetPt.Y - 8, 16, 16);
            }
            using (Pen goldPen = new Pen(Color.Gold, 2.5f))
            {
                g.DrawEllipse(goldPen, targetPt.X - 10, targetPt.Y - 10, 20, 20);
            }
        }

        public static Color GetColorAtPoint(Point mousePt, int width, int height, double angleX, double angleY, float zoom)
        {
            int cx = width / 2;
            int cy = height / 2;

            PointF[] pts = new PointF[8];
            double[,] vertices = {
                {-100, -100, -100}, {-100, -100, 100}, {100, -100, 100}, {100, -100, -100}, 
                {-100, 100, -100},  {-100, 100, 100},  {100, 100, 100},  {100, 100, -100}   
            };

            double radX = angleX * Math.PI / 180.0;
            double radY = angleY * Math.PI / 180.0;
            double cosX = Math.Cos(radX), sinX = Math.Sin(radX);
            double cosY = Math.Cos(radY), sinY = Math.Sin(radY);

            for (int i = 0; i < 8; i++)
            {
                double rY = vertices[i, 1] * cosX - vertices[i, 2] * sinX;
                double rZ = vertices[i, 1] * sinX + vertices[i, 2] * cosX;
                double rX = vertices[i, 0] * cosY + rZ * sinY;

                pts[i] = new PointF((float)(cx + rX * 0.4 * zoom), (float)(cy - rY * 0.4 * zoom));
            }

            int[][] faces = {
                new int[] {0, 1, 2, 3}, 
                new int[] {4, 5, 6, 7}, 
                new int[] {0, 1, 5, 4}, 
                new int[] {2, 3, 7, 6}, 
                new int[] {0, 3, 7, 4}, 
                new int[] {1, 2, 6, 5}  
            };

            for (int i = faces.Length - 1; i >= 0; i--)
            {
                // ضمان تفريغ الـ GraphicsPath فوراً لمنع تسريب الذاكرة عند الضغط المتكرر
                using (GraphicsPath path = new GraphicsPath())
                {
                    PointF[] facePoints = { pts[faces[i][0]], pts[faces[i][1]], pts[faces[i][2]], pts[faces[i][3]] };
                    path.AddPolygon(facePoints);

                    if (path.IsVisible(mousePt))
                    {
                        int r = (i == 0 || i == 3) ? 255 : (i == 4 ? 120 : 0);
                        int g = (i == 1 || i == 3) ? 255 : (i == 5 ? 180 : 0);
                        int b = (i == 2 || i == 5) ? 255 : (i == 1 ? 200 : 50);

                        return Color.FromArgb(Math.Min(255, Math.Max(0, r)), Math.Min(255, Math.Max(0, g)), Math.Min(255, Math.Max(0, b)));
                    }
                }
            }

            return Color.Transparent; 
        }
    }
}