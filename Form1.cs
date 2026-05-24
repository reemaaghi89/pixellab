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
        private FormSpaces _spaceForm;
        private bool isSyncingSystemFrom3D = false;
        private bool isGrayscaleActive = false;
        private bool isBlackAndWhiteActive = false;
        private bool isQuantizeActive = false;
        private ColorSystemManager _colorManager;

        public Form1()
        {
            InitializeComponent();
            BuildColorControls("RGB");
            InitializeLivePreview();
            ApplyDarkTheme();


            new DraggablePanelHandler(panelInspector);
            new DraggablePanelHandler(lblInspectorInfo);


            _colorManager = new ColorSystemManager(flowColorControls);

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

            lblInfo.BackColor = Color.FromArgb(30, 30, 30);
            lblInfo.ForeColor = Color.White;
            lblInfo.BorderStyle = BorderStyle.None;


            pictureBox1.BackColor = Color.FromArgb(35, 35, 35);

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
                panelCube.Invalidate(); 
            }
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;
            livePreviewTimer.Stop();
            if (currentImage != null) currentImage.Dispose();
            currentImage = new Bitmap(originalImage);
            pictureBox1.Image = currentImage;

            selectedPixelColor = Color.FromArgb(255, 255, 255);
            panelSelectedColor.BackColor = Color.White;
            panelCube.Invalidate();

            isGrayscaleActive = false;
            isBlackAndWhiteActive = false;
            isQuantizeActive = false;
            
            btnGray.BackColor = Color.FromArgb(45, 45, 48); 
            btnQuantize.BackColor = Color.FromArgb(45, 45, 48); 
        
            BuildColorControls(currentColorSpace);

            pictureBox1.Refresh();
        }       
        private void btnsave_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG|*.png|JPEG|*.jpg|Bitmap|*.bmp";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Bitmap imageToSave = new Bitmap(pictureBox1.Image))
                    {
                        string extension = Path.GetExtension(sfd.FileName).ToLower();
                        System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;

                        if (extension == ".jpg" || extension == ".jpeg")
                            format = System.Drawing.Imaging.ImageFormat.Jpeg;
                        else if (extension == ".bmp")
                            format = System.Drawing.Imaging.ImageFormat.Bmp;

                        imageToSave.Save(sfd.FileName, format);
                    }
                    MessageBox.Show("تم حفظ الصورة بنجاح مع كافة التعديلات!", "نجاح العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("حدث خطأ أثناء حفظ الصورة: " + ex.Message, "خطأ في الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnOpen3DLab_Click(object sender, EventArgs e)
        {
            if (_spaceForm == null || _spaceForm.IsDisposed)
            {
                _spaceForm = new FormSpaces();
            }
            _spaceForm.Owner = this;
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
                // _colorManager.UpdateSlidersFrom3D(pixelColor, currentColorSpace);
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
            
            try
            {
                if (string.IsNullOrWhiteSpace(NumberOfColors.Text))
                {
                    isQuantizeActive = false;
                    btn.BackColor = Color.FromArgb(45, 45, 48);
                }
                else
                {
                    if (!int.TryParse(NumberOfColors.Text, out int myLevels) || myLevels < 3 || myLevels > 255)
                    {
                        MessageBox.Show("الرجاء إدخال رقم صحيح بين 3 و 255", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    isQuantizeActive = true;
                    btn.BackColor = Color.FromArgb(0, 122, 204);
                }

                livePreviewTimer.Stop();
                livePreviewTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ غير متوقع أثناء معالجة الألوان: " + ex.Message, "خطأ تقني", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConvertGrayscale_Click(object sender, EventArgs e)
        {
            if (!HasImage()) return;
            Button btn = (Button)sender;

            if (!isGrayscaleActive)
            {
                isGrayscaleActive = true;
                btn.BackColor = Color.FromArgb(0, 122, 204); 
            }
            else
            {
                isGrayscaleActive = false;
                btn.BackColor = Color.FromArgb(45, 45, 48);
            }

            livePreviewTimer.Stop();
            livePreviewTimer.Start();
        }
        private void btnConvertBlackAndWhite_Click(object sender, EventArgs e)
        {
            if (!HasImage()) return;
            Button btn = (Button)sender;

            if (!isBlackAndWhiteActive)
            {
                isBlackAndWhiteActive = true;
                btn.BackColor = Color.FromArgb(0, 122, 204);
            }
            else
            {
                isBlackAndWhiteActive = false;
                btn.BackColor = Color.FromArgb(45, 45, 48);
            }

            livePreviewTimer.Stop();
            livePreviewTimer.Start();
        }
        //===========================================================
        private void cmbColorSpaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSyncingSystemFrom3D) return; 

            string selectedShortName = cmbColorSpaces.SelectedItem.ToString();
            
            BuildColorControls(selectedShortName);

            if (_spaceForm != null && !_spaceForm.IsDisposed)
            {
                string fullName = selectedShortName;
                if (selectedShortName == "RGB") fullName = "RGB Cube";
                else if (selectedShortName == "HSV") fullName = "HSV Cone";
                else if (selectedShortName == "LAB" || selectedShortName == "Lab") fullName = "Lab Space";
                else if (selectedShortName == "CMYK") fullName = "CMYK Space";
                else if (selectedShortName == "YUV") fullName = "YUV Space";
                else if (selectedShortName == "YCbCr") fullName = "YCbCr Space";

                _spaceForm.listSystems.SelectedItem = fullName;
            }
        }

        private void LivePreviewTimer_Tick(object sender, EventArgs e)
        {
            livePreviewTimer.Stop();
            if (originalImage == null) return;

            Bitmap processedBmp = new Bitmap(originalImage);

            if (isGrayscaleActive)
            {
                Bitmap temp = ImageProcessor.ConvertToGrayscale(processedBmp);
                processedBmp.Dispose();
                processedBmp = temp;
            }

            if (isBlackAndWhiteActive)
            {
                Bitmap temp = ImageProcessor.ConvertToBlackAndWhite(processedBmp);
                processedBmp.Dispose();
                processedBmp = temp;
            }

            if (isQuantizeActive)
            {
                if (int.TryParse(NumberOfColors.Text, out int myLevels) && myLevels >= 3 && myLevels <= 255)
                {
                    Bitmap temp = ImageProcessor.QuantizeImageColors(processedBmp, myLevels);
                    processedBmp.Dispose();
                    processedBmp = temp;
                }
            }

            ImageEffectApplier.Apply(currentColorSpace, processedBmp, flowColorControls, pictureBox1);

            if (currentImage != null) currentImage.Dispose();
            currentImage = processedBmp;
        }       
         private void InitializeLivePreview()
        {
            livePreviewTimer =new System.Windows.Forms.Timer();
            livePreviewTimer.Interval = 120;
            livePreviewTimer.Tick +=LivePreviewTimer_Tick;
        }
        

        private void BuildColorControls(string systemName)
        {
            currentColorSpace = systemName;
            ColorAdjustmentInterface.InitializeSliders(flowColorControls, systemName, ChannelTrackChanged);
        }

        private void ChannelTrackChanged(object sender, EventArgs e)
        {
            livePreviewTimer.Stop();
            livePreviewTimer.Start();

            if (_colorManager.IsUpdatingFrom3D) return;

            Color calculatedColor = _colorManager.GetColorFromSliders(currentColorSpace);

            panelSelectedColor.BackColor = calculatedColor;

            if (_spaceForm != null && !_spaceForm.IsDisposed)
            {
                _spaceForm.UpdateFromForm1(calculatedColor);
            }
        }
        
        public void UpdateSlidersFrom3D(Color pickedColor, string systemName)
        {
            if (_colorManager != null)
            {
                _colorManager.IsUpdatingFrom3D = true;
                _colorManager.UpdateSlidersFrom3D(pickedColor, systemName);
                _colorManager.IsUpdatingFrom3D = false;
                
                panelSelectedColor.BackColor = pickedColor;

                livePreviewTimer.Stop();
                livePreviewTimer.Start();
            }
        }
  
        
        //=============================================================

        public void SetSelectedSystemFrom3D(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return;
            string shortName = fullName.Split(' ')[0].ToLower(); 

            isSyncingSystemFrom3D = true;

            foreach (var item in cmbColorSpaces.Items)
            {
                string itemText = item.ToString().ToLower();
                
                if (itemText.Contains(shortName))
                {
                    cmbColorSpaces.SelectedItem = item;
                    BuildColorControls(item.ToString());
                    break; 
                }
            }
            isSyncingSystemFrom3D = false;
            livePreviewTimer.Stop();
            livePreviewTimer.Start();
        }
       
    }
}