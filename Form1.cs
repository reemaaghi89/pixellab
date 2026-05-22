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
        private string currentColorSpace = "RGB";
        private System.Windows.Forms.Timer livePreviewTimer;

        private Dictionary<string, List<ColorChannel>>
        colorSpaces;
        public class ColorChannel
        {
            public string Name { get; set; }

            public int Min { get; set; }

            public int Max { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
            InitializeColorSpaces();
            BuildColorControls("RGB");
            InitializeLivePreview();
            ApplyDarkTheme();
            panelInspector.MouseDown += PanelInspector_MouseDown;
            panelInspector.MouseMove += PanelInspector_MouseMove;
            panelInspector.MouseUp += PanelInspector_MouseUp;

            lblInspectorInfo.MouseDown += PanelInspector_MouseDown;
            lblInspectorInfo.MouseMove += PanelInspector_MouseMove;
            lblInspectorInfo.MouseUp += PanelInspector_MouseUp;

            panelSelectedColor.MouseDown += PanelInspector_MouseDown;
            panelSelectedColor.MouseMove += PanelInspector_MouseMove;
            panelSelectedColor.MouseUp += PanelInspector_MouseUp;
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


            // الـ Checkboxes

            // Labels

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

        private void ApplyCurrentColorSpace()
        {
            switch (currentColorSpace)
            {
                case "RGB":
                    ApplyRGB();
                    break;

                case "HSV":
                    ApplyHSV();
                    break;

                case "CMYK":
                    ApplyCMYK();
                    break;

                case "LAB":
                    ApplyLAB();
                    break;

                case "YUV":
                    ApplyYUV();
                    break;

                case "YCbCr":
                    ApplyYCbCr();
                    break;
            }
        }

        private int GetChannelValue(string channelName)
        {
            foreach (Control c in flowColorControls.Controls)
            {
                if (c is ChannelControl channel)
                {
                    if (channel.lblName.Text == channelName)
                    {
                        return channel.track.Value;
                    }
                }
            }

            return 0;
        }


        private bool isDragging = false;

        private Point dragCursorPoint;

        private Point dragPanelPoint;

        private void PanelInspector_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;

            dragCursorPoint = Cursor.Position;

            dragPanelPoint = panelInspector.Location;
        }

        private void PanelInspector_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point difference =
                    Point.Subtract(
                        Cursor.Position,
                        new Size(dragCursorPoint));

                panelInspector.Location =
                    Point.Add(
                        dragPanelPoint,
                        new Size(difference));
            }
        }

        private void PanelInspector_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }
        private void InitializeColorSpaces()
        {
            colorSpaces =
                new Dictionary<string, List<ColorChannel>>();

            // RGB
            colorSpaces["RGB"] =
                new List<ColorChannel>()
            {
                new ColorChannel()
                {
                    Name="Red",
                    Min=-255,
                    Max=255
                },

                new ColorChannel()
                {
                    Name="Green",
                    Min=-255,
                    Max=255
                },

                new ColorChannel()
                {
                    Name="Blue",
                    Min=-255,
                    Max=255
                }
            };

            // HSV
            colorSpaces["HSV"] =
                new List<ColorChannel>()
            {
                new ColorChannel()
                {
                    Name="Hue",
                    Min=0,
                    Max=360
                },

                new ColorChannel()
                {
                    Name="Saturation",
                    Min=0,
                    Max=100
                },

                new ColorChannel()
                {
                    Name="Value",
                    Min=0,
                    Max=100
                }
            };

            // CMYK
            colorSpaces["CMYK"] =
                new List<ColorChannel>()
            {
                new ColorChannel()
                {
                    Name="Cyan",
                    Min=0,
                    Max=100
                },

                new ColorChannel()
                {
                    Name="Magenta",
                    Min=0,
                    Max=100
                },

                new ColorChannel()
                {
                    Name="Yellow",
                    Min=0,
                    Max=100
                },

                new ColorChannel()
                {
                    Name="Black",
                    Min=0,
                    Max=100
                }
            };

            // LAB
            colorSpaces["LAB"] =
                new List<ColorChannel>()
            {
                new ColorChannel()
                {
                    Name="L",
                    Min=0,
                    Max=100
                },

                new ColorChannel()
                {
                    Name="A",
                    Min=-128,
                    Max=127
                },

                new ColorChannel()
                {
                    Name="B",
                    Min=-128,
                    Max=127
                }
            };

            // YUV
            colorSpaces["YUV"] =
                new List<ColorChannel>()
            {
                new ColorChannel()
                {
                    Name="Y",
                    Min=0,
                    Max=255
                },

                new ColorChannel()
                {
                    Name="U",
                    Min=-128,
                    Max=127
                },

                new ColorChannel()
                {
                    Name="V",
                    Min=-128,
                    Max=127
                }
            };

            // YCbCr
            colorSpaces["YCbCr"] =
                new List<ColorChannel>()
            {
                new ColorChannel()
                {
                    Name="Y",
                    Min=16,
                    Max=235
                },

                new ColorChannel()
                {
                    Name="Cb",
                    Min=16,
                    Max=240
                },

                new ColorChannel()
                {
                    Name="Cr",
                    Min=16,
                    Max=240
                }
            };
        }

        private void BuildColorControls(string systemName)
        {
            currentColorSpace = systemName;

            flowColorControls.Controls.Clear();

            foreach (var channel in colorSpaces[systemName])
            {
                ChannelControl control =
                    new ChannelControl(
                        channel.Name,
                        channel.Min,
                        channel.Max);

                // ربط الحدث
                control.track.Scroll += ChannelTrackChanged;

                flowColorControls.Controls.Add(control);
            }
        }
        private void ChannelTrackChanged(
         object sender,
         EventArgs e)
        {
            livePreviewTimer.Stop();

            livePreviewTimer.Start();
        }
        private void cmbColorSpaces_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            string selected =
                cmbColorSpaces.SelectedItem.ToString();

            BuildColorControls(selected);
        }

        private void InitializeLivePreview()
        {
            livePreviewTimer =
            new System.Windows.Forms.Timer();

            livePreviewTimer.Interval = 120;

            livePreviewTimer.Tick +=
                LivePreviewTimer_Tick;
        }

        private void LivePreviewTimer_Tick(
    object sender,
    EventArgs e)
        {
            livePreviewTimer.Stop();

            ApplyCurrentColorSpace();
        }

        private void ApplyRGB()
        {
            if (originalImage == null)
                return;

            int r = GetChannelValue("Red");

            int g = GetChannelValue("Green");

            int b = GetChannelValue("Blue");

            currentImage =
                ImageProcessor.ApplyRGBFast(
                    originalImage,
                    r,
                    g,
                    b);

            pictureBox1.Image =
                currentImage;
        }


        private void ApplyHSV()
        {
            if (originalImage == null)
                return;

            double h =
                GetChannelValue("Hue");

            double s =
                GetChannelValue("Saturation") / 100.0;

            double v =
                GetChannelValue("Value") / 100.0;

            currentImage =
                ImageProcessor.ApplyHSVAdjustment(
                    originalImage,
                    h,
                    s,
                    v);

            pictureBox1.Image =
                currentImage;
        }

        private void ApplyCMYK()
        {
            if (originalImage == null)
                return;

            double c =
                GetChannelValue("Cyan") / 100.0;

            double m =
                GetChannelValue("Magenta") / 100.0;

            double y =
                GetChannelValue("Yellow") / 100.0;

            double k =
                GetChannelValue("Black") / 100.0;

            currentImage =
                ImageProcessor.ApplyCMYKAdjustment(
                    originalImage,
                    c,
                    m,
                    y,
                    k);

            pictureBox1.Image =
                currentImage;
        }

        private void ApplyLAB()
        {
            if (originalImage == null)
                return;

            double l =
                GetChannelValue("L");

            double a =
                GetChannelValue("A");

            double b =
                GetChannelValue("B");

            currentImage =
                ImageProcessor.ApplyLABAdjustment(
                    originalImage,
                    l,
                    a,
                    b);

            pictureBox1.Image =
                currentImage;
        }

        private void ApplyYUV()
{
    if (originalImage == null)
        return;

    double y =
        GetChannelValue("Y");

    double u =
        GetChannelValue("U");

    double v =
        GetChannelValue("V");

    currentImage =
        ImageProcessor.ApplyYUVAdjustment(
            originalImage,
            y,
            u,
            v);

    pictureBox1.Image =
        currentImage;
}

      private void ApplyYCbCr()
{
    if (originalImage == null)
        return;

    double y =
        GetChannelValue("Y");

    double cb =
        GetChannelValue("Cb");

    double cr =
        GetChannelValue("Cr");

    currentImage =
        ImageProcessor.ApplyYCbCrAdjustment(
            originalImage,
            y,
            cb,
            cr);

    pictureBox1.Image =
        currentImage;
}

    }
}