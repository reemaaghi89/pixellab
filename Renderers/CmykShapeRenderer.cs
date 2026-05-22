using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using pixellab.Converters;

namespace pixellab.Renderers
{
    public static class CmykRenderer
    {
        private const int MeshSteps = 10; 

        private class CmykFace
        {
            public PointF[] Points { get; set; }
            public Color FaceColor { get; set; }
            public double AverageDepth { get; set; }
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

        public static void Render(Graphics g, int width, int height, double angleX, double angleY, float zoom)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int cx = width / 2;
            int cy = height / 2;

            List<CmykFace> facesList = GenerateCmykMesh(cx, cy, angleX, angleY, zoom);

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
                    using (Pen p = new Pen(Color.FromArgb(25, Color.White), 0.5f))
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

            List<CmykFace> facesList = GenerateCmykMesh(cx, cy, angleX, angleY, zoom);
            facesList.Sort((a, b) => a.AverageDepth.CompareTo(b.AverageDepth));

            for (int k = facesList.Count - 1; k >= 0; k--)
            {
                var face = facesList[k];
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(face.Points);
                    if (path.IsVisible(mousePt))
                    {
                        return face.FaceColor;
                    }
                }
            }
            return Color.Transparent;
        }

        private static List<CmykFace> GenerateCmykMesh(int cx, int cy, double angleX, double angleY, float zoom)
        {
            List<CmykFace> list = new List<CmykFace>();

            for (int dim = 0; dim < 3; dim++)
            {
                for (int fixedVal = 0; fixedVal <= 1; fixedVal++)
                {
                    double fixedCmyk = fixedVal;

                    for (int i = 0; i < MeshSteps; i++)
                    {
                        for (int j = 0; j < MeshSteps; j++)
                        {
                            double u1 = (double)i / MeshSteps;
                            double u2 = (double)(i + 1) / MeshSteps;
                            double v1 = (double)j / MeshSteps;
                            double v2 = (double)(j + 1) / MeshSteps;

                            var cmyk00 = GetFaceCmyk(dim, fixedCmyk, u1, v1);
                            var cmyk10 = GetFaceCmyk(dim, fixedCmyk, u2, v1);
                            var cmyk11 = GetFaceCmyk(dim, fixedCmyk, u2, v2);
                            var cmyk01 = GetFaceCmyk(dim, fixedCmyk, u1, v2);

                            var p00 = ProjectWithDepth(cmyk00.C * 100 - 50, cmyk00.M * 100 - 50, cmyk00.Y * 100 - 50, cx, cy, angleX, angleY, zoom);
                            var p10 = ProjectWithDepth(cmyk10.C * 100 - 50, cmyk10.M * 100 - 50, cmyk10.Y * 100 - 50, cx, cy, angleX, angleY, zoom);
                            var p11 = ProjectWithDepth(cmyk11.C * 100 - 50, cmyk11.M * 100 - 50, cmyk11.Y * 100 - 50, cx, cy, angleX, angleY, zoom);
                            var p01 = ProjectWithDepth(cmyk01.C * 100 - 50, cmyk01.M * 100 - 50, cmyk01.Y * 100 - 50, cx, cy, angleX, angleY, zoom);

                            double avgDepth = (p00.ZDepth + p10.ZDepth + p11.ZDepth + p01.ZDepth) / 4.0;

                            double midC = (cmyk00.C + cmyk11.C) / 2.0;
                            double midM = (cmyk00.M + cmyk11.M) / 2.0;
                            double midY = (cmyk00.Y + cmyk11.Y) / 2.0;
                            Color faceColor = CmykConverter.ToRgb(midC, midM, midY, 0);

                            list.Add(new CmykFace
                            {
                                Points = new PointF[] { p00.Point, p10.Point, p11.Point, p01.Point },
                                FaceColor = faceColor,
                                AverageDepth = avgDepth
                            });
                        }
                    }
                }
            }
            return list;
        }

        private static (double C, double M, double Y) GetFaceCmyk(int dim, double fixedVal, double u, double v)
        {
            if (dim == 0) return (fixedVal, u, v); 
            if (dim == 1) return (u, fixedVal, v); 
            return (u, v, fixedVal);               
        }
    }
}