using System;
using System.Drawing;
using System.Windows.Forms;

namespace pixellab
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private PictureBox pictureBox1;

        private Button btnopen;
        private Button btnsave;
        private Button btnReset;
        private TextBox NumberOfColors;
        private Panel panelResetRow;
        private Button btnGray;
        private Button btnQuantize;
        private Button btn;
        private Button btnOpen3DLab;

        private Label lblInfo;
        private Label lblInspectorInfo;
        private Label lblColorTitle;

        private Label lblLogo;
        private Label lblSubtitle;

        private Label lblImageSection;
        private Label lblFiltersSection;
        private Label lblExperimentalSection;

        private Panel panelSelectedColor;

        private Panel panelSidebar;
        private Panel panelMain;
        private Panel panelTop;
        private Panel panelInspector;
        private Panel panelColorCard;
        private Panel panelImageContainer;

        private FlowLayoutPanel flowSidebar;
        private FlowLayoutPanel flowColorControls;

        private ComboBox cmbColorSpaces;
        private Panel panelCube;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pictureBox1 = new PictureBox();

            this.btnopen = new Button();
            this.btnsave = new Button();
            this.btnReset = new Button();
            this.NumberOfColors = new TextBox();
            this.panelResetRow = new Panel();

            this.btnGray = new Button();
            this.btnQuantize = new Button();
            this.btn = new Button();

            this.btnOpen3DLab = new Button();

            this.lblInfo = new Label();
            this.lblInspectorInfo = new Label();
            this.lblColorTitle = new Label();

            this.lblLogo = new Label();
            this.lblSubtitle = new Label();

            this.lblImageSection = new Label();
            this.lblFiltersSection = new Label();
            this.lblExperimentalSection = new Label();

            this.panelSelectedColor = new Panel();

            this.panelSidebar = new Panel();
            this.panelMain = new Panel();
            this.panelTop = new Panel();
            this.panelInspector = new Panel();
            this.panelColorCard = new Panel();
            this.panelImageContainer = new Panel();

            this.flowSidebar = new FlowLayoutPanel();
            this.flowColorControls = new FlowLayoutPanel();

            this.cmbColorSpaces = new ComboBox();
            this.panelCube = new Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();

            this.SuspendLayout();

            // =====================================================
            // FORM
            // =====================================================

            this.AllowDrop = true;

            this.AutoScaleDimensions =
                new SizeF(6F, 13F);

            this.AutoScaleMode =
                AutoScaleMode.Font;

            this.ClientSize =
                new Size(1400, 750);

            this.BackColor =
                Color.FromArgb(18, 18, 18);

            this.Text = "pixellab";

            this.DragDrop +=
                new DragEventHandler(this.Form1_DragDrop);

            this.DragEnter +=
                new DragEventHandler(this.Form1_DragEnter);

            // =====================================================
            // SIDEBAR
            // =====================================================

            this.panelSidebar.Dock = DockStyle.Left;
            this.panelSidebar.Width = 220;
            this.panelSidebar.BackColor = Color.FromArgb(24, 24, 24);

            // =====================================================
            // FLOW SIDEBAR
            // =====================================================

            this.flowSidebar.Dock = DockStyle.Fill;

            this.flowSidebar.FlowDirection = FlowDirection.TopDown;

            this.flowSidebar.WrapContents = false;

            this.flowSidebar.AutoScroll = true;

            this.flowSidebar.Padding = new Padding(15);

            this.flowSidebar.BackColor = Color.FromArgb(24, 24, 24);

            // =====================================================
            // TOP PANEL
            // =====================================================

            this.panelTop.Dock = DockStyle.Top;

            this.panelTop.Height = 55;

            this.panelTop.BackColor = Color.FromArgb(30, 30, 30);

            // =====================================================
            // MAIN PANEL
            // =====================================================

            this.panelMain.Dock = DockStyle.Fill;

            this.panelMain.BackColor = Color.FromArgb(18, 18, 18);

            this.panelMain.Padding = new Padding(15);

            // =====================================================
            // INSPECTOR
            // =====================================================

            this.panelInspector.Dock = DockStyle.Right;

            this.panelInspector.Width = 300;

            this.panelInspector.BackColor = Color.FromArgb(24, 24, 24);

            this.panelInspector.Padding = new Padding(15);

            // =====================================================
            // IMAGE CONTAINER
            // =====================================================

            this.panelImageContainer.Dock = DockStyle.Fill;

            this.panelImageContainer.Padding = new Padding(10);

            this.panelImageContainer.BackColor = Color.FromArgb(18, 18, 18);

            // =====================================================
            // PICTUREBOX
            // =====================================================

            this.pictureBox1.Dock = DockStyle.Fill;

            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            this.pictureBox1.BackColor = Color.FromArgb(22, 22, 22);

            this.pictureBox1.BorderStyle = BorderStyle.None;

            this.pictureBox1.MouseClick += new MouseEventHandler(this.pictureBox1_MouseClick);

            // =====================================================
            // LOGO
            // =====================================================

            this.lblLogo.Text = "PIXELLAB";

            this.lblLogo.ForeColor = Color.White;

            this.lblLogo.Font =
                new Font("Segoe UI", 20, FontStyle.Bold);

            this.lblLogo.AutoSize = true;

            // =====================================================
            // SUBTITLE
            // =====================================================

            this.lblSubtitle.Text =
                "Image Processing Studio";

            this.lblSubtitle.ForeColor =
                Color.Gray;

            this.lblSubtitle.Font =
                new Font("Segoe UI", 9);

            this.lblSubtitle.AutoSize = true;

            this.lblSubtitle.Margin =
                new Padding(0, 0, 0, 25);

            // =====================================================
            // SECTION LABELS
            // =====================================================

            this.lblImageSection.Text = "IMAGE";
            this.lblFiltersSection.Text = "FILTERS";
            // this.lblExperimentalSection.Text = "EXPERIMENTAL";

            Label[] sectionLabels =
            {
                this.lblImageSection,
                this.lblFiltersSection,
                // this.lblExperimentalSection
            };

            foreach (Label lbl in sectionLabels)
            {
                lbl.ForeColor = Color.DarkGray;

                lbl.Font =
                    new Font("Segoe UI", 9, FontStyle.Bold);

                lbl.AutoSize = true;

                lbl.Margin =
                    new Padding(0, 15, 0, 10);
            }
            //
            //PanelReset
            //
            this.panelResetRow.Width = 185;
            this.panelResetRow.Height = 36;
            this.panelResetRow.BackColor = Color.Transparent;
            this.panelResetRow.Margin = new Padding(0, 0, 0, 10);
            // =====================================================
            // BUTTONS
            // =====================================================

            Button[] buttons =
            {
                btnopen,
                btnsave,
                btn,
                btnGray,
                btnQuantize
            };

            foreach (Button b in buttons)
            {
                b.Width = 185;
                b.Height = 42;

                b.FlatStyle = FlatStyle.Flat;

                b.FlatAppearance.BorderSize = 0;

                b.BackColor =
                    Color.FromArgb(40, 40, 45);

                b.ForeColor = Color.White;

                b.Font =
                    new Font("Segoe UI", 10, FontStyle.Bold);

                b.Cursor = Cursors.Hand;

                b.Margin =
                    new Padding(0, 0, 0, 10);
            }
            this.btnReset.Width = 40;
            this.btnReset.Height = 36;
            this.btnReset.FlatStyle = FlatStyle.Flat;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.BackColor = Color.FromArgb(40, 40, 45);
            this.btnReset.ForeColor = Color.White;
            this.btnReset.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            this.btnReset.Dock = DockStyle.Left;

            this.NumberOfColors.Dock = DockStyle.Fill;

            this.panelResetRow.Height = 36;

            this.NumberOfColors.BackColor = Color.FromArgb(40, 40, 45);

            this.NumberOfColors.ForeColor = Color.White;

            this.NumberOfColors.BorderStyle = BorderStyle.FixedSingle;

            this.NumberOfColors.Font = new Font("Segoe UI", 10);

            this.NumberOfColors.Text = "";
            this.NumberOfColors.Margin = new Padding(5, 0, 0, 0);

            // =====================================================
            // BUTTON TEXT
            // =====================================================

            this.btnopen.Text = "Open Image";
            this.btnsave.Text = "Save Image";
            this.btnReset.Text = "Reset";

            this.btn.Text = "Gray Scale";
            this.btnGray.Text = "Black & White";
            this.btnQuantize.Text = "Reduce Colors";

            this.btnOpen3DLab.Text = "🚀 3D Lab";

            // =====================================================
            // EVENTS
            // =====================================================

            this.btnopen.Click +=
                new EventHandler(this.btnopen_Click);

            this.btnsave.Click +=
                new EventHandler(this.btnsave_Click);

            this.btnReset.Click +=
                new EventHandler(this.btnReset_Click);

            this.btn.Click +=
                new EventHandler(this.btnConvertGrayscale_Click);

            this.btnGray.Click +=
                new EventHandler(this.btnConvertBlackAndWhite_Click);

            this.btnQuantize.Click +=
                new EventHandler(this.btnQuantizeColors_Click);

            this.btnOpen3DLab.Click +=
                new EventHandler(this.btnOpen3DLab_Click);

            // =====================================================
            // 3D BUTTON STYLE
            // =====================================================

            this.btnOpen3DLab.Width = 185;
            this.btnOpen3DLab.Height = 45;

            this.btnOpen3DLab.FlatStyle =
                FlatStyle.Flat;

            this.btnOpen3DLab.FlatAppearance.BorderSize = 0;

            this.btnOpen3DLab.BackColor =
                Color.FromArgb(0, 122, 204);

            this.btnOpen3DLab.ForeColor =
                Color.White;
            this.btnOpen3DLab.Dock = DockStyle.Top;

            this.btnOpen3DLab.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            // =====================================================
            // INFO LABEL
            // =====================================================

            this.lblInfo.Size =
                new Size(230, 70);

            this.lblInfo.ForeColor =
                Color.Gainsboro;

            this.lblInfo.BackColor =
                Color.FromArgb(30, 30, 30);

            this.lblInfo.Font =
                new Font("Segoe UI", 9);

            this.lblInfo.Padding =
                new Padding(10);

            this.lblInfo.Text =
                "No image loaded";

            // =====================================================
            // COLOR CARD
            // =====================================================

            this.panelColorCard.Width = 190;

            this.panelColorCard.Height = 380;

            this.panelColorCard.BackColor =
                Color.FromArgb(32, 32, 32);

            this.panelColorCard.Padding =
                new Padding(10);

            this.panelColorCard.Margin =
                new Padding(0, 5, 0, 10);

            // =====================================================
            // COLOR TITLE
            // =====================================================

            this.lblColorTitle.Text =
                "Color Space";

            this.lblColorTitle.ForeColor =
                Color.White;

            this.lblColorTitle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            this.lblColorTitle.AutoSize = true;

            // =====================================================
            // COMBOBOX
            // =====================================================

            this.cmbColorSpaces.Width = 165;

            this.cmbColorSpaces.DropDownStyle =
                ComboBoxStyle.DropDownList;

            this.cmbColorSpaces.BackColor =
                Color.FromArgb(45, 45, 48);

            this.cmbColorSpaces.ForeColor =
                Color.White;

            this.cmbColorSpaces.FlatStyle =
                FlatStyle.Flat;

            this.cmbColorSpaces.Items.AddRange(new object[]
            {
                "RGB",
                "HSV",
                "CMYK",
                "LAB",
                "YUV",
                "YCbCr"
            });

            this.cmbColorSpaces.SelectedIndex = 0;

            this.cmbColorSpaces.Location =
                new Point(10, 45);

            this.cmbColorSpaces.SelectedIndexChanged +=
                new EventHandler(this.cmbColorSpaces_SelectedIndexChanged);

            // =====================================================
            // FLOW COLOR CONTROLS
            // =====================================================

            this.flowColorControls.Location =
                new Point(10, 90);

            this.flowColorControls.Width = 170;

            this.flowColorControls.Height = 320;

            this.flowColorControls.FlowDirection =
                FlowDirection.TopDown;

            this.flowColorControls.WrapContents = false;

            this.flowColorControls.BackColor =
                Color.FromArgb(32, 32, 32);

            // =====================================================
            // INSPECTOR INFO
            // =====================================================

            this.panelSelectedColor.Location =
                new Point(20, 60);

            this.panelSelectedColor.Size =
                new Size(240, 30);

            this.panelSelectedColor.BackColor =
                Color.Black;

            this.lblInspectorInfo.Location =
                new Point(20, 100);

            this.lblInspectorInfo.Size =
                new Size(240, 500);

            this.lblInspectorInfo.ForeColor =
                Color.White;

            this.lblInspectorInfo.Font =
                new Font("Segoe UI", 10);

            this.lblInspectorInfo.Text =
                "No color selected";

            // =====================================================
            // PANEL CUBE
            // =====================================================

            this.panelCube.Size =
                new Size(180, 120);

            this.panelCube.BackColor =
                Color.FromArgb(35, 35, 35);

            this.panelCube.BorderStyle =
                BorderStyle.FixedSingle;

            this.panelCube.Margin =
                new Padding(0, 10, 0, 10);

            // =====================================================
            // ADD CONTROLS
            // =====================================================

            this.panelInspector.Controls.Add(this.panelSelectedColor);
            this.panelInspector.Controls.Add(this.lblInspectorInfo);
            this.panelInspector.Controls.Add(this.btnOpen3DLab);
            btnOpen3DLab.BringToFront();

            this.panelColorCard.Controls.Add(this.lblColorTitle);
            this.panelColorCard.Controls.Add(this.cmbColorSpaces);
            this.panelColorCard.Controls.Add(this.flowColorControls);

            this.flowSidebar.Controls.Add(this.lblLogo);
            this.flowSidebar.Controls.Add(this.lblSubtitle);

            this.flowSidebar.Controls.Add(this.lblImageSection);

            this.flowSidebar.Controls.Add(this.btnopen);
            this.flowSidebar.Controls.Add(this.btnsave);
            this.panelResetRow.Controls.Add(this.NumberOfColors);
            this.panelResetRow.Controls.Add(this.btnReset);
            this.flowSidebar.Controls.Add(this.panelResetRow);

            this.flowSidebar.Controls.Add(this.lblFiltersSection);

            this.flowSidebar.Controls.Add(this.btn);
            this.flowSidebar.Controls.Add(this.btnGray);
            this.flowSidebar.Controls.Add(this.btnQuantize);

            this.flowSidebar.Controls.Add(this.lblInfo);

            this.flowSidebar.Controls.Add(this.panelColorCard);
            //this.flowSidebar.Controls.Add(this.panelCube);
            this.flowSidebar.Controls.Add(this.lblExperimentalSection);

            //this.flowSidebar.Controls.Add(this.btnOpen3DLab);

            this.panelSidebar.Controls.Add(this.flowSidebar);

            this.panelImageContainer.Controls.Add(this.pictureBox1);

            this.panelMain.Controls.Add(this.panelImageContainer);
            this.panelMain.Controls.Add(this.panelInspector);

            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelSidebar);

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();

            this.ResumeLayout(false);
        }

        #endregion
    }
}