using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace pixellab.Renderers
{
    public static class HsvConeRenderer
    {
        private const int Segments = 36; 

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

        private static PointF Project(double x, double y, double z, int cx, int cy, double angleX, double angleY, float zoom)
        {
            return ProjectWithDepth(x, y, z, cx, cy, angleX, angleY, zoom).Point;
        }

        public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom, Color selectedColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = width / 2;
            int cy = height / 2;

          
            double[,] vertices = new double[2 + Segments, 3];
            vertices[0, 0] = 0;  vertices[0, 1] = -50; vertices[0, 2] = 0;  
            vertices[1, 0] = 0;  vertices[1, 1] = 50;  vertices[1, 2] = 0; 

            Color[] vertexColors = new Color[2 + Segments];
            vertexColors[0] = Color.Black;
            vertexColors[1] = Color.White;

            for (int i = 0; i < Segments; i++)
            {
                double angleDeg = i * (360.0 / Segments);
                double rad = angleDeg * Math.PI / 180.0;

                vertices[2 + i, 0] = 50 * Math.Cos(rad);
                vertices[2 + i, 1] = 50;
                vertices[2 + i, 2] = 50 * Math.Sin(rad);

                vertexColors[2 + i] = HsvToRgb(angleDeg, 1.0, 1.0);
            }

            PointF[] pts = new PointF[2 + Segments];
            double[] vertexDepths = new double[2 + Segments];

            for (int i = 0; i < 2 + Segments; i++)
            {
                var projection = ProjectWithDepth(vertices[i, 0], vertices[i, 1], vertices[i, 2], cx, cy, angleX, angleY, zoom);
                pts[i] = projection.Point;
                vertexDepths[i] = projection.ZDepth;
            }

            List<ConeFace> faces = new List<ConeFace>();
            for (int i = 0; i < Segments; i++)
            {
                int next = (i + 1) % Segments;

                faces.Add(new ConeFace
                {
                    V0 = 0, V1 = 2 + i, V2 = 2 + next,
                    AverageDepth = (vertexDepths[0] + vertexDepths[2 + i] + vertexDepths[2 + next]) / 3.0
                });

                faces.Add(new ConeFace
                {
                    V0 = 1, V1 = 2 + i, V2 = 2 + next,
                    AverageDepth = (vertexDepths[1] + vertexDepths[2 + i] + vertexDepths[2 + next]) / 3.0
                });
            }

            faces.Sort((a, b) => a.AverageDepth.CompareTo(b.AverageDepth));

            foreach (var face in faces)
            {
                PointF[] facePoints = { pts[face.V0], pts[face.V1], pts[face.V2] };
                Color[] faceColors = { vertexColors[face.V0], vertexColors[face.V1], vertexColors[face.V2] };

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(facePoints);
                    using (PathGradientBrush pgb = new PathGradientBrush(path))
                    {
                        pgb.CenterColor = Color.FromArgb(
                            (faceColors[0].R + faceColors[1].R + faceColors[2].R) / 3,
                            (faceColors[0].G + faceColors[1].G + faceColors[2].G) / 3,
                            (faceColors[0].B + faceColors[1].B + faceColors[2].B) / 3
                        );
                        pgb.SurroundColors = faceColors;
                        g.FillPolygon(pgb, facePoints);
                    }
                }

                using (Pen p = new Pen(Color.FromArgb(25, Color.White), 1f))
                {
                    g.DrawPolygon(p, facePoints);
                }
            }

            ColorToHsv(selectedColor, out double h, out double s, out double v);
            double pY = (v * 100.0) - 50.0;
            double currentRadius = s * v * 50.0; 
            double radH = h * Math.PI / 180.0;
            double pX = currentRadius * Math.Cos(radH);
            double pZ = currentRadius * Math.Sin(radH);

            PointF targetPt = Project(pX, pY, pZ, cx, cy, angleX, angleY, zoom);

            using (Brush b = new SolidBrush(selectedColor)) g.FillEllipse(b, targetPt.X - 7, targetPt.Y - 7, 14, 14);
            using (Pen goldPen = new Pen(Color.Gold, 2f)) g.DrawEllipse(goldPen, targetPt.X - 9, targetPt.Y - 9, 18, 18);
        }

        public static Color GetColorAtPointDirect(Point mousePt, int width, int height, double angleX, double angleY, float zoom)
        {
            int cx = width / 2;
            int cy = height / 2;

            double[,] vertices = new double[2 + Segments, 3];
            vertices[0, 0] = 0;  vertices[0, 1] = -50; vertices[0, 2] = 0;
            vertices[1, 0] = 0;  vertices[1, 1] = 50;  vertices[1, 2] = 0;

            for (int i = 0; i < Segments; i++)
            {
                double angleDeg = i * (360.0 / Segments);
                double rad = angleDeg * Math.PI / 180.0;
                vertices[2 + i, 0] = 50 * Math.Cos(rad);
                vertices[2 + i, 1] = 50;
                vertices[2 + i, 2] = 50 * Math.Sin(rad);
            }

            PointF[] pts = new PointF[2 + Segments];
            double[] vertexDepths = new double[2 + Segments];

            for (int i = 0; i < 2 + Segments; i++)
            {
                var projection = ProjectWithDepth(vertices[i, 0], vertices[i, 1], vertices[i, 2], cx, cy, angleX, angleY, zoom);
                pts[i] = projection.Point;
                vertexDepths[i] = projection.ZDepth;
            }

            List<ConeFace> faces = new List<ConeFace>();
            for (int i = 0; i < Segments; i++)
            {
                int next = (i + 1) % Segments;
                faces.Add(new ConeFace { V0 = 0, V1 = 2 + i, V2 = 2 + next, AverageDepth = (vertexDepths[0] + vertexDepths[2 + i] + vertexDepths[2 + next]) / 3.0 });
                faces.Add(new ConeFace { V0 = 1, V1 = 2 + i, V2 = 2 + next, AverageDepth = (vertexDepths[1] + vertexDepths[2 + i] + vertexDepths[2 + next]) / 3.0 });
            }

            faces.Sort((a, b) => a.AverageDepth.CompareTo(b.AverageDepth));

            for (int j = faces.Count - 1; j >= 0; j--)
            {
                var face = faces[j];
                PointF p0 = pts[face.V0], p1 = pts[face.V1], p2 = pts[face.V2];

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(new PointF[] { p0, p1, p2 });

                    if (path.IsVisible(mousePt))
                    {
                        CalculateBilinearTriangleRatio(mousePt, p0, p1, p2, out double w0, out double w1, out double w2);

                        double x3D = w0 * vertices[face.V0, 0] + w1 * vertices[face.V1, 0] + w2 * vertices[face.V2, 0];
                        double y3D = w0 * vertices[face.V0, 1] + w1 * vertices[face.V1, 1] + w2 * vertices[face.V2, 1];
                        double z3D = w0 * vertices[face.V0, 2] + w1 * vertices[face.V1, 2] + w2 * vertices[face.V2, 2];

                        double v = (y3D + 50.0) / 100.0;
                        v = Math.Max(0.0, Math.Min(1.0, v));

                        double s = 0;
                        double h = 0;

                        if (v > 0.001)
                        {
                            double maxRadiusAtHeight = v * 50.0;
                            double actualRadius = Math.Sqrt(x3D * x3D + z3D * z3D);
                            s = actualRadius / maxRadiusAtHeight;
                            s = Math.Max(0.0, Math.Min(1.0, s));

                            double angleRad = Math.Atan2(z3D, x3D);
                            h = angleRad * (180.0 / Math.PI);
                            if (h < 0) h += 360.0;
                        }

                        return HsvToRgb(h, s, v);
                    }
                }
            }

            return Color.Transparent;
        }

        private static void CalculateBilinearTriangleRatio(Point p, PointF a, PointF b, PointF c, out double w0, out double w1, out double w2)
        {
            double denominator = (b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y);
            if (Math.Abs(denominator) < 0.000001)
            {
                w0 = 1.0 / 3.0; w1 = 1.0 / 3.0; w2 = 1.0 / 3.0;
                return;
            }

            w0 = ((b.Y - c.Y) * (p.X - c.X) + (c.X - b.X) * (p.Y - c.Y)) / denominator;
            w1 = ((c.Y - a.Y) * (p.X - c.X) + (a.X - c.X) * (p.Y - c.Y)) / denominator;
            w2 = 1.0 - w0 - w1;
        }

        private static Color HsvToRgb(double h, double s, double v)
        {
            double r = 0, g = 0, b = 0;
            if (s == 0)
            {
                r = v; g = v; b = v;
            }
            else
            {
                double sectorPos = h / 60.0;
                int sectorNumber = (int)Math.Floor(sectorPos);
                double fractionalSector = sectorPos - sectorNumber;

                double p = v * (1.0 - s);
                double q = v * (1.0 - (s * fractionalSector));
                double t = v * (1.0 - (s * (1.0 - fractionalSector)));

                switch (sectorNumber % 6)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    case 5: r = v; g = p; b = q; break;
                }
            }
            return Color.FromArgb(
                Math.Max(0, Math.Min(255, (int)Math.Round(r * 255.0))),
                Math.Max(0, Math.Min(255, (int)Math.Round(g * 255.0))),
                Math.Max(0, Math.Min(255, (int)Math.Round(b * 255.0)))
            );
        }

        private static void ColorToHsv(Color color, out double hue, out double sat, out double val)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            sat = (max == 0) ? 0 : 1d - (1d * min / max);
            val = max / 255d;
        }

        private class ConeFace
        {
            public int V0 { get; set; }
            public int V1 { get; set; }
            public int V2 { get; set; }
            public double AverageDepth { get; set; }
        }
    }
}