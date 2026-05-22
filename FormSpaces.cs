// using System;
// using System.Drawing;
// using System.Windows.Forms;
// using pixellab.Renderers;

// namespace pixellab
// {
//     public partial class FormSpaces : Form
//     {
//         private double angleX = 35.0;
//         private double angleY = 45.0;
//         private float zoomFactor = 1.0f; 
//         private Point lastMousePos;
//         private string selectedSystem = "RGB Cube"; 

//         private Color localSelectedColor = Color.Blue; 
//         private string cachedReportText = string.Empty;

//         private ListBox listSystems;
//         private Panel panel3D;

//         public FormSpaces()
//         {
//             InitializeComponentLayout();
            
//             // تفعيل التخزين المزدوج لمنع الوميض نهائياً أثناء التدوير والزوم
//             typeof(Panel).InvokeMember("DoubleBuffered", 
//                 System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, 
//                 null, panel3D, new object[] { true });

//             panel3D.MouseWheel += Panel3D_MouseWheel;

//             // حساب التقرير للون الافتراضي عند الإقلاع
//             UpdateSyncReport(localSelectedColor);
//         }

//         private void InitializeComponentLayout()
//         {
//             this.Text = "مختبر الفضاءات اللونية التفاعلي ثلاثي الأبعاد - 3D Color Spaces Lab";
//             this.Size = new Size(1000, 680); // تكبير طفيف لاستيعاب بيانات التقرير الإضافية
//             this.StartPosition = FormStartPosition.CenterScreen;
//             this.BackColor = Color.FromArgb(25, 25, 25);

//             listSystems = new ListBox();
//             listSystems.Dock = DockStyle.Left;
//             listSystems.Width = 180;
//             listSystems.BackColor = Color.FromArgb(35, 35, 35);
//             listSystems.ForeColor = Color.White;
//             listSystems.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
//             listSystems.BorderStyle = BorderStyle.None;
            
//             // 🌟 تحديث: إضافة Lab Space إلى القائمة الجانبية بشكل رسمي
//             listSystems.Items.AddRange(new object[] { "RGB Cube", "HSV Cone", "YCbCr Space", "Lab Space" });
//             listSystems.SelectedIndex = 0;
//             listSystems.SelectedIndexChanged += ListSystems_SelectedIndexChanged;

//             panel3D = new Panel();
//             panel3D.Dock = DockStyle.Fill;
//             panel3D.BackColor = Color.FromArgb(20, 20, 20); // الخلفية الموحدة للواجهة
//             panel3D.Paint += Panel3D_Paint;
//             panel3D.MouseDown += Panel3D_MouseDown;
//             panel3D.MouseMove += Panel3D_MouseMove;

//             this.Controls.Add(panel3D);
//             this.Controls.Add(listSystems);
//         }

//         private void UpdateSyncReport(Color targetColor)
//         {
//             // 1. حساب قيم HSV
//             double h, s, v;
//             ColorToHsv(targetColor, out h, out s, out v);

//             // 2. حساب قيم YCbCr الأكاديمية
//             double y = 0.299 * targetColor.R + 0.587 * targetColor.G + 0.114 * targetColor.B;
//             double cb = 128 - 0.168736 * targetColor.R - 0.331264 * targetColor.G + 0.5 * targetColor.B;
//             double cr = 128 + 0.5 * targetColor.R - 0.418688 * targetColor.G - 0.081312 * targetColor.B;

//             // 3. 🌟 تحديث: حساب قيم CIELAB (Lab) اللحظية بدقة متناهية للمزامنة النصية
//             var lab = LocalRgbToLab(targetColor);

//             cachedReportText = $"📊 تقرير المزامنة اللحظي (3D Color Sync):\n" +
//                                $"━━━━━━━━━━━━━━━━━━━━━━━━\n" +
//                                $"RGB   →  (R: {targetColor.R}, G: {targetColor.G}, B: {targetColor.B})\n\n" +
//                                $"HSV   →  (H: {h:0}°, S: {s * 100:0}%, V: {v * 100:0}%)\n\n" +
//                                $"YCbCr →  (Y: {y:0}, Cb: {cb:0}, Cr: {cr:0})\n\n" +
//                                $"Lab   →  (L*: {lab.L:0}, a*: {lab.A:0}, b*: {lab.B:0})";
//         }

//         private void ListSystems_SelectedIndexChanged(object sender, EventArgs e)
//         {
//             selectedSystem = listSystems.SelectedItem.ToString();
//             panel3D.Invalidate(); 
//         }

//         private void Panel3D_Paint(object sender, PaintEventArgs e)
//         {
//             try
//             {
//                 Graphics g = e.Graphics;
//                 g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

//                 if (selectedSystem == "RGB Cube")
//                 {
//                     RgbCubeRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
//                     RenderReportText(g);
//                 }
//                 else if (selectedSystem == "HSV Cone")
//                 {
//                     HsvConeRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
//                     RenderReportText(g);
//                 }
//                 else if (selectedSystem == "YCbCr Space")
//                 {
//                     YCbCrSpaceRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
//                     RenderReportText(g);
//                 }
//                 else if (selectedSystem == "Lab Space")
//                 {
//                     LabSpaceRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
//                     RenderReportText(g);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 System.Diagnostics.Debug.WriteLine("Paint Error: " + ex.Message);
//             }
//         }

//         private void RenderReportText(Graphics g)
//         {
//             using (Font reportFont = new Font("Consolas", 11f, FontStyle.Bold))
//             using (Brush textBrush = new SolidBrush(Color.FromArgb(230, 230, 230)))
//             {
//                 g.DrawString(cachedReportText, reportFont, textBrush, 20, 20);
//             }
//         }

//         private void Panel3D_MouseDown(object sender, MouseEventArgs e)
//         {
//             if (e.Button == MouseButtons.Left)
//             {
//                 lastMousePos = e.Location;
//                 Color pickedColor = Color.Transparent;

//                 if (selectedSystem == "RGB Cube")
//                 {
//                     pickedColor = RgbCubeRenderer.GetColorAtPointDirect(e.Location, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
//                 }
//                 else if (selectedSystem == "HSV Cone")
//                 {
//                     pickedColor = HsvConeRenderer.GetColorAtPointDirect(e.Location, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
//                 }
//                 else if (selectedSystem == "YCbCr Space")
//                 {
//                     // 🌟 سيتم ربط تابع التقاط الماوس لـ YCbCr هنا في الخطوة القادمة
//                 }
//                 else if (selectedSystem == "Lab Space")
//                 {
//                     // 🌟 سيتم ربط تابع التقاط الماوس لـ Lab هنا في الخطوة القادمة
//                 }

//                 if (pickedColor.ToArgb() != Color.Transparent.ToArgb())
//                 {
//                     localSelectedColor = pickedColor;
//                     UpdateSyncReport(localSelectedColor); 
//                     panel3D.Invalidate(); 
//                 }
//             }
//         }

//         private void Panel3D_MouseMove(object sender, MouseEventArgs e)
//         {
//             if (e.Button == MouseButtons.Left)
//             {
//                 int dx = e.X - lastMousePos.X;
//                 int dy = e.Y - lastMousePos.Y;

//                 angleY += dx * 0.5;
//                 angleX += dy * 0.5;

//                 lastMousePos = e.Location;
//                 panel3D.Invalidate(); 
//             }
//         }

//         private void Panel3D_MouseWheel(object sender, MouseEventArgs e)
//         {
//             if (e.Delta > 0) zoomFactor += 0.1f;
//             else zoomFactor -= 0.1f;

//             zoomFactor = Math.Max(0.3f, Math.Min(3.0f, zoomFactor)); 
//             panel3D.Invalidate();
//         }

//         private void ColorToHsv(Color color, out double hue, out double sat, out double val)
//         {
//             int max = Math.Max(color.R, Math.Max(color.G, color.B));
//             int min = Math.Min(color.R, Math.Min(color.G, color.B));

//             hue = color.GetHue();
//             sat = (max == 0) ? 0 : 1d - (1d * min / max);
//             val = max / 255d;
//         }

//         // 🌟 تابع داخلي مساعد لحساب قيم Lab للتقرير النصي دون التضارب مع كلاسات الـ Renderers
//         private (double L, double A, double B) LocalRgbToLab(Color c)
//         {
//             double r = c.R / 255.0;
//             double g = c.G / 255.0;
//             double b = c.B / 255.0;

//             r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
//             g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
//             b = (b > 0.04045) ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92;

//             double x = (r * 0.4124 + g * 0.3576 + b * 0.1805) / 0.95047;
//             double y = (r * 0.2126 + g * 0.7152 + b * 0.0722) / 1.00000;
//             double z = (r * 0.0193 + g * 0.1192 + b * 0.9505) / 1.08883;

//             x = (x > 0.008856) ? Math.Pow(x, 1.0 / 3.0) : (7.787 * x) + (16.0 / 116.0);
//             y = (y > 0.008856) ? Math.Pow(y, 1.0 / 3.0) : (7.787 * y) + (16.0 / 116.0);
//             z = (z > 0.008856) ? Math.Pow(z, 1.0 / 3.0) : (7.787 * z) + (16.0 / 116.0);

//             double L = (116.0 * y) - 16.0;
//             double A = 500.0 * (x - y);
//             double B = 200.0 * (y - z);

//             return (L, A, B);
//         }
//     }
// }

using System;
using System.Drawing;
using System.Windows.Forms;
using pixellab.Renderers;

namespace pixellab
{
    public partial class FormSpaces : Form
    {
        private double angleX = 35.0;
        private double angleY = 45.0;
        private float zoomFactor = 1.0f; 
        private Point lastMousePos;
        private string selectedSystem = "RGB Cube"; 

        private Color localSelectedColor = Color.Blue; 
        private string cachedReportText = string.Empty;

        private ListBox listSystems;
        private Panel panel3D;

        // 🌟 المكونات الجديدة الخاصة بمعالجة الصور
        private Panel panelImageProcess;
        private PictureBox picOriginal;
        private PictureBox picProcessed;
        private Button btnLoadImage;
        private Button btnApplyChannels;
        private Bitmap originalBitmap = null;

        public FormSpaces()
        {
            InitializeComponentLayout();
            
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, 
                null, panel3D, new object[] { true });

            panel3D.MouseWheel += Panel3D_MouseWheel;

            UpdateSyncReport(localSelectedColor);
        }

        private void InitializeComponentLayout()
        {
            this.Text = "مختبر الفضاءات اللونية التفاعلي ثلاثي الأبعاد - 3D Color Spaces Lab";
            this.Size = new Size(1300, 700); // قمنا بتكبير الواجهة أفقياً لتتسع لقسم معالجة الصور
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(25, 25, 25);

            // 1. القائمة اليسارية (الأنظمة ثلاثية الأبعاد)
            listSystems = new ListBox();
            listSystems.Dock = DockStyle.Left;
            listSystems.Width = 180;
            listSystems.BackColor = Color.FromArgb(35, 35, 35);
            listSystems.ForeColor = Color.White;
            listSystems.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            listSystems.BorderStyle = BorderStyle.None;
            listSystems.Items.AddRange(new object[] { "RGB Cube", "HSV Cone", "YCbCr Space", "Lab Space" });
            listSystems.SelectedIndex = 0;
            listSystems.SelectedIndexChanged += ListSystems_SelectedIndexChanged;

            // 2. اللوحة اليمنى الجديدة (قسم الصور)
            panelImageProcess = new Panel();
            panelImageProcess.Dock = DockStyle.Right;
            panelImageProcess.Width = 320;
            panelImageProcess.BackColor = Color.FromArgb(30, 30, 30);
            panelImageProcess.Padding = new Padding(10);

            btnLoadImage = new Button();
            btnLoadImage.Text = "📁 تحميل صورة";
            btnLoadImage.Dock = DockStyle.Top;
            btnLoadImage.Height = 40;
            btnLoadImage.BackColor = Color.FromArgb(50, 50, 50);
            btnLoadImage.ForeColor = Color.White;
            btnLoadImage.FlatStyle = FlatStyle.Flat;
            btnLoadImage.Click += BtnLoadImage_Click;

            Label lblOrig = new Label { Text = "الصورة الأصلية:", ForeColor = Color.Gray, Dock = DockStyle.Top, Height = 25, TextAlign = ContentAlignment.BottomLeft };
            
            picOriginal = new PictureBox();
            picOriginal.Dock = DockStyle.Top;
            picOriginal.Height = 200;
            picOriginal.BorderStyle = BorderStyle.FixedSingle;
            picOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            picOriginal.MouseClick += PicOriginal_MouseClick; // التقاط اللون عند النقر على الصورة!

            Label lblProc = new Label { Text = "معالجة القنوات (حسب النظام المختار):", ForeColor = Color.Gray, Dock = DockStyle.Top, Height = 25, TextAlign = ContentAlignment.BottomLeft };

            picProcessed = new PictureBox();
            picProcessed.Dock = DockStyle.Top;
            picProcessed.Height = 200;
            picProcessed.BorderStyle = BorderStyle.FixedSingle;
            picProcessed.SizeMode = PictureBoxSizeMode.Zoom;

            btnApplyChannels = new Button();
            btnApplyChannels.Text = "⚙️ تطبيق فضاء الألوان على الصورة";
            btnApplyChannels.Dock = DockStyle.Top;
            btnApplyChannels.Height = 45;
            btnApplyChannels.BackColor = Color.DarkSlateGray;
            btnApplyChannels.ForeColor = Color.White;
            btnApplyChannels.FlatStyle = FlatStyle.Flat;
            btnApplyChannels.Click += BtnApplyChannels_Click;

            // ترتيب المكونات في لوحة الصور (من الأسفل للأعلى في الـ Controls بسبب الـ Dock)
            panelImageProcess.Controls.Add(btnApplyChannels);
            panelImageProcess.Controls.Add(picProcessed);
            panelImageProcess.Controls.Add(lblProc);
            panelImageProcess.Controls.Add(picOriginal);
            panelImageProcess.Controls.Add(lblOrig);
            panelImageProcess.Controls.Add(btnLoadImage);

            // 3. لوحة الرسم المركزية
            panel3D = new Panel();
            panel3D.Dock = DockStyle.Fill;
            panel3D.BackColor = Color.FromArgb(20, 20, 20); 
            panel3D.Paint += Panel3D_Paint;
            panel3D.MouseDown += Panel3D_MouseDown;
            panel3D.MouseMove += Panel3D_MouseMove;

            // إضافة كل اللوحات للـ Form
            this.Controls.Add(panel3D);
            this.Controls.Add(panelImageProcess);
            this.Controls.Add(listSystems);
        }

        // تابع لرفع الصورة من جهازكِ
            // 1. تعديل تابع تحميل الصورة لتصغيرها تلقائياً ومنع التعليق
    private void BtnLoadImage_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog ofd = new OpenFileDialog())
        {
            ofd.Filter = "Image Files(*.jpg; *.jpeg; *.png; *.bmp)|*.jpg; *.jpeg; *.png; *.bmp";
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            Bitmap loadedImg = new Bitmap(ofd.FileName);

            // حد أقصى للأبعاد لمنع تجمد الواجهة مع الصور الضخمة
            int maxDimension = 600; 
            int newWidth = loadedImg.Width;
            int newHeight = loadedImg.Height;

            if (loadedImg.Width > maxDimension || loadedImg.Height > maxDimension)
            {
                if (loadedImg.Width > loadedImg.Height)
                {
                    newWidth = maxDimension;
                    newHeight = (int)((double)loadedImg.Height / loadedImg.Width * maxDimension);
                }
                else
                {
                    newHeight = maxDimension;
                    newWidth = (int)((double)loadedImg.Width / loadedImg.Height * maxDimension);
                }
            }

            // إنشاء النسخة المصغرة المريحة للمعالجة والذاكرة
            originalBitmap = new Bitmap(loadedImg, new Size(newWidth, newHeight));
            loadedImg.Dispose(); // تحرير ملف الصورة الأصلي من الذاكرة

            picOriginal.Image = originalBitmap;
            picProcessed.Image = null; 
                }
        }
     }

        // تفاعل ذكي: عند الضغط على أي بكسل في الصورة الأصلية، يتم تحديث التقرير والـ 3D فوراً!
        private void PicOriginal_MouseClick(object sender, MouseEventArgs e)
        {
            if (picOriginal.Image == null || originalBitmap == null) return;

            // حساب الإحداثيات الحقيقية للبكسل داخل الصورة مع مراعاة الـ Zoom Mode
            int pWidth = picOriginal.Width;
            int pHeight = picOriginal.Height;
            int w = originalBitmap.Width;
            int h = originalBitmap.Height;

            double ratioX = (double)w / pWidth;
            double ratioY = (double)h / pHeight;
            double ratio = Math.Max(ratioX, ratioY);

            int imgWidth = (int)(w / ratio);
            int imgHeight = (int)(h / ratio);

            int xOffset = (pWidth - imgWidth) / 2;
            int yOffset = (pHeight - imgHeight) / 2;

            int clickedX = (int)((e.X - xOffset) * ratio);
            int clickedY = (int)((e.Y - yOffset) * ratio);

            if (clickedX >= 0 && clickedX < w && clickedY >= 0 && clickedY < h)
            {
                localSelectedColor = originalBitmap.GetPixel(clickedX, clickedY);
                UpdateSyncReport(localSelectedColor);
                panel3D.Invalidate(); // إعادة رسم المكعب والمؤشر بناءً على بكسل الصورة!
            }
            BtnApplyChannels_Click(null, null);
        }

        // زر المعالجة: يمر على الصورة بكسل بكسل ويحولها حسب النظام النشط حالياً في القائمة اليسرى
        // 2. تعديل تابع تطبيق الفضاءات لحل مشكلة الفلتر الأحمر وتسريع الأداء
    private void BtnApplyChannels_Click(object sender, EventArgs e)
{
    if (originalBitmap == null)
    {
        MessageBox.Show("الرجاء تحميل صورة أولاً!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    this.Cursor = Cursors.WaitCursor;
    
    // إنشاء صورة مخرجات جديدة بنفس أبعاد الصورة الحالية
    Bitmap output = new Bitmap(originalBitmap.Width, originalBitmap.Height);

    // جلب قيم اللون الحالي الذي يقف عنده المؤشر في الـ 3D لاستخدامه في الفلترة اللحظية
    Color target = localSelectedColor; 

    for (int x = 0; x < originalBitmap.Width; x++)
    {
        for (int y = 0; y < originalBitmap.Height; y++)
        {
            Color c = originalBitmap.GetPixel(x, y);

            if (selectedSystem == "RGB Cube")
            {
                // ✨ التصحيح الحقيقي: يعرض الألوان الطبيعية والكاملة للصورة!
                // ويقوم بعمل عزل لوني: البكسلات القريبة من لون المؤشر تظل مضيئة، والبعيدة تصبح معتمة
                int dist = Math.Abs(c.R - target.R) + Math.Abs(c.G - target.G) + Math.Abs(c.B - target.B);
                if (dist < 120) 
                {
                    output.SetPixel(x, y, c); // يظهر بلونه الطبيعي الكامل
                }
                else
                {
                    // تخفيت بقية الألوان البعيدة عن اختيار المؤشر لإبراز اللون المحدد
                    output.SetPixel(x, y, Color.FromArgb(c.R / 4, c.G / 4, c.B / 4));
                }
            }
            else if (selectedSystem == "HSV Cone")
            {
                // فلاتر تعتمد على تفاعل قيمة الـ HSV للون المؤشر
                double h, s, v;
                ColorToHsv(c, out h, out s, out v);
                
                double th, ts, tv;
                ColorToHsv(target, out th, out ts, out tv);

                // إذا كان البكسل له نفس صبغة (Hue) اللون المختار للمؤشر تقريباً
                if (Math.Abs(h - th) < 20) 
                {
                    output.SetPixel(x, y, c); 
                }
                else
                {
                    // تحويل بقية الألوان لرمادي بناءً على سطوع البكسل
                    int gray = (int)(v * 255);
                    output.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            else if (selectedSystem == "YCbCr Space")
            {
                // تحويل الصورة إلى تدرج رمادي أكاديمي دقيق عبر قناة Y
                int grayY = (int)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B);
                grayY = Math.Max(0, Math.Min(255, grayY));
                output.SetPixel(x, y, Color.FromArgb(grayY, grayY, grayY));
            }
            else if (selectedSystem == "Lab Space")
            {
                var labc = LocalRgbToLab(c);
                var labt = LocalRgbToLab(target);

                // حساب الفارق اللوني الحقيقي (Delta E) بين البكسل والمؤشر
                double deltaE = Math.Sqrt(Math.Pow(labc.L - labt.L, 2) + Math.Pow(labc.A - labt.A, 2) + Math.Pow(labc.B - labt.B, 2));

                if (deltaE < 30) // قريب جداً برؤية العين البشرية من لون المؤشر
                {
                    output.SetPixel(x, y, c);
                }
                else
                {
                    int lByte = (int)(labc.L * 2.55);
                    lByte = Math.Max(0, Math.Min(255, lByte));
                    output.SetPixel(x, y, Color.FromArgb(lByte, lByte, lByte));
                }
            }
        }
    }

    // عرض النتيجة المحدثة فوراً وتنظيف الذاكرة السابقة
    if (picProcessed.Image != null) picProcessed.Image.Dispose();
    picProcessed.Image = output;
    
    this.Cursor = Cursors.Default;
}
    
        private void UpdateSyncReport(Color targetColor)
        {
            double h, s, v;
            ColorToHsv(targetColor, out h, out s, out v);

            double y = 0.299 * targetColor.R + 0.587 * targetColor.G + 0.114 * targetColor.B;
            double cb = 128 - 0.168736 * targetColor.R - 0.331264 * targetColor.G + 0.5 * targetColor.B;
            double cr = 128 + 0.5 * targetColor.R - 0.418688 * targetColor.G - 0.081312 * targetColor.B;

            var lab = LocalRgbToLab(targetColor);

            cachedReportText = $"📊 تقرير المزامنة اللحظي (3D Color Sync):\n" +
                               $"━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                               $"RGB   →  (R: {targetColor.R}, G: {targetColor.G}, B: {targetColor.B})\n\n" +
                               $"HSV   →  (H: {h:0}°, S: {s * 100:0}%, V: {v * 100:0}%)\n\n" +
                               $"YCbCr →  (Y: {y:0}, Cb: {cb:0}, Cr: {cr:0})\n\n" +
                               $"Lab   →  (L*: {lab.L:0}, a*: {lab.A:0}, b*: {lab.B:0})";
        }

        
        private void ListSystems_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSystem = listSystems.SelectedItem.ToString();
            panel3D.Invalidate(); 
        }

        private void Panel3D_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                if (selectedSystem == "RGB Cube")
                {
                    RgbCubeRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
                    RenderReportText(g);
                }
                else if (selectedSystem == "HSV Cone")
                {
                    HsvConeRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
                    RenderReportText(g);
                }
                else if (selectedSystem == "YCbCr Space")
                {
                    YCbCrSpaceRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
                    RenderReportText(g);
                }
                else if (selectedSystem == "Lab Space")
                {
                    LabSpaceRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
                    RenderReportText(g);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Paint Error: " + ex.Message);
            }
        }

        private void RenderReportText(Graphics g)
        {
            using (Font reportFont = new Font("Consolas", 11f, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(230, 230, 230)))
            {
                g.DrawString(cachedReportText, reportFont, textBrush, 20, 20);
            }
        }

        private void Panel3D_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastMousePos = e.Location;
                Color pickedColor = Color.Transparent;

                if (selectedSystem == "RGB Cube")
                {
                    pickedColor = RgbCubeRenderer.GetColorAtPointDirect(e.Location, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
                }
                else if (selectedSystem == "HSV Cone")
                {
                    pickedColor = HsvConeRenderer.GetColorAtPointDirect(e.Location, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
                }

                if (pickedColor.ToArgb() != Color.Transparent.ToArgb())
                {
                    localSelectedColor = pickedColor;
                    UpdateSyncReport(localSelectedColor); 
                    panel3D.Invalidate(); 
                }
            }
            BtnApplyChannels_Click(null, null); // استدعاء الفلتر فوراً لتحديث الصورة مع المؤشر!
        }

        private void Panel3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.X - lastMousePos.X;
                int dy = e.Y - lastMousePos.Y;

                angleY += dx * 0.5;
                angleX += dy * 0.5;

                lastMousePos = e.Location;
                panel3D.Invalidate(); 
            }
        }

        private void Panel3D_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) zoomFactor += 0.1f;
            else zoomFactor -= 0.1f;

            zoomFactor = Math.Max(0.3f, Math.Min(3.0f, zoomFactor)); 
            panel3D.Invalidate();
        }

        private void ColorToHsv(Color color, out double hue, out double sat, out double val)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            sat = (max == 0) ? 0 : 1d - (1d * min / max);
            val = max / 255d;
        }

        private (double L, double A, double B) LocalRgbToLab(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
            g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
            b = (b > 0.04045) ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92;

            double x = (r * 0.4124 + g * 0.3576 + b * 0.1805) / 0.95047;
            double y = (r * 0.2126 + g * 0.7152 + b * 0.0722) / 1.00000;
            double z = (r * 0.0193 + g * 0.1192 + b * 0.9505) / 1.08883;

            x = (x > 0.008856) ? Math.Pow(x, 1.0 / 3.0) : (7.787 * x) + (16.0 / 116.0);
            y = (y > 0.008856) ? Math.Pow(y, 1.0 / 3.0) : (7.787 * y) + (16.0 / 116.0);
            z = (z > 0.008856) ? Math.Pow(z, 1.0 / 3.0) : (7.787 * z) + (16.0 / 116.0);

            double L = (116.0 * y) - 16.0;
            double A = 500.0 * (x - y);
            double B = 200.0 * (y - z);

            return (L, A, B);
        }
    }
}