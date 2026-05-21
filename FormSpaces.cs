using System;
using System.Drawing;
using System.Windows.Forms;
using pixellab.Renderers; // استيراد مجلد توابع الرسم

namespace pixellab
{
    public partial class FormSpaces : Form
    {
        // متغيرات التحكم ثلاثية الأبعاد (دوران وزوم)
        private double angleX = 35.0;
        private double angleY = 45.0;
        private float zoomFactor = 1.0f; 
        private Point lastMousePos;
        private string selectedSystem = "RGB Cube"; // النظام الافتراضي

        // 🌟 اللون المختار محلياً بقلب المختبر 3D (يبدأ باللون الأزرق كمثال تفاعلي)
        private Color localSelectedColor = Color.Blue; 

        // أدوات الواجهة البرمجية
        private ListBox listSystems;
        private Panel panel3D;

        public FormSpaces()
        {
            InitializeComponentLayout();
            
            // تفعيل التخزين المزدوج لمنع الوميض نهائياً أثناء التدوير والزوم
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, 
                null, panel3D, new object[] { true });

            // ربط حدث دولاب الماوس من أجل الزوم (Zoom In / Zoom Out)
            panel3D.MouseWheel += Panel3D_MouseWheel;
        }

        private void InitializeComponentLayout()
        {
            this.Text = "مختبر الفضاءات اللونية التفاعلي ثلاثي الأبعاد - 3D Color Spaces Lab";
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(25, 25, 25);

            // 1. القائمة الجانبية لأنظمة الألوان
            listSystems = new ListBox();
            listSystems.Dock = DockStyle.Left;
            listSystems.Width = 180;
            listSystems.BackColor = Color.FromArgb(35, 35, 35);
            listSystems.ForeColor = Color.White;
            listSystems.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            listSystems.BorderStyle = BorderStyle.None;
            listSystems.Items.AddRange(new object[] { "RGB Cube", "HSV Cone", "YCbCr Space" });
            listSystems.SelectedIndex = 0;
            listSystems.SelectedIndexChanged += ListSystems_SelectedIndexChanged;

            // 2. بانل الرسم ثلاثي الأبعاد والتفاعل
            panel3D = new Panel();
            panel3D.Dock = DockStyle.Fill;
            panel3D.BackColor = Color.FromArgb(20, 20, 20);
            panel3D.Paint += Panel3D_Paint;
            panel3D.MouseDown += Panel3D_MouseDown;
            panel3D.MouseMove += Panel3D_MouseMove;

            // إضافة الأدوات للواجهة
            this.Controls.Add(panel3D);
            this.Controls.Add(listSystems);
        }

        private void ListSystems_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSystem = listSystems.SelectedItem.ToString();
            panel3D.Invalidate(); // إعادة الرسم فوراً عند تغيير النظام
        }

        private void Panel3D_Paint(object sender, PaintEventArgs e)
        {
            // 🛡️ حماية الواجهة بالكامل داخل try-catch لمنع الانهيار
            try
            {
                Graphics g = e.Graphics;
                Color currentColor = localSelectedColor;

                if (selectedSystem == "RGB Cube")
                {
                    // 1. استدعاء الرسم
                    RgbCubeRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, currentColor, this.Font);

                    // 2. الحسابات الرياضية للمزامنة
                    double h, s, v;
                    ColorToHsv(currentColor, out h, out s, out v);

                    double y = 0.299 * currentColor.R + 0.587 * currentColor.G + 0.114 * currentColor.B;
                    double cb = 128 - 0.168736 * currentColor.R - 0.331264 * currentColor.G + 0.5 * currentColor.B;
                    double cr = 128 + 0.5 * currentColor.R - 0.418688 * currentColor.G - 0.081312 * currentColor.B;

                    string reportText = $"📊 تقرير المزامنة اللحظي (3D Color Sync):\n" +
                                        $"━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                                        $"RGB   →  (R: {currentColor.R}, G: {currentColor.G}, B: {currentColor.B})\n\n" +
                                        $"HSV   →  (H: {h:0}°, S: {s * 100:0}%, V: {v * 100:0}%)\n\n" +
                                        $"YCbCr →  (Y: {y:0}, Cb: {cb:0}, Cr: {cr:0})";

                    using (Brush textBrush = new SolidBrush(Color.FromArgb(220, 220, 220)))
                    {
                        g.DrawString(reportText, new Font("Consolas", 11f, FontStyle.Bold), textBrush, 20, 20);
                    }
                }
                else
                {
                    g.Clear(Color.FromArgb(20, 20, 20));
                    using (Brush whiteBrush = new SolidBrush(Color.White))
                    {
                        g.DrawString($"قريباً سيتم رسم فضاء {selectedSystem} هنا وتزامنه 🚀", this.Font, whiteBrush, 20, 20);
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                // 🔥 في حال حدوث ضغط لحظي على الذاكرة، نقوم بعمل تنظيف قسري فوراً دون إظهار خطأ للمستخدم
                GC.Collect();
                GC.WaitForPendingFinalizers();
                panel3D.Invalidate(); // إعادة محاولة الرسم بنظافة
            }
            catch (Exception ex)
            {
                // لأي خطأ آخر غير متوقع
                System.Diagnostics.Debug.WriteLine("Paint Error: " + ex.Message);
            }
        }

        private void Panel3D_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastMousePos = e.Location;

                // 🛡️ حماية كبسة الماوس والتقاط الألوان رياضياً
                try
                {
                    Color detectedColor = RgbCubeRenderer.GetColorAtPoint(e.Location, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
                    
                    if (detectedColor != Color.Transparent)
                    {
                        localSelectedColor = detectedColor;
                        panel3D.Invalidate(); 
                    }
                }
                catch (OutOfMemoryException)
                {
                    // تنظيف قسري للذاكرة إذا ضغط الماوس تزامن مع زووم ضخم جداً
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("MouseDown Error: " + ex.Message);
                }
            }
        }
        private void Panel3D_MouseMove(object sender, MouseEventArgs e)
        {
            // تدوير المجسم عند السحب بالماوس
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.X - lastMousePos.X;
                int dy = e.Y - lastMousePos.Y;

                angleY += dx * 0.5;
                angleX += dy * 0.5;

                lastMousePos = e.Location;
                panel3D.Invalidate(); // إعادة رسم ناعمة جداً بالزمن الحقيقي
            }
        }

        private void Panel3D_MouseWheel(object sender, MouseEventArgs e)
        {
            // التحكم بالزوم عبر دولاب الماوس
            if (e.Delta > 0) zoomFactor += 0.1f;
            else zoomFactor -= 0.1f;

            zoomFactor = Math.Max(0.3f, Math.Min(3.0f, zoomFactor)); // حدود الأمان
            panel3D.Invalidate();
        }

        // دالة مساعدة سريعة لتحويل الـ RGB إلى HSV داخل الفورم لضمان تيسير الحسابات المتزامنة
        private void ColorToHsv(Color color, out double hue, out double sat, out double val)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            sat = (max == 0) ? 0 : 1d - (1d * min / max);
            val = max / 255d;
        }
    }
}