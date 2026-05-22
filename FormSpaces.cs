using System;
using System.Drawing;
using System.Windows.Forms;
using pixellab.Renderers;
using pixellab.Converters; // هنا السحر كله!

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
            
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, 
                null, panel3D, new object[] { true });

            panel3D.MouseWheel += Panel3D_MouseWheel;
            UpdateSyncReport(localSelectedColor);
        }
        public void UpdateFromForm1(Color newColor)
        {
            int r = Math.Max(0, Math.Min(255, (int)newColor.R));
            int g = Math.Max(0, Math.Min(255, (int)newColor.G));
            int b = Math.Max(0, Math.Min(255, (int)newColor.B));

            this.localSelectedColor = Color.FromArgb(r, g, b); 
            
            this.UpdateSyncReport(this.localSelectedColor);
            panel3D.Invalidate();
            System.Diagnostics.Debug.WriteLine("Color received in FormSpaces!"); 
            
        }
        private void InitializeComponentLayout()
        {
            this.Text = "مختبر الفضاءات اللونية التفاعلي ثلاثي الأبعاد - 3D Color Spaces Lab";
            this.Size = new Size(1000, 680); 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(25, 25, 25);

            listSystems = new ListBox();
            listSystems.Dock = DockStyle.Left;
            listSystems.Width = 180;
            listSystems.BackColor = Color.FromArgb(35, 35, 35);
            listSystems.ForeColor = Color.White;
            listSystems.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            listSystems.BorderStyle = BorderStyle.None;
            
            listSystems.Items.AddRange(new object[] { "RGB Cube", "HSV Cone", "YCbCr Space", "YUV Space", "Lab Space", "CMYK Space" });
            listSystems.SelectedIndex = 0;
            listSystems.SelectedIndexChanged += ListSystems_SelectedIndexChanged;

            panel3D = new Panel();
            panel3D.Dock = DockStyle.Fill;
            panel3D.BackColor = Color.FromArgb(20, 20, 20); 
            panel3D.Paint += Panel3D_Paint;
            panel3D.MouseDown += Panel3D_MouseDown;
            panel3D.MouseMove += Panel3D_MouseMove;

            this.Controls.Add(panel3D);
            this.Controls.Add(listSystems);
        }

        private void UpdateSyncReport(Color targetColor)
        {
            // استدعاء التوابع الجاهزة من الكلاسات الخاصة بكِ مباشرة بدون تكرار المعادلات
            var hsv = HsvConverter.FromRgb(targetColor);
            var ycbcr = YcbcrConverter.FromRgb(targetColor);
            var yuv = YuvConverter.FromRgb(targetColor);
            var lab = LabConverter.FromRgb(targetColor);
            var cmyk = CmykConverter.FromRgb(targetColor);

            cachedReportText = $"📊 تقرير المزامنة اللحظي (3D Color Sync):\n" +
                               $"━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                               $"RGB   →  (R: {targetColor.R}, G: {targetColor.G}, B: {targetColor.B})\n\n" +
                               $"HSV   →  (H: {hsv.Hue:0}°, S: {hsv.Saturation * 100:0}%, V: {hsv.Value * 100:0}%)\n\n" +
                               $"YCbCr →  (Y: {ycbcr.Y:0}, Cb: {ycbcr.Cb:0}, Cr: {ycbcr.Cr:0})\n\n" +
                               $"YUV   →  (Y: {yuv.Y * 255:0}, U: {yuv.U:0.00}, V: {yuv.V:0.00})\n\n" + 
                               $"Lab   →  (L*: {lab.L}, a*: {lab.A}, b*: {lab.B:0})\n\n" + 
                               $"CMYK  →  (C: {cmyk.C * 100:0}%, M: {cmyk.M * 100:0}%, Y: {cmyk.Y * 100:0}%, K: {cmyk.K * 100:0}%)";
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
                }
                else if (selectedSystem == "HSV Cone")
                {
                    HsvConeRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
                }
                else if (selectedSystem == "YCbCr Space")
                {
                    YCbCrSpaceRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
                }
                else if (selectedSystem == "YUV Space")
                {
                    YuvSpaceRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
                }
                else if (selectedSystem == "Lab Space")
                {
                    LabRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor, localSelectedColor);
                }
                else if (selectedSystem == "CMYK Space")
                {
                    CmykRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
                }

                RenderReportText(g);
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
                    pickedColor = YCbCrSpaceRenderer.GetColorAtPointDirect(e.Location, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
                }
                else if (selectedSystem == "YUV Space")
                {
                    pickedColor = YuvSpaceRenderer.GetColorAtPointDirect(e.Location, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
                }
                else if (selectedSystem == "Lab Space")
                {
                    pickedColor = LabRenderer.GetColorAtPointDirect(e.Location, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
                }
                else if (selectedSystem == "CMYK Space")
                {
                    pickedColor = CmykRenderer.GetColorAtPointDirect(e.Location, panel3D.Width, panel3D.Height, angleX, angleY, zoomFactor);
                }

                if (pickedColor.ToArgb() != Color.Transparent.ToArgb())
                {
                    localSelectedColor = pickedColor;
                    UpdateSyncReport(localSelectedColor); 
                    panel3D.Invalidate(); 
                    if (Application.OpenForms["Form1"] is Form1 mainForm)
                    {
                        mainForm.UpdateSlidersFrom3D(pickedColor,selectedSystem);
                    }
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
    }
}