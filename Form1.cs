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
        private Color selectedPixelColor = Color.FromArgb(255, 255, 255);
        private string currentColorSpace = "RGB";
        private System.Windows.Forms.Timer livePreviewTimer;
        private bool isGrayscaleActive = false;
        private bool isBlackAndWhiteActive = false;
        private bool isQuantizeActive = false;

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


        // private void btnReset_Click(object sender, EventArgs e)
        // {
        //     if (originalImage == null) return;

        //     currentImage = new Bitmap(originalImage);
        //     pictureBox1.Image = currentImage;

        //     selectedPixelColor = Color.FromArgb(255, 255, 255);
        //     panelCube.Invalidate();
        // }
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;

            currentImage = new Bitmap(originalImage);
            pictureBox1.Image = currentImage;
            selectedPixelColor = Color.FromArgb(255, 255, 255);
            panelCube.Invalidate();
            isGrayscaleActive = false;
            isBlackAndWhiteActive = false;
            isQuantizeActive = false;
            btn.BackColor = Color.FromArgb(45, 45, 48); 
            btnGray.BackColor = Color.FromArgb(45, 45, 48); 
            btnQuantize.BackColor = Color.FromArgb(45, 45, 48); 
            // =========================================================================
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
                UpdateSlidersFromColor(pixelColor);
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
            Button btn = (Button)sender;
            if (!isQuantizeActive)
            {
                Bitmap quantizedResult = ImageProcessor.QuantizeImageColors((Bitmap)currentImage, levels: 4);
                currentImage = quantizedResult;
                pictureBox1.Image = currentImage;
                isQuantizeActive = true;
                btn.BackColor = Color.FromArgb(0, 122, 204);
            }
            else
            {
                btnReset_Click(null, null);
                isQuantizeActive = false;
                btn.BackColor = Color.FromArgb(45, 45, 48);
            }
        }
        private void btnConvertGrayscale_Click(object sender, EventArgs e)
        {
            if (!HasImage()) return;
            Button btn = (Button)sender;

            if (!isGrayscaleActive)
            {
                currentImage = ImageProcessor.ConvertToGrayscale((Bitmap)currentImage);
                pictureBox1.Image = currentImage;
                isGrayscaleActive = true;
                btn.BackColor = Color.FromArgb(0, 122, 204); 
            }
            else
            {
                btnReset_Click(null, null); 
                isGrayscaleActive = false;
                btn.BackColor = Color.FromArgb(45, 45, 48);
            }
        }
        private void btnConvertBlackAndWhite_Click(object sender, EventArgs e)
        {
            if (!HasImage()) return;
            Button btn = (Button)sender;
            if (!isBlackAndWhiteActive)
            {
                currentImage = ImageProcessor.ConvertToBlackAndWhite((Bitmap)currentImage);
                pictureBox1.Image = currentImage;
                isBlackAndWhiteActive = true;
                btn.BackColor = Color.FromArgb(0, 122, 204);
            }
            else
            {
                btnReset_Click(null, null);
                isBlackAndWhiteActive = false;
                btn.BackColor = Color.FromArgb(45, 45, 48);
            }
        }


        //==========================================================

        private void ChannelTrackChanged(object sender, EventArgs e)
        {
            livePreviewTimer.Stop();

            livePreviewTimer.Start();
        }
        private void cmbColorSpaces_SelectedIndexChanged(object sender, EventArgs e)
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

        private void LivePreviewTimer_Tick(object sender, EventArgs e)
        {
            livePreviewTimer.Stop();

            ImageEffectApplier.Apply(currentColorSpace, originalImage, flowColorControls, pictureBox1);
        }

        private void BuildColorControls(string systemName)
        {
            currentColorSpace = systemName;
            ColorAdjustmentInterface.InitializeSliders(flowColorControls, systemName, ChannelTrackChanged);
        }

        private void UpdateSlidersFromColor(Color color)
        {
            if (flowColorControls.Controls.Count == 0) return;
            livePreviewTimer.Stop();
            switch (currentColorSpace)
            {
                case "RGB":
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Red", color.R);
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Green", color.G);
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Blue", color.B);
                    break;

                case "HSV":
                    var hsv = pixellab.Converters.HsvConverter.FromRgb(color); // استدعاء المحول المكتوب مسبقاً
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Hue", (int)Math.Round(hsv.Hue));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Saturation", (int)Math.Round(hsv.Saturation * 100));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Value", (int)Math.Round(hsv.Value * 100));
                    break;

                case "CMYK":
                    var cmyk = pixellab.Converters.CmykConverter.FromRgb(color);
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Cyan", (int)Math.Round(cmyk.C * 100));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Magenta", (int)Math.Round(cmyk.M * 100));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Yellow", (int)Math.Round(cmyk.Y * 100));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Black", (int)Math.Round(cmyk.K * 100));
                    break;

                case "LAB":
                    var lab = pixellab.Converters.LabConverter.FromRgb(color);
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "L", (int)Math.Round(lab.L));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "A", (int)Math.Round(lab.A));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "B", (int)Math.Round(lab.B));
                    break;

                case "YUV":
                    var yuv = pixellab.Converters.YuvConverter.FromRgb(color);
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Y", (int)Math.Round(yuv.Y));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "U", (int)Math.Round(yuv.U));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "V", (int)Math.Round(yuv.V));
                    break;

                case "YCbCr":
                    var ycbcr = pixellab.Converters.YcbcrConverter.FromRgb(color);
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Y", (int)Math.Round(ycbcr.Y));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Cb", (int)Math.Round(ycbcr.Cb));
                    ColorAdjustmentInterface.SetSliderValue(flowColorControls, "Cr", (int)Math.Round(ycbcr.Cr));
                    break;
            }
        }

    }
}