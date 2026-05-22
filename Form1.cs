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
using System.IO;
using System.Drawing.Drawing2D;
namespace pixellab
{
    public partial class Form1 : Form
    {
        private Bitmap originalImage;
        private Bitmap currentImage;

        private double angleX = 35.0; // زاوية الدوران الأفقي البدئية
        private double angleY = 45.0; // زاوية الدوران العمودي البدئية
        private Point lastMousePos;  // تتبع موقع الماوس الأخير للسحب
        private Color selectedPixelColor = Color.FromArgb(255, 255, 255); // لتحديد موضع النقطة داخل المكعب

        public Form1()
        {
            InitializeComponent();
            ApplyDarkTheme();
            // تفعيل ميزة التخزين المؤقت المزدوج لمنع الوميض المزعج أثناء تدوير المكعب
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, panelCube, new object[] { true });
        }

        private void ApplyDarkTheme()
        {
            // خلفية الفورم
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.ForeColor = Color.White;

            // تخصيص الأزرار
            StyleButton(btnopen);
            StyleButton(btnsave);
            StyleButton(btnReset);
            StyleButton(btnGray);
            StyleButton(btnQuantize);
            StyleButton(btnOpen3DLab);
            StyleButton(btn);

            // معلومات الصورة
            lblInfo.BackColor = Color.FromArgb(30, 30, 30);
            lblInfo.ForeColor = Color.White;
            lblInfo.BorderStyle = BorderStyle.None;


            // الصورة
            pictureBox1.BackColor = Color.FromArgb(35, 35, 35);

            // المكعب
            panelCube.BackColor = Color.FromArgb(25, 25, 25);
            panelSelectedColor.BorderStyle = BorderStyle.None;

            // الـ TrackBars
            trackRed.BackColor = this.BackColor;
            trackGreen.BackColor = this.BackColor;
            trackBlue.BackColor = this.BackColor;

            // الـ Checkboxes
            chkRed.ForeColor = Color.White;
            chkGreen.ForeColor = Color.White;
            chkBlue.ForeColor = Color.White;

            // Labels
            label1.ForeColor = Color.White;
            label2.ForeColor = Color.White;
            label3.ForeColor = Color.White;
        }

        private void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;

            button.FlatAppearance.BorderSize = 0;

            button.BackColor = Color.FromArgb(45, 45, 48);

            button.ForeColor = Color.White;

            button.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            button.Cursor = Cursors.Hand;

            button.Height = 40;

            button.MouseEnter += Button_MouseEnter;

            button.MouseLeave += Button_MouseLeave;

            button.Width = 200;

            button.Margin = new Padding(5);

            button.Region = Region.FromHrgn(
          CreateRoundRectRgn(0, 0, button.Width, button.Height, 10, 10));
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            button.BackColor = Color.FromArgb(70, 70, 75);
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            button.BackColor = Color.FromArgb(45, 45, 48);
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int left,
            int top,
            int right,
            int bottom,
            int width,
            int height
        );
        private bool HasImage()
        {
            return currentImage != null;
        }

        private void LoadImage(string imagePath)
        {
            originalImage = new Bitmap(imagePath);
            currentImage = new Bitmap(originalImage);

            pictureBox1.Image = currentImage;

            UpdateImageInfo(imagePath);

            panelCube.Invalidate();
        }

        private void UpdateImageInfo(string imagePath)
        {
            FileInfo file = new FileInfo(imagePath);

            lblInfo.Text =
                $"Name: {file.Name}" +
                $"\nSize: {file.Length / 1024} KB" +
                $"\nWidth: {originalImage.Width}" +
                $"\nHeight: {originalImage.Height}";
        }

        private void btnopen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadImage(ofd.FileName);
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
            if (!HasImage()) return;

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
                panelSelectedColor.BackColor = pixelColor;
                selectedPixelColor = pixelColor; // تحديث اللون العام للمكعب

                // مزامنة الـ Trackbars على اليمين تلقائياً
                if (trackRed != null) trackRed.Value = pixelColor.R;
                if (trackGreen != null) trackGreen.Value = pixelColor.G;
                if (trackBlue != null) trackBlue.Value = pixelColor.B;

                // تحديث لوحة رسم مكعب الألوان فوراً ليتحرك المؤشر المشع
                panelCube.Invalidate();

                panelSelectedColor.BackColor = pixelColor;

                lblInspectorInfo.Text = infoReport;
            }
        }


        private void Form1_DragEnter(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Copy; }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadImage(files[0]);
        }

        //====================================================================================
        //====================================================================================
        // حدث النقر على زر تقليل عدد الألوان (تكميم الصورة)
        private void btnQuantizeColors_Click(object sender, EventArgs e)
        {
            // التأكد من وجود صورة محملة في مساحة العمل أولاً
            if (!HasImage()) return;

            // استدعاء تابع تقليل الألوان السريع من كلاس المعالجة بـ 4 مستويات لونية
            Bitmap quantizedResult = ImageProcessor.QuantizeImageColors((Bitmap)currentImage, colorLevelsCount: 4);

            // تحديث الصورة وعرضها على الواجهة الرسومية فوراً
            currentImage = quantizedResult;
            pictureBox1.Image = currentImage;
        }

        // حدث النقر على زر تحويل الصورة إلى رمادي
        private void btnConvertGrayscale_Click(object sender, EventArgs e)
        {
            if (!HasImage()) return;
            // استدعاء تابع الرمادي السريع
            currentImage = ImageProcessor.ConvertToGrayscale((Bitmap)currentImage);
            pictureBox1.Image = currentImage;
        }

        private void btnConvertBlackAndWhite_Click(object sender, EventArgs e)
        {
            if (!HasImage()) return;

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

        private void LoadAndDisplayImage(string filePath)
        {
            // 1. تحميل الصورة وإعداد النسخ
            originalImage = new Bitmap(filePath);
            currentImage = new Bitmap(originalImage);
            pictureBox1.Image = currentImage;

            // 2. جلب معلومات الملف وتحديث بطاقة النصوص (الطلب الثامن)
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            lblInfo.Text = "Name: " + file.Name +
                        "\nSize: " + (file.Length / 1024) + " KB" +
                        "\nWidth: " + originalImage.Width +
                        "\nHeight: " + originalImage.Height;

            // 3. إعادة رسم مكعب الألوان ثلاثي الأبعاد ليتحرك المؤشر
            panelCube.Invalidate();
        }
    }
}