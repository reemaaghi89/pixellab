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

        // private double angleX = 35.0; 
        // private double angleY = 45.0; 
        // private Point lastMousePos;  
        private Color selectedPixelColor = Color.FromArgb(255, 255, 255); 
        private string currentColorSpace = "RGB";
        private System.Windows.Forms.Timer livePreviewTimer;
        private FormSpaces _spaceForm;
        public Form1()
        {
            InitializeComponent();
            BuildColorControls("RGB");
            InitializeLivePreview();
            ApplyDarkTheme();


            new DraggablePanelHandler(panelInspector);
            new DraggablePanelHandler(lblInspectorInfo);
            
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, panelCube, new object[] { true });
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.ForeColor = Color.White;

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
            if (_spaceForm == null || _spaceForm.IsDisposed)
            {
                _spaceForm = new FormSpaces();
            }
            
            _spaceForm.Show();
            _spaceForm.BringToFront();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null || currentImage == null) return;

            var (selectedColor, infoReport) = AnalyzeClickedPixelClass.AnalyzeClickedPixel(currentImage, pictureBox1.Width, pictureBox1.Height, e);
            if (selectedColor.HasValue)
            {
                Color pixelColor = selectedColor.Value;
                panelSelectedColor.BackColor = pixelColor;
                selectedPixelColor = pixelColor;
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

        private void btnQuantizeColors_Click(object sender, EventArgs e)
        {
            if (!HasImage()) return;
            Bitmap quantizedResult = ImageProcessor.QuantizeImageColors((Bitmap)currentImage, levels: 4);
            currentImage = quantizedResult;
            pictureBox1.Image = currentImage;
        }

        private void btnConvertGrayscale_Click(object sender, EventArgs e)
        {
            if (!HasImage()) return;
            currentImage = ImageProcessor.ConvertToGrayscale((Bitmap)currentImage);
            pictureBox1.Image = currentImage;
        }

        private void btnConvertBlackAndWhite_Click(object sender, EventArgs e)
        {
            if (!HasImage()) return;
            currentImage = ImageProcessor.ConvertToBlackAndWhite((Bitmap)currentImage);
            pictureBox1.Image = currentImage;
        }


        private void cmbColorSpaces_SelectedIndexChanged(object sender,EventArgs e)
        {
            string selected =cmbColorSpaces.SelectedItem.ToString();
            BuildColorControls(selected);
        }
        private void BuildColorControls(string systemName)
        {
            currentColorSpace = systemName;
            ColorAdjustmentInterface.InitializeSliders(flowColorControls, systemName, ChannelTrackChanged);
        }

        private void InitializeLivePreview()
        {
            livePreviewTimer =new System.Windows.Forms.Timer();
            livePreviewTimer.Interval = 120;
            livePreviewTimer.Tick +=LivePreviewTimer_Tick;
        }
        private void LivePreviewTimer_Tick(object sender, EventArgs e)
        {
            livePreviewTimer.Stop();
            ImageEffectApplier.Apply(currentColorSpace, originalImage, flowColorControls, pictureBox1);
        }
        private bool isUpdatingFrom3D = false;
        private void ChannelTrackChanged(object sender,EventArgs e)
        {
            livePreviewTimer.Stop();

            livePreviewTimer.Start();
            if (isUpdatingFrom3D) return;

            Color calculatedColor = Color.White; 
            if (currentColorSpace == "RGB")
            {
                int r = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Red");
                int g = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Green");
                int b = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Blue");
                calculatedColor = Color.FromArgb(r, g, b);
            }
            else if (currentColorSpace == "HSV")
            {
                double h = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Hue");
                double s = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Saturation") / 100.0;
                double v = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Value") / 100.0;
                calculatedColor = HsvConverter.ToRgb(h, s, v); // استخدام المحول الخاص بكِ
            }
            else if (currentColorSpace == "LAB")
            {
                double l = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "L");
                double a = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "A");
                double b = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "B");
                calculatedColor = LabConverter.ToRgb(l, a, b); // استخدام المحول الخاص بكِ
            }
            else if (currentColorSpace == "CMYK")
            {
                double c = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Cyan") / 100.0;
                double m = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Magenta") / 100.0;
                double y = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Yellow") / 100.0;
                double k = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Black") / 100.0;
                calculatedColor = CmykConverter.ToRgb(c, m, y, k);
            }
            else if (currentColorSpace == "YUV")
            {
                // لاحظي هنا بنقسم الـ Y على 255 إذا كان المحول عندك بيتعامل مع مجال 0-1
                double y = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Y") / 255.0;
                double u = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "U");
                double v = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "V");
                calculatedColor = YuvConverter.ToRgb(y, u, v);
            }
            else if (currentColorSpace == "YCbCr")
            {
                double y = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Y");
                double cb = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Cb");
                double cr = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Cr");
                calculatedColor = YcbcrConverter.ToRgb(y, cb, cr);
            }
            
            panelSelectedColor.BackColor = calculatedColor;

            if (_spaceForm != null && !_spaceForm.IsDisposed)
            {
                _spaceForm.UpdateFromForm1(calculatedColor);
            }
        }
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////  
        ///         
        /// </summary>
    
    // ////////////////////////////////////////////////////////////////////////////
    // /// /////////////////////////////////////////////////////////////////////////////
    // /// 
    // ///// من المجسم للسلايدرات    
        public void UpdateSlidersFrom3D(Color pickedColor, string systemName)
        {
            livePreviewTimer.Stop();
            isUpdatingFrom3D = true;
            
            panelSelectedColor.BackColor = pickedColor;

            if (systemName == "RGB Cube" || systemName == "RGB")
            {
                UpdateSingleSlider("Red", pickedColor.R);
                UpdateSingleSlider("Green", pickedColor.G);
                UpdateSingleSlider("Blue", pickedColor.B);
            }
            else if (systemName == "HSV Cone" || systemName == "HSV")
            {
                var hsv = HsvConverter.FromRgb(pickedColor);
                UpdateSingleSlider("Hue", (int)hsv.Hue);
                UpdateSingleSlider("Saturation", (int)(hsv.Saturation * 100));
                UpdateSingleSlider("Value", (int)(hsv.Value * 100));
            }
            else if (systemName == "Lab Space" || systemName == "LAB")
            {
                var lab = LabConverter.FromRgb(pickedColor);
                UpdateSingleSlider("L", (int)lab.L);
                UpdateSingleSlider("A", (int)lab.A);
                UpdateSingleSlider("B", (int)lab.B);
            }
            else if (systemName == "CMYK Space" || systemName == "CMYK")
            {
                var cmyk = CmykConverter.FromRgb(pickedColor); // تأكدي من وجود FromRgb عندك
                UpdateSingleSlider("Cyan", (int)(cmyk.C * 100));
                UpdateSingleSlider("Magenta", (int)(cmyk.M * 100));
                UpdateSingleSlider("Yellow", (int)(cmyk.Y * 100));
                UpdateSingleSlider("Black", (int)(cmyk.K * 100));
            }
            else if (systemName == "YUV Space" || systemName == "YUV")
            {
                var yuv = YuvConverter.FromRgb(pickedColor);
                UpdateSingleSlider("Y", (int)(yuv.Y * 255));
                UpdateSingleSlider("U", (int)yuv.U);
                UpdateSingleSlider("V", (int)yuv.V);
            }
            else if (systemName == "YCbCr Space" || systemName == "YCbCr")
            {
                var ycc = YcbcrConverter.FromRgb(pickedColor);
                UpdateSingleSlider("Y", (int)ycc.Y);
                UpdateSingleSlider("Cb", (int)ycc.Cb);
                UpdateSingleSlider("Cr", (int)ycc.Cr);
            }

            livePreviewTimer.Start();
            isUpdatingFrom3D = false;
        }

        private void UpdateSingleSlider(string name, int value)
        {
            foreach (Control ctrl in flowColorControls.Controls)
            {
                if (ctrl is ChannelControl cc && cc.lblName.Text == name)
                {
                    cc.track.Value = value; 
                    cc.lblValue.Text = value.ToString(); // تحديث النص
                }
            }
        }


    }
}