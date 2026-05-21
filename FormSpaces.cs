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

        public FormSpaces()
        {
            InitializeComponentLayout();
            
            // تفعيل التخزين المزدوج لمنع الوميض نهائياً أثناء التدوير والزوم
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, 
                null, panel3D, new object[] { true });

            panel3D.MouseWheel += Panel3D_MouseWheel;

            // حساب التقرير للون الافتراضي عند الإقلاع
            UpdateSyncReport(localSelectedColor);
        }

        private void InitializeComponentLayout()
        {
            this.Text = "مختبر الفضاءات اللونية التفاعلي ثلاثي الأبعاد - 3D Color Spaces Lab";
            this.Size = new Size(1000, 680); // تكبير طفيف لاستيعاب بيانات التقرير الإضافية
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(25, 25, 25);

            listSystems = new ListBox();
            listSystems.Dock = DockStyle.Left;
            listSystems.Width = 180;
            listSystems.BackColor = Color.FromArgb(35, 35, 35);
            listSystems.ForeColor = Color.White;
            listSystems.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            listSystems.BorderStyle = BorderStyle.None;
            
            // 🌟 تحديث: إضافة Lab Space إلى القائمة الجانبية بشكل رسمي
            listSystems.Items.AddRange(new object[] { "RGB Cube", "HSV Cone", "YCbCr Space", "Lab Space" });
            listSystems.SelectedIndex = 0;
            listSystems.SelectedIndexChanged += ListSystems_SelectedIndexChanged;

            panel3D = new Panel();
            panel3D.Dock = DockStyle.Fill;
            panel3D.BackColor = Color.FromArgb(20, 20, 20); // الخلفية الموحدة للواجهة
            panel3D.Paint += Panel3D_Paint;
            panel3D.MouseDown += Panel3D_MouseDown;
            panel3D.MouseMove += Panel3D_MouseMove;

            this.Controls.Add(panel3D);
            this.Controls.Add(listSystems);
        }

        private void UpdateSyncReport(Color targetColor)
        {
            // 1. حساب قيم HSV
            double h, s, v;
            ColorToHsv(targetColor, out h, out s, out v);

            // 2. حساب قيم YCbCr الأكاديمية
            double y = 0.299 * targetColor.R + 0.587 * targetColor.G + 0.114 * targetColor.B;
            double cb = 128 - 0.168736 * targetColor.R - 0.331264 * targetColor.G + 0.5 * targetColor.B;
            double cr = 128 + 0.5 * targetColor.R - 0.418688 * targetColor.G - 0.081312 * targetColor.B;

            // 3. 🌟 تحديث: حساب قيم CIELAB (Lab) اللحظية بدقة متناهية للمزامنة النصية
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
                else if (selectedSystem == "YCbCr Space")
                {
                    // 🌟 سيتم ربط تابع التقاط الماوس لـ YCbCr هنا في الخطوة القادمة
                }
                else if (selectedSystem == "Lab Space")
                {
                    // 🌟 سيتم ربط تابع التقاط الماوس لـ Lab هنا في الخطوة القادمة
                }

                if (pickedColor.ToArgb() != Color.Transparent.ToArgb())
                {
                    localSelectedColor = pickedColor;
                    UpdateSyncReport(localSelectedColor); 
                    panel3D.Invalidate(); 
                }
            }
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

        // 🌟 تابع داخلي مساعد لحساب قيم Lab للتقرير النصي دون التضارب مع كلاسات الـ Renderers
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