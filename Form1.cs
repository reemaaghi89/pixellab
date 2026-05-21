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

        private double angleX = 35.0; // زاوية الدوران الأفقي البدئية
        private double angleY = 45.0; // زاوية الدوران العمودي البدئية
        private Point lastMousePos;  // تتبع موقع الماوس الأخير للسحب
        private Color selectedPixelColor = Color.FromArgb(255, 255, 255); // لتحديد موضع النقطة داخل المكعب

        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
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
                LoadAndDisplayImage(ofd.FileName);
            }
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                LoadAndDisplayImage(files[0]);
            }
        }
        
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
        

        //====================================================================================
        //====================================================================================
        // حدث النقر على زر تقليل عدد الألوان (تكميم الصورة)
        private void btnQuantizeColors_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;
            Bitmap quantizedResult = ImageProcessor.QuantizeImageColors(originalImage, colorLevelsCount: 4);
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