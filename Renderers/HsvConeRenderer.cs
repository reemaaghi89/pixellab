using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace pixellab.Renderers
{
    public static class HsvConeRenderer
    {
        private static (PointF Point, double ZDepth) ProjectWithDepth(double x, double y, double z, int cx, int cy, double angleX, double angleY, float zoom)
        {
            double radX = angleX * Math.PI / 180.0;
            double radY = angleY * Math.PI / 180.0;

            double cosX = Math.Cos(radX), sinX = Math.Sin(radX);
            double cosY = Math.Cos(radY), sinY = Math.Sin(radY);

            double rY = y * cosX - z * sinX;
            double rZ = y * sinX + z * cosX;
            double rX = x * cosY + rZ * sinY;
            double depthZ = -x * sinY + rZ * cosY;

            float screenX = (float)(cx + rX * 2.5 * zoom);
            float screenY = (float)(cy - rY * 2.5 * zoom);

            return (new PointF(screenX, screenY), depthZ);
        }

        public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom, Color selectedColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = width / 2;
            int cy = height / 2;

            // حساب قيم الـ HSV للون المختار حالياً لرسم المؤشر في مكانه الصحيح
            double selH = selectedColor.GetHue();
            double selS = (Math.Max(selectedColor.R, Math.Max(selectedColor.G, selectedColor.B)) == 0) ? 0 : 
                          1d - (1d * Math.Min(selectedColor.R, Math.Min(selectedColor.G, selectedColor.B)) / Math.Max(selectedColor.R, Math.Max(selectedColor.G, selectedColor.B)));
            double selV = Math.Max(selectedColor.R, Math.Max(selectedColor.G, selectedColor.B)) / 255d;

            // بناء قاعدة المخروط (الدائرة العلوية الدائرية) باستخدام 32 نقطة لضمان النعومة
            int segments = 32;
            PointF[] coneBasePts = new PointF[segments];
            double[] baseDepths = new double[segments];
            double radius = 50.0;
            double coneHeight = 100.0;

            // نقاط القاعدة تقع عند الارتفاع العلوي (Z = 50) للـ Value = 1
            for (int i = 0; i < segments; i++)
            {
                double angle = (i * 360.0 / segments) * Math.PI / 180.0;
                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);
                double z = 30.0; // أعلى المخروط

                var proj = ProjectWithDepth(x, y, z, cx, cy, angleX, angleY, zoom);
                coneBasePts[i] = proj.Point;
                baseDepths[i] = proj.ZDepth;
            }

            // رأس المخروط السفلي المدبب يمثل (Black / Value = 0) ويقع عند الإحداثي السفلي
            var apexProj = ProjectWithDepth(0, 0, -50.0, cx, cy, angleX, angleY, zoom);
            PointF apexPt = apexProj.Point;

            // رسم الأوجه الجانبية للمخروط بترتيب العمق الصحيح لتجنب التداخل البصري
            List<int> sideIndices = new List<int>();
            for (int i = 0; i < segments; i++) sideIndices.Add(i);

            sideIndices.Sort((a, b) => {
                int nextA = (a + 1) % segments;
                int nextB = (b + 1) % segments;
                double depthA = (baseDepths[a] + baseDepths[nextA] + apexProj.ZDepth) / 3.0;
                double depthB = (baseDepths[b] + baseDepths[nextB] + apexProj.ZDepth) / 3.0;
                return depthA.CompareTo(depthB);
            });

            // رسم الأوجه الجانبية بتدرج لوني مناسب للـ Hue
            foreach (int i in sideIndices)
            {
                int next = (i + 1) % segments;
                PointF[] sideTriangle = { coneBasePts[i], coneBasePts[next], apexPt };

                double hueAngle = i * 360.0 / segments;
                Color sideColor = ColorFromHsv(hueAngle, 1.0, 0.8);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(sideTriangle);
                    using (SolidBrush br = new SolidBrush(Color.FromArgb(180, sideColor)))
                    {
                        g.FillPolygon(br, sideTriangle);
                    }
                }
                using (Pen p = new Pen(Color.FromArgb(40, Color.White), 1f)) g.DrawPolygon(p, sideTriangle);
            }

            // رسم قاعدة المخروط العلوية الملونة بالتدرج الدائري الكامل للـ Hue
            using (GraphicsPath baseCircle = new GraphicsPath())
            {
                baseCircle.AddPolygon(coneBasePts);
                using (PathGradientBrush pgb = new PathGradientBrush(baseCircle))
                {
                    pgb.CenterColor = Color.White; // المركز أبيض لأن الإشباع صفر السطوع كامل
                    Color[] surroundColors = new Color[segments];
                    for (int i = 0; i < segments; i++)
                    {
                        surroundColors[i] = ColorFromHsv(i * 360.0 / segments, 1.0, 1.0);
                    }
                    pgb.SurroundColors = surroundColors;
                    g.FillPolygon(pgb, coneBasePts);
                }
            }

            // 🌟 حساب موضع المؤشر المشع المشتق من الـ HSV اللحظي
            double hRad = selH * Math.PI / 180.0;
            double curRadius = radius * selS * selV; // يتناقص نصف القطر كلما قل الـ Value أو الـ Saturation
            double pX = curRadius * Math.Cos(hRad);
            double pY = curRadius * Math.Sin(hRad);
            double pZ = (selV * coneHeight) - 50.0; // إسقاط الارتفاع بناءً على قيمة السطوع V

            var targetProj = ProjectWithDepth(pX, pY, pZ, cx, cy, angleX, angleY, zoom);
            PointF targetPt = targetProj.Point;

            using (Brush b = new SolidBrush(selectedColor)) g.FillEllipse(b, targetPt.X - 7, targetPt.Y - 7, 14, 14);
            using (Pen goldPen = new Pen(Color.Gold, 2f)) g.DrawEllipse(goldPen, targetPt.X - 9, targetPt.Y - 9, 18, 18);
        }

        // تابع حسابي لإعادة بناء اللون من الـ HSV لغرض الرسم النظيف للمخروط
        private static Color ColorFromHsv(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0) return Color.FromArgb(255, v, t, p);
            else if (hi == 1) return Color.FromArgb(255, q, v, p);
            else if (hi == 2) return Color.FromArgb(255, p, v, t);
            else if (hi == 3) return Color.FromArgb(255, p, q, v);
            else if (hi == 4) return Color.FromArgb(255, t, p, v);
            else return Color.FromArgb(255, v, p, q);
        }
        // 🌟 تابع لقط اللون المباشر من فوق مجسم المخروط ثلاثي الأبعاد
    public static Color GetColorAtPointDirect(Point mousePt, int width, int height, double angleX, double angleY, float zoom)
    {
        int cx = width / 2;
        int cy = height / 2;

        int segments = 32;
        PointF[] coneBasePts = new PointF[segments];
        double[] baseDepths = new double[segments];
        double radius = 50.0;

        // إعادة حساب نقاط القاعدة لمعرفة الأوجه
        for (int i = 0; i < segments; i++)
        {
            double angle = (i * 360.0 / segments) * Math.PI / 180.0;
            double x = radius * Math.Cos(angle);
            double y = radius * Math.Sin(angle);
            double z = 30.0;

            var proj = ProjectWithDepth(x, y, z, cx, cy, angleX, angleY, zoom);
            coneBasePts[i] = proj.Point;
            baseDepths[i] = proj.ZDepth;
        }

        var apexProj = ProjectWithDepth(0, 0, -50.0, cx, cy, angleX, angleY, zoom);
        PointF apexPt = apexProj.Point;

        // 1. أولاً: فحص إذا كانت النقرة داخل القاعدة العلوية الدائرية للمخروط
        using (GraphicsPath baseCircle = new GraphicsPath())
        {
            baseCircle.AddPolygon(coneBasePts);
            if (baseCircle.IsVisible(mousePt))
            {
                // حساب المركز التقريبي المسقط للقاعدة
                var centerProj = ProjectWithDepth(0, 0, 30.0, cx, cy, angleX, angleY, zoom);
                PointF centerPt = centerProj.Point;

                // حساب الاتجاه والمسافة لمعرفة الـ Hue والـ Saturation
                double dx = mousePt.X - centerPt.X;
                double dy = mousePt.Y - centerPt.Y;
                
                // حساب الزاوية وتحويلها إلى درجات [0, 360] لتمثيل الـ Hue
                double angleRad = Math.Atan2(dy, dx);
                double hue = angleRad * 180.0 / Math.PI;
                if (hue < 0) hue += 360.0;

                // تقدير نسبة البعد عن المركز لتمثيل الـ Saturation
                double maxEdgeDist = 0;
                for (int i = 0; i < segments; i++)
                {
                    double d = Math.Sqrt(Math.Pow(coneBasePts[i].X - centerPt.X, 2) + Math.Pow(coneBasePts[i].Y - centerPt.Y, 2));
                    maxEdgeDist += d;
                }
                maxEdgeDist /= segments; // متوسط نصف القطر المسقط على الشاشة

                double currentDist = Math.Sqrt(dx * dx + dy * dy);
                double sat = Math.Min(1.0, currentDist / maxEdgeDist);

                // بما أننا نضغط على السطح العلوي المستوي تماماً فـ القيمة اللونية (Value) دائماً كاملة 1.0
                return ColorFromHsv(hue, sat, 1.0);
            }
        }

        // 2. ثانياً: فحص إذا كانت النقرة على الأوجه الجانبية المائلة للمخروط
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            PointF[] sideTriangle = { coneBasePts[i], coneBasePts[next], apexPt };

            using (GraphicsPath sidePath = new GraphicsPath())
            {
                sidePath.AddPolygon(sideTriangle);
                if (sidePath.IsVisible(mousePt))
                {
                    // الـ Hue مشتق مباشرة من رقم الوجه الجانبي
                    double hue = i * 360.0 / segments;

                    // حساب الـ Value (السطوع) بناءً على مدى قرب النقرة من الرأس السفلي الأسود (Apex)
                    double totalHeight = Math.Sqrt(Math.Pow(coneBasePts[i].X - apexPt.X, 2) + Math.Pow(coneBasePts[i].Y - apexPt.Y, 2));
                    double clickDistFromApex = Math.Sqrt(Math.Pow(mousePt.X - apexPt.X, 2) + Math.Pow(mousePt.Y - apexPt.Y, 2));
                    
                    double val = Math.Min(1.0, clickDistFromApex / totalHeight);
                    // الأطراف الجانبية الخارجية للمخروط تمتلك دائماً إشباع كامل
                    double sat = 1.0; 

                    return ColorFromHsv(hue, sat, val);
                }
            }
        }

        return Color.Transparent;
    }
    }
}