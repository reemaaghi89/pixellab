using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pixellab
{
    public partial class Form1 : Form
    {
        Bitmap originalImage;
        Bitmap currentImage;

        // متغيرات هندسية للتحكم بمكعب الألوان ثلاثي الأبعاد وتدويره بالماوس
        private double angleX = 35.0; // زاوية الدوران الأفقي البدئية
        private double angleY = 45.0; // زاوية الدوران العمودي البدئية
        private Point lastMousePos;  // تتبع موقع الماوس الأخير للسحب
        private Color selectedPixelColor = Color.FromArgb(255, 255, 255); // لتحديد موضع النقطة داخل المكعب

        public Form1()
        {
            InitializeComponent();
            // تفعيل ميزة التخزين المؤقت المزدوج لمنع الوميض المزعج أثناء تدوير المكعب
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, 
                null, panelCube, new object[] { true });
        }

        private void btnopen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(ofd.FileName);
                currentImage = new Bitmap(originalImage);
                pictureBox1.Image = currentImage;

                System.IO.FileInfo file = new System.IO.FileInfo(ofd.FileName);

                lblInfo.Text = "Name: " + file.Name +
                               "\nSize: " + (file.Length / 1024) + " KB" +
                               "\nWidth: " + originalImage.Width +
                               "\nHeight: " + originalImage.Height;
                panelCube.Invalidate(); // إعادة رسم المكعب للتحديث
            }
        }

        private void ApplyRGB()
        {
            if (originalImage == null)
                return;

            Bitmap temp = new Bitmap(originalImage);

            for (int y = 0; y < temp.Height; y++)
            {
                for (int x = 0; x < temp.Width; x++)
                {
                    Color p = temp.GetPixel(x, y);
                    int r = chkRed.Checked ? 0 : p.R + trackRed.Value;
                    int g = chkGreen.Checked ? 0 : p.G + trackGreen.Value;
                    int b = chkBlue.Checked ? 0 : p.B + trackBlue.Value;

                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    temp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            currentImage = temp;
            pictureBox1.Image = currentImage;
        }

        private void trackRed_Scroll(object sender, EventArgs e) { ApplyRGB(); }
        private void trackGreen_Scroll(object sender, EventArgs e) { ApplyRGB(); }
        private void trackBlue_Scroll(object sender, EventArgs e) { ApplyRGB(); }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;

            currentImage = new Bitmap(originalImage);
            pictureBox1.Image = currentImage;

            trackRed.Value = 0;
            trackGreen.Value = 0;
            trackBlue.Value = 0;

            chkRed.Checked = false;
            chkGreen.Checked = false;
            chkBlue.Checked = false;
            selectedPixelColor = Color.FromArgb(255, 255, 255);
            panelCube.Invalidate();
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            if (currentImage == null) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG|*.png|JPEG|*.jpg|Bitmap|*.bmp";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                currentImage.Save(sfd.FileName);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
{
    if (pictureBox1.Image == null)
        return;

    // 1. جلب الأبعاد الحقيقية للصورة وأبعاد أداة العرض الكلية
    int imgWidth = pictureBox1.Image.Width;
    int imgHeight = pictureBox1.Image.Height;
    int pboxWidth = pictureBox1.Width;
    int pboxHeight = pictureBox1.Height;

    // 2. حساب نسبة التكبير الدقيقة لنمط العرض Zoom (المحافظ على أبعاد الصورة الأصلية)
    float ratioX = (float)pboxWidth / imgWidth;
    float ratioY = (float)pboxHeight / imgHeight;
    float ratio = Math.Min(ratioX, ratioY); // النسبة المعتمدة للعرض

    float itemWidth = imgWidth * ratio;
    float itemHeight = imgHeight * ratio;

    // حساب الهوامش الرمادية المحيطة بالصورة (Letterboxing) يميناً ويساراً أو فوق وتحت
    float leftMargin = (pboxWidth - itemWidth) / 2;
    float topMargin = (pboxHeight - itemHeight) / 2;

    // 3. تحويل إحداثيات النقرة الفأرية من أبعاد الأداة إلى رقم البكسل الحقيقي داخل مصفوفة الصورة
    int x = (int)((e.X - leftMargin) / ratio);
    int y = (int)((e.Y - topMargin) / ratio);

    // التأكد التام من أن النقرة وقعت داخل حدود الصورة الفعلية وليس في الهامش الرمادي المحيط
    if (x >= 0 && x < imgWidth && y >= 0 && y < imgHeight)
    {
        // إنشاء البيتماب بأمان لجلب البكسل المختار
        using (Bitmap bmp = new Bitmap(pictureBox1.Image))
        {
            Color c = bmp.GetPixel(x, y);
            selectedPixelColor = c; // تخزين اللون الحالي لإسقاطه ومزامنته بداخل مكعب الألوان

            // 4. مزامنة الـ Trackbars على اليمين لتتحرك وتتحدث تلقائياً مع لون البكسل المختار
            if (trackRed != null) trackRed.Value = c.R;
            if (trackGreen != null) trackGreen.Value = c.G;
            if (trackBlue != null) trackBlue.Value = c.B;

            // 5. استدعاء توابع التحويل الخاصة بكِ لجميع الأنظمة اللونية الأكاديمية المطلوبة
            double h, s, v;
            CustomColorConverter.RGBtoHSV(c, out h, out s, out v);

            double cyan, magenta, yellow, black;
            CustomColorConverter.RGBtoCMYK(c, out cyan, out magenta, out yellow, out black);

            double y_u, u, v_u;
            CustomColorConverter.RGBtoYUV(c, out y_u, out u, out v_u);

            double y_cb, cb, cr;
            CustomColorConverter.RGBtoYCbCr(c, out y_cb, out cb, out cr);

            double labL, labA, labB;
            CustomColorConverter.RGBtoLAB(c, out labL, out labA, out labB);

            // 6. بناء السلسلة النصية المنسقة للتقرير الشامل
            string info = "🎨 نتائج تحليل اللون ومزامنة الفضاءات:\n" +
                          "━━━━━━━━━━━━━━━━━━━━━\n" +
                          $"📍 الإحداثيات الحقيقية: X: {x}, Y: {y}\n\n" +
                          $"🔹 RGB:\n   R = {c.R}, G = {c.G}, B = {c.B}\n\n" +
                          $"🔹 HSV:\n   H = {h:0}°, S = {s * 100:0}%, V = {v * 100:0}%\n\n" +
                          $"🔹 CMYK (نظام الطباعة):\n   C = {cyan * 100:0}%, M = {magenta * 100:0}%, Y = {yellow * 100:0}%, K = {black * 100:0}%\n\n" +
                          $"🔹 LAB (فضاء إدراك العين البشرية الموحد):\n   L = {labL:0.0}, A = {labA:0.0}, B = {labB:0.0}\n\n" +
                          $"🔹 YUV (بث تماثلي): Y = {y_u:0.0}, U = {u:0.0}, V = {v_u:0.0}\n\n" +
                          $"🔹 YCbCr (المعيار الرقمي): Y = {y_cb:0.0}, Cb = {cb:0.0}, Cr = {cr:0.0}";

            // 7. تحديث لوحة رسم مكعب الألوان (panelCube) فوراً لجعل المؤشر المشع يقفز للموقع الجديد بالزمن الحقيقي
            panelCube.Invalidate();

            // عرض صندوق الرسائل للمستخدم
            MessageBox.Show(info, "Pixel Lab - Color Spaces", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

        private void chkRed_CheckedChanged(object sender, EventArgs e) { ApplyRGB(); }
        private void chkGreen_CheckedChanged(object sender, EventArgs e) { ApplyRGB(); }
        private void chkBlue_CheckedChanged(object sender, EventArgs e) { ApplyRGB(); }

        private void Form1_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            originalImage = new Bitmap(files[0]);
            currentImage = new Bitmap(originalImage);
            pictureBox1.Image = currentImage;
            panelCube.Invalidate();
        }

        private void btnGray_Click(object sender, EventArgs e)
        {
            if (currentImage == null) return;
            Bitmap temp = new Bitmap(currentImage);
            for (int y = 0; y < temp.Height; y++)
            {
                for (int x = 0; x < temp.Width; x++)
                {
                    Color p = temp.GetPixel(x, y);
                    int gray = (p.R + p.G + p.B) / 3;
                    temp.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            currentImage = temp;
            pictureBox1.Image = currentImage;
        }

        private void btnQuantize_Click(object sender, EventArgs e)
        {
            if (currentImage == null) return;
            Bitmap temp = new Bitmap(currentImage);
            int levels = 4;
            int step = 256 / levels;
            for (int y = 0; y < temp.Height; y++)
            {
                for (int x = 0; x < temp.Width; x++)
                {
                    Color p = temp.GetPixel(x, y);
                    int r = (p.R / step) * step;
                    int g = (p.G / step) * step;
                    int b = (p.B / step) * step;
                    temp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            currentImage = temp;
            pictureBox1.Image = currentImage;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            if (currentImage == null) return;
            Bitmap temp = new Bitmap(currentImage);
            for (int y = 0; y < temp.Height; y++)
            {
                for (int x = 0; x < temp.Width; x++)
                {
                    Color p = temp.GetPixel(x, y);
                    int gray = (int)(0.3 * p.R + 0.59 * p.G + 0.11 * p.B);
                    temp.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            currentImage = temp;
            pictureBox1.Image = currentImage;
        }

        // =========================================================================
        // قسم الرسم الهندسي المحترف لمكعب الألوان ثلاثي الأبعاد وإسقاطه التفاعلي متزامناً
        // =========================================================================

        private PointF Project3D(double x, double y, double z, int cx, int cy)
        {
            // تحويل الزوايا إلى راديان لاستخدامها برياضيات الدوال المثلثية
            double radX = angleX * Math.PI / 180.0;
            double radY = angleY * Math.PI / 180.0;

            // تطبيق مصفوفات الدوران الرياضية حول المحاور الإسقاطية
            double cosX = Math.Cos(radX), sinX = Math.Sin(radX);
            double cosY = Math.Cos(radY), sinY = Math.Sin(radY);

            double rY = y * cosX - z * sinX;
            double rZ = y * sinX + z * cosX;

            double rX = x * cosY + rZ * sinY;

            // إسقاط النقطة هندسياً على مستوى الشاشة ثنائي الأبعاد في منتصف البانل
            float screenX = (float)(cx + rX * 0.4);
            float screenY = (float)(cy - rY * 0.4);

            return new PointF(screenX, screenY);
        }

        private void panelCube_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(25, 25, 25)); // خلفية داكنة جداً لتبرز الألوان بشكل ساحر

            int cx = panelCube.Width / 2;
            int cy = panelCube.Height / 2;

            // 1. تعريف الرؤوس الثمانية للمكعب في الفضاء ثلاثي الأبعاد
            double[,] vertices = {
                {-100, -100, -100}, // 0: أسود (Black)
                {100, -100, -100},  // 1: أحمر (Red)
                {100, 100, -100},   // 2: أصفر (Yellow)
                {-100, 100, -100},  // 3: أخضر (Green)
                {-100, -100, 100},  // 4: أزرق (Blue)
                {100, -100, 100},   // 5: ماجنتا (Magenta)
                {100, 100, 100},    // 6: أبيض (White)
                {-100, 100, 100}    // 7: سيان (Cyan)
            };

            // 2. مصفوفة الألوان الحقيقية المقابلة لكل رأس (أساس فضاء RGB)
            Color[] vertexColors = {
                Color.Black, Color.Red, Color.Yellow, Color.Green,
                Color.Blue, Color.Magenta, Color.White, Color.Cyan
            };

            // إسقاط النقاط من 3D إلى 2D على الشاشة
            PointF[] pts = new PointF[8];
            for (int i = 0; i < 8; i++)
            {
                pts[i] = Project3D(vertices[i, 0], vertices[i, 1], vertices[i, 2], cx, cy);
            }

            // ترتيب الحواف وتقسيمها حسب محاور الألوان (X=Red, Y=Green, Z=Blue)
            int[,] edges = {
                {0,1}, {3,2}, {4,5}, {7,6}, // حواف موازية لمحور اللون الأحمر
                {1,2}, {0,3}, {5,6}, {4,7}, // حواف موازية لمحور اللون الأخضر
                {0,4}, {1,5}, {2,6}, {3,7}  // حواف موازية لمحور اللون الأزرق
            };

            // 3. رسم الحواف ملونة بحسب المحور اللوني لتوضيح الفضاء الأكاديمي
            for (int i = 0; i < 12; i++)
            {
                Color edgeColor;
                if (i < 4) edgeColor = Color.FromArgb(200, 255, 70, 70);       // خطوط حمراء لمحور R
                else if (i < 8) edgeColor = Color.FromArgb(200, 70, 255, 70);  // خطوط خضراء لمحور G
                else edgeColor = Color.FromArgb(200, 70, 70, 255);             // خطوط زرقاء لمحور B

                using (Pen p = new Pen(edgeColor, 2f))
                {
                    g.DrawLine(p, pts[edges[i, 0]], pts[edges[i, 1]]);
                }
            }

            // 4. رسم رؤوس المكعب ككرات ملونة زاهية تُمثّل الألوان الأساسية والثانوية لـ RGB
            for (int i = 0; i < 8; i++)
            {
                using (Brush b = new SolidBrush(vertexColors[i]))
                {
                    g.FillEllipse(b, pts[i].X - 6, pts[i].Y - 6, 12, 12);
                }
                using (Pen p = new Pen(Color.White, 1f)) // إطار أبيض ناعم لكل رأس
                {
                    g.DrawEllipse(p, pts[i].X - 6, pts[i].Y - 6, 12, 12);
                }
            }

            // 5. حساب وإسقاط موقع البكسل المحدد حالياً داخل المكعب اللوني
            double pX = ((selectedPixelColor.R / 255.0) * 200.0) - 100.0;
            double pY = ((selectedPixelColor.G / 255.0) * 200.0) - 100.0;
            double pZ = ((selectedPixelColor.B / 255.0) * 200.0) - 100.0;

            PointF selectedTargetPt = Project3D(pX, pY, pZ, cx, cy);

            // 6. رسم "المؤشر التفاعلي المشع" (يأخذ نفس لون البكسل المختار تماماً ويكون ضخماً)
            using (Brush b = new SolidBrush(selectedPixelColor))
            {
                g.FillEllipse(b, selectedTargetPt.X - 8, selectedTargetPt.Y - 8, 16, 16);
            }
            using (Pen goldPen = new Pen(Color.Gold, 2.5f)) // طوق ذهبي لامع ليميز البكسل الحالي عن زوايا المكعب
            {
                g.DrawEllipse(goldPen, selectedTargetPt.X - 10, selectedTargetPt.Y - 10, 20, 20);
            }

            // نص الدليل على الواجهة
            g.DrawString("فضاء RGB ثلاثي الأبعاد - اسحب للتدوير", this.Font, Brushes.White, 5, 5);
        }

        private void panelCube_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) lastMousePos = e.Location;
        }

        private void panelCube_MouseMove(object sender, MouseEventArgs e)
        {
            // عند قيام المستخدم بالسحب بالماوس فوق أداة الرسم يتم تعديل زوايا الإسقاط فوراً بالزمن الحقيقي
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.X - lastMousePos.X;
                int dy = e.Y - lastMousePos.Y;

                angleY += dx * 0.5;
                angleX += dy * 0.5;

                lastMousePos = e.Location;
                panelCube.Invalidate(); // طلب إعادة رسم فوري لتوفير حركة تدوير ناعمة للغاية ومستمرة
            }
        }
    }
}