namespace pixellab
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnopen = new System.Windows.Forms.Button();
            this.btnsave = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.panelSelectedColor = new System.Windows.Forms.Panel();
            this.btnGray = new System.Windows.Forms.Button();
            this.btnQuantize = new System.Windows.Forms.Button();
            this.btn = new System.Windows.Forms.Button();
            this.btnOpen3DLab = new System.Windows.Forms.Button();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.flowSidebar = new System.Windows.Forms.FlowLayoutPanel();
            this.cmbColorSpaces = new System.Windows.Forms.ComboBox();
            this.flowColorControls = new System.Windows.Forms.FlowLayoutPanel();
            this.panelColorCard =  new System.Windows.Forms.Panel();
            this.lblColorTitle =  new System.Windows.Forms.Label();
            this.panelInspector = new System.Windows.Forms.Panel();
            this.panelSelectedColor = new System.Windows.Forms.Panel();
            this.lblInspectorInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // تهيئة أداة البانل الجديدة للمكعب هنا
            this.panelCube = new System.Windows.Forms.Panel();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Dock = DockStyle.Fill;
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // btnopen
            // 
            this.btnopen.Name = "btnopen";
            this.btnopen.Size = new System.Drawing.Size(120, 40);
            this.btnopen.TabIndex = 1;
            this.btnopen.Text = "open Image";
            this.btnopen.Click += new System.EventHandler(this.btnopen_Click);
            // 
            // btnsave
            // 
            this.btnsave.Name = "btnsave";
            this.btnsave.Size = new System.Drawing.Size(120, 40);
            this.btnsave.TabIndex = 2;
            this.btnsave.Text = "save Image";
            this.btnsave.Click += new System.EventHandler(this.btnsave_Click);
            // 
            // btnReset
            // 
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 40);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // 
            // lblInfo
            // 
            this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(200, 120);
            this.lblInfo.TabIndex = 13;
            this.lblInfo.Text = "label4";
            //
            // panelSelectedColor
            //
            this.panelSelectedColor.Size = new System.Drawing.Size(220, 40);

            this.panelSelectedColor.BackColor = Color.Black;

            this.panelSelectedColor.BorderStyle = BorderStyle.FixedSingle;

            this.panelSelectedColor.Margin = new Padding(5);
            // 
            // btnGray
            // 
            this.btnGray.Name = "btnGray";
            this.btnGray.Size = new System.Drawing.Size(200, 40);
            this.btnGray.TabIndex = 14;
            this.btnGray.Text = "Black&White";
            this.btnGray.Click += new System.EventHandler(this.btnConvertBlackAndWhite_Click);
            // 
            // btnQuantize
            // 
            this.btnQuantize.Name = "btnQuantize";
            this.btnQuantize.Size = new System.Drawing.Size(200, 40);
            this.btnQuantize.TabIndex = 15;
            this.btnQuantize.Text = "ReduceColors";
            this.btnQuantize.Click += new System.EventHandler(this.btnQuantizeColors_Click);
            // 
            // btn
            // 
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(200, 40);
            this.btn.TabIndex = 16;
            this.btn.Text = "Gray scale";
            this.btn.Click += new System.EventHandler(this.btnConvertGrayscale_Click);
            // 
            // panelCube
            // 
            this.btnOpen3DLab.Name = "btnOpen3DLab";
            this.btnOpen3DLab.Size = new System.Drawing.Size(200, 40);
            this.btnOpen3DLab.TabIndex = 18;
            this.btnOpen3DLab.Text = "🚀 3D Lab";
            this.btnOpen3DLab.Click += new System.EventHandler(this.btnOpen3DLab_Click);



            this.panelCube.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelCube.Name = "panelCube";
            this.panelCube.Size = new System.Drawing.Size(180, 120);
            this.panelCube.TabIndex = 17;


            //
            // panelSidebar
            //
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Width = 250;
            this.panelSidebar.BackColor = Color.FromArgb(28, 28, 28);

            //
            // panelTop
            //
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 60;
            this.panelTop.BackColor = Color.FromArgb(35, 35, 35);

            //
            // panelMain
            //
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.BackColor = Color.FromArgb(20, 20, 20);
            //
            // panelInspector
            //
            this.panelInspector.BackColor =  Color.FromArgb(28, 28, 28);

            this.panelInspector.Size = new Size(220, 450);

            this.panelInspector.Location = new Point(20, 20);
            this.panelInspector.BorderStyle =   BorderStyle.None;
            this.panelInspector.Padding =new Padding(15);
            this.panelInspector.Controls.Add(this.panelSelectedColor);
            this.panelInspector.Controls.Add(this.lblInspectorInfo);

            //
            // lblInspectorInfo
            //
            this.lblInspectorInfo.ForeColor = Color.White;

            this.lblInspectorInfo.Font =
                new Font("Segoe UI", 10);

            this.lblInspectorInfo.Location =
                new Point(20, 90);

            this.lblInspectorInfo.Size =
                new Size(300, 500);

            this.lblInspectorInfo.Text =
                "No color selected";

            this.lblInspectorInfo.AutoSize = false;

            this.lblInspectorInfo.TextAlign =
               ContentAlignment.TopLeft;

            //
            // flowSidebar
            //
            this.flowSidebar.Dock = DockStyle.Fill;

            this.flowSidebar.FlowDirection = FlowDirection.TopDown;

            this.flowSidebar.WrapContents = false;

            this.flowSidebar.AutoScroll = true;

            this.flowSidebar.Padding = new Padding(15);

            this.flowSidebar.SetFlowBreak(this.panelCube, true);

            this.flowSidebar.BackColor = Color.FromArgb(28, 28, 28);

                        //
            // cmbColorSpaces
            //
            this.cmbColorSpaces.Width = 200;

            this.cmbColorSpaces.DropDownStyle =
                ComboBoxStyle.DropDownList;

            this.cmbColorSpaces.BackColor =
                Color.FromArgb(45,45,48);

            this.cmbColorSpaces.ForeColor = Color.White;

            this.cmbColorSpaces.FlatStyle = FlatStyle.Flat;

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

            this.cmbColorSpaces.SelectedIndexChanged +=new EventHandler(this.cmbColorSpaces_SelectedIndexChanged);

                        //
            // flowColorControls
            //
            this.flowColorControls.FlowDirection =FlowDirection.TopDown;

            this.flowColorControls.WrapContents = false;

            this.flowColorControls.AutoSize = false;

            this.flowColorControls.Width = 220;

            this.flowColorControls.Height = 300;

            this.flowColorControls.BackColor =Color.FromArgb(28,28,28);

                        //
            // panelColorCard
            //

            this.panelColorCard.Width = 220;

            this.panelColorCard.Height = 420;

            this.panelColorCard.BackColor =
                Color.FromArgb(35,35,35);

            this.panelColorCard.Padding =
                new Padding(10);

            this.panelColorCard.Margin =
                new Padding(5);
            this.panelColorCard.AutoScroll = true;


            this.lblColorTitle.Text =
                "Color Space";

            this.lblColorTitle.ForeColor =
                Color.White;

            this.lblColorTitle.Font =
                new Font("Segoe UI", 11, FontStyle.Bold);

            this.lblColorTitle.AutoSize = true;

            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.flowSidebar.Controls.Add(this.btnOpen3DLab);

            this.flowSidebar.Controls.Add(this.btnopen);

            this.flowSidebar.Controls.Add(this.btnsave);

            this.flowSidebar.Controls.Add(this.btnReset);


            this.flowSidebar.Controls.Add(this.btn);
            this.flowSidebar.Controls.Add(this.btnGray);
            this.flowSidebar.Controls.Add(this.btnQuantize);

            this.flowSidebar.Controls.Add(this.lblInfo);

            this.panelSidebar.Controls.Add(this.flowSidebar);

            this.panelColorCard.Controls.Add( this.lblColorTitle);

            this.panelColorCard.Controls.Add( this.cmbColorSpaces);
            this.cmbColorSpaces.Location = new Point(10, 45);

            this.panelColorCard.Controls.Add( this.flowColorControls);
            this.flowColorControls.Location =new Point(10, 90);

            this.flowSidebar.Controls.Add(this.panelColorCard);

            this.panelMain.Controls.Add(this.pictureBox1);

            this.pictureBox1.Controls.Add(this.panelInspector);

            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelSidebar);
            this.Name = "Form1";
            this.Text = "pixellab";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnopen;
        private System.Windows.Forms.Button btnsave;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Panel panelSelectedColor;
        private System.Windows.Forms.Button btnGray;
        private System.Windows.Forms.Button btnQuantize;
        private System.Windows.Forms.Button btn;
        private System.Windows.Forms.Panel panelCube;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.FlowLayoutPanel flowSidebar;
        private System.Windows.Forms.Panel panelInspector;
        private System.Windows.Forms.Label lblInspectorInfo;

        private System.Windows.Forms.Button btnOpen3DLab;

        private System.Windows.Forms.ComboBox cmbColorSpaces;

        private System.Windows.Forms.FlowLayoutPanel flowColorControls;

        private System.Windows.Forms.Panel panelColorCard;

        private System.Windows.Forms.Label lblColorTitle;

    }
}