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

            double[,] rgbVertices = {
                {0, 0, 0},       
                {255, 0, 0},    
                {255, 255, 0},   
                {0, 255, 0},     
                {0, 0, 255},     
                {255, 0, 255},   
                {255, 255, 255},
                {0, 255, 255}    
            };

            double[,] vertices = new double[8, 3];
            PointF[] pts = new PointF[8];
            double[] vertexDepths = new double[8];

            for (int i = 0; i < 8; i++)
            {
                var ycbcr = RgbToYCbCr(rgbVertices[i, 0], rgbVertices[i, 1], rgbVertices[i, 2]);
                
                vertices[i, 0] = ((ycbcr.Cb / 255.0) * 100.0) - 50.0;
                vertices[i, 1] = ((ycbcr.Y / 255.0) * 100.0) - 50.0;
                vertices[i, 2] = ((ycbcr.Cr / 255.0) * 100.0) - 50.0;

                var projection = ProjectWithDepth(vertices[i, 0], vertices[i, 1], vertices[i, 2], cx, cy, angleX, angleY, zoom);
                pts[i] = projection.Point;
                vertexDepths[i] = projection.ZDepth;
            }

            int[][] faces = {
                new int[] {0, 1, 2, 3}, 
                new int[] {4, 5, 6, 7}, 
                new int[] {0, 1, 5, 4}, 
                new int[] {2, 3, 7, 6}, 
                new int[] {0, 3, 7, 4}, 
                new int[] {1, 2, 6, 5}  
            };

            Color[][] faceGradientColors = {
                new Color[] { Color.Black, Color.Red, Color.Yellow, Color.Green },     
                new Color[] { Color.Blue, Color.Magenta, Color.White, Color.Cyan },    
                new Color[] { Color.Black, Color.Red, Color.Magenta, Color.Blue },     
                new Color[] { Color.Yellow, Color.Green, Color.Cyan, Color.White },    
                new Color[] { Color.Black, Color.Green, Color.Cyan, Color.Blue },     
                new Color[] { Color.Red, Color.Yellow, Color.White, Color.Magenta }    
            };

            List<int> faceIndices = new List<int> { 0, 1, 2, 3, 4, 5 };
            faceIndices.Sort((a, b) => {
                double depthA = (vertexDepths[faces[a][0]] + vertexDepths[faces[a][1]] + vertexDepths[faces[a][2]] + vertexDepths[faces[a][3]]) / 4.0;
                double depthB = (vertexDepths[faces[b][0]] + vertexDepths[faces[b][1]] + vertexDepths[faces[b][2]] + vertexDepths[faces[b][3]]) / 4.0;
                return depthA.CompareTo(depthB);
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

                using (Pen p = new Pen(Color.FromArgb(50, Color.White), 1f))
                {
                    g.DrawPolygon(p, facePoints);
                }
            }

            var selYCbCr = RgbToYCbCr(selectedColor.R, selectedColor.G, selectedColor.B);
            double pX = ((selYCbCr.Cb / 255.0) * 100.0) - 50.0;
            double pY = ((selYCbCr.Y / 255.0) * 100.0) - 50.0;
            double pZ = ((selYCbCr.Cr / 255.0) * 100.0) - 50.0;

            PointF targetPt = Project(pX, pY, pZ, cx, cy, angleX, angleY, zoom);

            using (Brush b = new SolidBrush(selectedColor)) g.FillEllipse(b, targetPt.X - 7, targetPt.Y - 7, 14, 14);
            using (Pen goldPen = new Pen(Color.Gold, 2f)) g.DrawEllipse(goldPen, targetPt.X - 9, targetPt.Y - 9, 18, 18);
        }

        public static Color GetColorAtPointDirect(Point mousePt, int width, int height, double angleX, double angleY, float zoom)
        {
            int cx = width / 2;
            int cy = height / 2;

            double[,] rgbVertices = {
                {0, 0, 0}, {255, 0, 0}, {255, 255, 0}, {0, 255, 0},
                {0, 0, 255}, {255, 0, 255}, {255, 255, 255}, {0, 255, 255}
            };

            double[,] vertices = new double[8, 3];
            PointF[] pts = new PointF[8];
            double[] vertexDepths = new double[8];

            for (int i = 0; i < 8; i++)
            {
                var ycbcr = RgbToYCbCr(rgbVertices[i, 0], rgbVertices[i, 1], rgbVertices[i, 2]);
                vertices[i, 0] = ((ycbcr.Cb / 255.0) * 100.0) - 50.0;
                vertices[i, 1] = ((ycbcr.Y / 255.0) * 100.0) - 50.0;
                vertices[i, 2] = ((ycbcr.Cr / 255.0) * 100.0) - 50.0;

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

            for (int j = faceIndices.Count - 1; j >= 0; j--)
            {
                int i = faceIndices[j];
                PointF p0 = pts[faces[i][0]], p1 = pts[faces[i][1]], p2 = pts[faces[i][2]], p3 = pts[faces[i][3]];

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(new PointF[] { p0, p1, p2, p3 });

                    if (path.IsVisible(mousePt))
                    {
                        double u = CalculateAdvancedBilinearRatio(mousePt, p0, p1, p2, p3, out double v);
                        u = Math.Max(0.0, Math.Min(1.0, u));
                        v = Math.Max(0.0, Math.Min(1.0, v));

                        double[,] faceVerts3D = {
                            { vertices[faces[i][0], 0], vertices[faces[i][0], 1], vertices[faces[i][0], 2] },
                            { vertices[faces[i][1], 0], vertices[faces[i][1], 1], vertices[faces[i][1], 2] },
                            { vertices[faces[i][2], 0], vertices[faces[i][2], 1], vertices[faces[i][2], 2] },
                            { vertices[faces[i][3], 0], vertices[faces[i][3], 1], vertices[faces[i][3], 2] }
                        };

                        double x3D = (1 - u) * ((1 - v) * faceVerts3D[0, 0] + v * faceVerts3D[3, 0]) + u * ((1 - v) * faceVerts3D[1, 0] + v * faceVerts3D[2, 0]);
                        double y3D = (1 - u) * ((1 - v) * faceVerts3D[0, 1] + v * faceVerts3D[3, 1]) + u * ((1 - v) * faceVerts3D[1, 1] + v * faceVerts3D[2, 1]);
                        double z3D = (1 - u) * ((1 - v) * faceVerts3D[0, 2] + v * faceVerts3D[3, 2]) + u * ((1 - v) * faceVerts3D[1, 2] + v * faceVerts3D[2, 2]);

                        double cb = ((x3D + 50.0) / 100.0) * 255.0;
                        double y    = ((y3D + 50.0) / 100.0) * 255.0;
                        double cr = ((z3D + 50.0) / 100.0) * 255.0;

                        return YCbCrToRgb(y, cb, cr);
                    }
                }
            }
            return Color.Transparent;
        }

        private static double CalculateAdvancedBilinearRatio(Point pt, PointF p0, PointF p1, PointF p2, PointF p3, out double v)
        {
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

        private static (double Y, double Cb, double Cr) RgbToYCbCr(double r, double g, double b)
        {
            double y = 0.299 * r + 0.587 * g + 0.114 * b;
            double cb = 128.0 - 0.168736 * r - 0.331264 * g + 0.5 * b;
            double cr = 128.0 + 0.5 * r - 0.418688 * g - 0.081312 * b;
            return (y, cb, cr);
        }

        private static Color YCbCrToRgb(double y, double cb, double cr)
        {
            double r = y + 1.402 * (cr - 128.0);
            double g = y - 0.344136 * (cb - 128.0) - 0.714136 * (cr - 128.0);
            double b = y + 1.772 * (cb - 128.0);

            return Color.FromArgb(
                Math.Max(0, Math.Min(255, (int)Math.Round(r))),
                Math.Max(0, Math.Min(255, (int)Math.Round(g))),
                Math.Max(0, Math.Min(255, (int)Math.Round(b)))
            );
        }
    }
}