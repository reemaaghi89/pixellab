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
            this.Size = new Size(1150, 780); 
            this.MinimumSize = new Size(800, 600); // يمنع المستخدم من تصغير النافذة لحجم غير منطقي يخرب التصميم
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(18, 18, 22);

            Panel sidePanel = new Panel();
            sidePanel.Dock = DockStyle.Left;
            sidePanel.Width = 260; 
            sidePanel.BackColor = Color.FromArgb(24, 24, 28);
            sidePanel.Padding = new Padding(12);

            listSystems = new ListBox();
            listSystems.Dock = DockStyle.Top;
            listSystems.Height = 200; 
            listSystems.BackColor = Color.FromArgb(30, 30, 35);
            listSystems.ForeColor = Color.Gainsboro;
            listSystems.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            listSystems.BorderStyle = BorderStyle.None;
            listSystems.ItemHeight = 32;
            listSystems.DrawMode = DrawMode.OwnerDrawFixed;
            listSystems.DrawItem += ListSystems_DrawItem;
            
            listSystems.Items.AddRange(new object[] { "RGB Cube", "HSV Cone", "YCbCr Space", "YUV Space", "Lab Space", "CMYK Space" });
            listSystems.SelectedIndex = 0;
            listSystems.SelectedIndexChanged += ListSystems_SelectedIndexChanged;

            // الفاصل بين القائمة والتقرير
            Panel spacer = new Panel { Dock = DockStyle.Top, Height = 15 }; 

            // 3. صندوق التقارير الذكي (يتمدد تلقائياً ليملأ باقي مساحة القائمة الجانبية نزولاً)
            RichTextBox rtbReport = new RichTextBox();
            rtbReport.Dock = DockStyle.Fill; // يملأ كامل المساحة المتبقية تحت القائمة
            rtbReport.Font = new Font("Consolas", 9.5f, FontStyle.Bold);
            rtbReport.ForeColor = Color.FromArgb(220, 220, 225);
            rtbReport.BackColor = Color.FromArgb(18, 18, 22);
            rtbReport.BorderStyle = BorderStyle.None;
            rtbReport.ReadOnly = true;        // للمشاهدة فقط
            rtbReport.WordWrap = true;        // يمنع انقطاع النص ويقوم بإنزال الكلمات للسطر الجديد تلقائياً
            rtbReport.ScrollBars = RichTextBoxScrollBars.Vertical; // يظهر شريط تصفح جانبي فقط إذا صغرت الشاشة عمودياً
            rtbReport.Name = "rtbReport";

            // إضافة العناصر بترتيبها الصحيح داخل الحاوية اليسرى
            sidePanel.Controls.Add(rtbReport);
            sidePanel.Controls.Add(spacer);
            sidePanel.Controls.Add(listSystems);

            // 4. منطقة العرض الثلاثي الأبعاد (تتوسع وتتقلص تلقائياً وبشكل مرن مع حركة النافذة)
            panel3D = new Panel();
            panel3D.Dock = DockStyle.Fill; // سيمتص كل المساحة المتبقية في الشاشة فوراً عند التكبير
            panel3D.BackColor = Color.FromArgb(12, 12, 14); 
            panel3D.Paint += Panel3D_Paint;
            panel3D.MouseDown += Panel3D_MouseDown;
            panel3D.MouseMove += Panel3D_MouseMove;
            panel3D.SizeChanged += Panel3D_SizeChanged; // حدث هام لإعادة رسم المجسم بأبعاده الجديدة فوراً

            // إضافة الحاويات للـ Form
            this.Controls.Add(panel3D);
            this.Controls.Add(sidePanel);
        }

        // هذا التابع يضمن إعادة رسم المجسم وتحديث مركز الأبعاد فور قيام المستخدم بسحب وتغيير حجم الواجهة
        private void Panel3D_SizeChanged(object sender, EventArgs e)
        {
            panel3D.Invalidate(); 
        }     
        private void UpdateSyncReport(Color targetColor)
        {
            var hsv = HsvConverter.FromRgb(targetColor);
            var ycbcr = YcbcrConverter.FromRgb(targetColor);
            var yuv = YuvConverter.FromRgb(targetColor);
            var lab = LabConverter.FromRgb(targetColor);
            var cmyk = CmykConverter.FromRgb(targetColor);

            // صياغة النص بشكل عمودي منظم ومتناسق مع الحجم الصغير
            cachedReportText = $"📊 تقرير المزامنة اللحظي\n" +
                            $"━━━━━━━━━━━━━━━━━━━━━\n" +
                            $"RGB  → R:{targetColor.R} G:{targetColor.G} B:{targetColor.B}\n\n" +
                            $"HSV  → H:{hsv.Hue:0}° S:{hsv.Saturation * 100:0}% V:{hsv.Value * 100:0}%\n\n" +
                            $"YCbCr→ Y:{ycbcr.Y:0} Cb:{ycbcr.Cb:0} Cr:{ycbcr.Cr:0}\n\n" +
                            $"YUV  → Y:{yuv.Y * 255:0} U:{yuv.U:0.0} V:{yuv.V:0.0}\n\n" +
                            $"Lab  → L*:{lab.L:0.0} a*:{lab.A:0.0} b*:{lab.B:0.0}\n\n" +
                            $"CMYK → C:{cmyk.C * 100:0}% M:{cmyk.M * 100:0}% Y:{cmyk.Y * 100:0}% K:{cmyk.K * 100:0}%";

            // تحديث أداة الـ Label الموجودة داخل الـ sidePanel
            if (this.Controls.Find("lblReport", true).FirstOrDefault() is Label reportLabel)
            {
                reportLabel.Text = cachedReportText;
            }
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
                    RgbCubeRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, GetCurrentZoom(), localSelectedColor);
                }
                else if (selectedSystem == "HSV Cone")
                {
                    HsvConeRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, GetCurrentZoom(), localSelectedColor);
                }
                else if (selectedSystem == "YCbCr Space")
                {
                    YCbCrSpaceRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, GetCurrentZoom(), localSelectedColor);
                }
                else if (selectedSystem == "YUV Space")
                {
                    YuvSpaceRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, GetCurrentZoom(), localSelectedColor);
                }
                else if (selectedSystem == "Lab Space")
                {
                    LabRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, GetCurrentZoom(), localSelectedColor);
                }
                else if (selectedSystem == "CMYK Space")
                {
                    CmykRenderer.Render(g, panel3D.Width, panel3D.Height, angleX, angleY, GetCurrentZoom());
                }
                // تم إزالة استدعاء دالة رسم النص من هنا ليبقى الفضاء ثلاثي الأبعاد نقيّاً
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Paint Error: " + ex.Message);
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
        // استبدل التعريف القديم بهذا القاموس
        private Dictionary<string, float> systemZooms = new Dictionary<string, float>
        {
            { "RGB Cube", 1.0f },
            { "YUV", 1.0f },
            { "Lab", 1.0f },
            { "HSV", 1.0f },
            { "YCbCr", 1.0f },
            { "CMYK", 1.0f }
        };

        // تابع بسيط لجلب الزوم الخاص بالنظام الحالي المختار
        private float GetCurrentZoom() 
        {
            return systemZooms.ContainsKey(selectedSystem) ? systemZooms[selectedSystem] : 1.0f;
        }

        private void Panel3D_MouseWheel(object sender, MouseEventArgs e)
        {
            float currentZoom = GetCurrentZoom();
    
            if (e.Delta > 0) currentZoom += 0.1f;
            else currentZoom -= 0.1f;

            // حفظ الزوم للنظام الحالي فقط
            systemZooms[selectedSystem] = Math.Max(0.3f, Math.Min(3.0f, currentZoom));
            
            panel3D.Invalidate();
        }
        private void ListSystems_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = isSelected ? Color.FromArgb(0, 122, 204) : Color.FromArgb(24, 24, 28);
            Color textColor = isSelected ? Color.White : Color.Gray;

            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);
            
            // رسم النص في المنتصف عمودياً
            TextRenderer.DrawText(e.Graphics, listSystems.Items[e.Index].ToString(), 
                listSystems.Font, e.Bounds, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            
            if (isSelected) // إضافة خط تمييز صغير
            {
                e.Graphics.FillRectangle(Brushes.Cyan, e.Bounds.X, e.Bounds.Y, 4, e.Bounds.Height);
            }
        }
        
    }
}