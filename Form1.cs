using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pixellab.Converters;
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

        // استدعاء المعالجة السريعة بالزمن الحقيقي لقنوات RGB بدون تعليق البرنامج
        
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
        private void btnOpen3DLab_Click(object sender, EventArgs e)
        {
            FormSpaces spacesForm = new FormSpaces();
            spacesForm.ShowDialog();
        }
        
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

        private void pictureBox1_Click(object sender, EventArgs e) { }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null || currentImage == null) return;

            // 🌟 استدعاء السطر السحري الشامل من ملف التحليل الخارجي
            var (selectedColor, infoReport) = ColorAnalyzer.AnalyzeClickedPixel(currentImage, pictureBox1.Width, pictureBox1.Height, e);

            // إذا نجح التحليل والتقطنا لوناً حقيقياً داخل حدود الصورة
            if (selectedColor.HasValue)
            {
                Color pixelColor = selectedColor.Value;
                selectedPixelColor = pixelColor; // تحديث اللون العام للمكعب

                // مزامنة الـ Trackbars على اليمين تلقائياً
                if (trackRed != null) trackRed.Value = pixelColor.R;
                if (trackGreen != null) trackGreen.Value = pixelColor.G;
                if (trackBlue != null) trackBlue.Value = pixelColor.B;

                // تحديث لوحة رسم مكعب الألوان فوراً ليتحرك المؤشر المشع
                panelCube.Invalidate();

                // عرض صندوق الرسائل بالنص المولد جاهزاً
                MessageBox.Show(infoReport, "Pixel Lab - Color Spaces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void Form1_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            originalImage = new Bitmap(files[0]);
            currentImage = new Bitmap(originalImage);
            pictureBox1.Image = currentImage;
            panelCube.Invalidate();
        }

        //====================================================================================
        //====================================================================================
        // حدث النقر على زر تقليل عدد الألوان (تكميم الصورة)
        private void btnQuantizeColors_Click(object sender, EventArgs e)
        {
            // التأكد من وجود صورة محملة في مساحة العمل أولاً
            if (currentImage == null) return;

            // استدعاء تابع تقليل الألوان السريع من كلاس المعالجة بـ 4 مستويات لونية
            Bitmap quantizedResult = ImageProcessor.QuantizeImageColors((Bitmap)currentImage, colorLevelsCount: 4);

            // تحديث الصورة وعرضها على الواجهة الرسومية فوراً
            currentImage = quantizedResult;
            pictureBox1.Image = currentImage;
        }

        // حدث النقر على زر تحويل الصورة إلى رمادي
        private void btnConvertGrayscale_Click(object sender, EventArgs e)
        {
            if (currentImage == null) return;
            // استدعاء تابع الرمادي السريع
            currentImage = ImageProcessor.ConvertToGrayscale((Bitmap)currentImage);
            pictureBox1.Image = currentImage;
        }

        private void btnConvertBlackAndWhite_Click(object sender, EventArgs e)
        {
            if (currentImage == null) return;

            // استدعاء تابع الأبيض والأسود الصافي الحقيقي
            currentImage = ImageProcessor.ConvertToBlackAndWhite((Bitmap)currentImage);
            pictureBox1.Image = currentImage;
        }

        private void ApplyRGB()
        {
            if (originalImage == null) return;

            // نرسل الخيارات مباشرة لملف المعالجة الخارجي المحترف
            currentImage = ChannelProcessor.AdjustRgbChannels(
                originalImage,
                chkRed.Checked, trackRed.Value,
                chkGreen.Checked, trackGreen.Value,
                chkBlue.Checked, trackBlue.Value
            );

            // عرض النتيجة فوراً على الواجهة
            pictureBox1.Image = currentImage;
        }

        private void trackRed_Scroll(object sender, EventArgs e) { ApplyRGB(); }
        private void trackGreen_Scroll(object sender, EventArgs e) { ApplyRGB(); }
        private void trackBlue_Scroll(object sender, EventArgs e) { ApplyRGB(); }

        private void chkRed_CheckedChanged(object sender, EventArgs e) { ApplyRGB(); }
        private void chkGreen_CheckedChanged(object sender, EventArgs e) { ApplyRGB(); }
        private void chkBlue_CheckedChanged(object sender, EventArgs e) { ApplyRGB(); }

        // =========================================================================
        // قسم الرسم الهندسي المحترف لمكعب الألوان ثلاثي الأبعاد وإسقاطه التفاعلي متزامناً
        // =========================================================================

    }
}