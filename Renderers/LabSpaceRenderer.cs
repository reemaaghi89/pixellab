using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using pixellab.Converters;

namespace pixellab.Renderers
{
    public static class LabRenderer
    {
        private const int L_Steps = 15;   
        private const int Segments = 30;  

        private class LabFace
        {
            public PointF[] Points { get; set; }
            public Color FaceColor { get; set; }
            public double AverageDepth { get; set; }

            public (double L, double a, double b)[] LabVertices { get; set; }
        }

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

            return (new PointF((float)(cx + rX * zoom), (float)(cy - rY * zoom)), depthZ);
        }

        public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom, Color selectedColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int cx = width / 2;
            int cy = height / 2;

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

                    var proj0 = ProjectWithDepth(l1.a, l1.L - 50, l1.b, cx, cy, angleX, angleY, zoom);
                    var proj1 = ProjectWithDepth(l2.a, l2.L - 50, l2.b, cx, cy, angleX, angleY, zoom);
                    var proj2 = ProjectWithDepth(l3.a, l3.L - 50, l3.b, cx, cy, angleX, angleY, zoom);
                    var proj3 = ProjectWithDepth(l4.a, l4.L - 50, l4.b, cx, cy, angleX, angleY, zoom);

                    double avgDepth = (proj0.ZDepth + proj1.ZDepth + proj2.ZDepth + proj3.ZDepth) / 4.0;

                    Color faceColor = LabConverter.ToRgb((l1.L + l3.L) / 2, (l1.a + l3.a) / 2, (l1.b + l3.b) / 2);

                    facesList.Add(new LabFace
                    {
                        Points = new PointF[] { proj0.Point, proj1.Point, proj2.Point, proj3.Point },
                        FaceColor = faceColor,
                        AverageDepth = avgDepth,
                        LabVertices = new (double L, double a, double b)[] { l1, l2, l3, l4 }
                    });
                    var lab = LabConverter.FromRgb(selectedColor);

                  
                    double radius = Math.Sin(lab.L / 100.0 * Math.PI) * 80.0;
                    double angle = Math.Atan2(lab.B, lab.A);

                    double xPos = radius * Math.Cos(angle);
                    double zPos = radius * Math.Sin(angle);
                    double yPos = lab.L - 50; 

                    var target = ProjectWithDepth(xPos, yPos, zPos, cx, cy, angleX, angleY, zoom);

                }
            }

            facesList.Sort((a, b) => a.AverageDepth.CompareTo(b.AverageDepth));

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

            for (int i = 0; i < L_Steps; i++)
            {
                for (int j = 0; j < Segments; j++)
                {
                    int nextJ = (j + 1) % Segments;
                    
                    var l1 = GetLabAt(i, j); 
                    var l2 = GetLabAt(i, nextJ);
                    var l3 = GetLabAt(i + 1, nextJ);
                    var l4 = GetLabAt(i + 1, j);

                    var p0 = ProjectWithDepth(l1.a, l1.L - 50, l1.b, cx, cy, angleX, angleY, zoom);
                    var p1 = ProjectWithDepth(l2.a, l2.L - 50, l2.b, cx, cy, angleX, angleY, zoom);
                    var p2 = ProjectWithDepth(l3.a, l3.L - 50, l3.b, cx, cy, angleX, angleY, zoom);
                    var p3 = ProjectWithDepth(l4.a, l4.L - 50, l4.b, cx, cy, angleX, angleY, zoom);

                    if (IsMouseInTriangle(mousePt, p0.Point, p1.Point, p2.Point))
                    {
                        return LabConverter.ToRgb((l1.L + l2.L + l3.L)/3, (l1.a + l2.a + l3.a)/3, (l1.b + l2.b + l3.b)/3);
                    }
                    
                    if (IsMouseInTriangle(mousePt, p0.Point, p2.Point, p3.Point))
                    {
                        return LabConverter.ToRgb((l1.L + l3.L + l4.L)/3, (l1.a + l3.a + l4.a)/3, (l1.b + l3.b + l4.b)/3);
                    }
                }
            }
            return Color.Transparent;
        }

        private static bool IsMouseInTriangle(Point p, PointF a, PointF b, PointF c)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(new PointF[] { a, b, c });
                return path.IsVisible(p);
            }
        }
      
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