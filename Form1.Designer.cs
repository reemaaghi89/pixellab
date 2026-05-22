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
            this.trackRed = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.trackGreen = new System.Windows.Forms.TrackBar();
            this.trackBlue = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkRed = new System.Windows.Forms.CheckBox();
            this.chkGreen = new System.Windows.Forms.CheckBox();
            this.chkBlue = new System.Windows.Forms.CheckBox();
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
            this.panelInspector = new System.Windows.Forms.Panel();
            this.panelSelectedColor = new System.Windows.Forms.Panel();
            this.lblInspectorInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // تهيئة أداة البانل الجديدة للمكعب هنا
            this.panelCube = new System.Windows.Forms.Panel();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBlue)).BeginInit();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Dock = DockStyle.Fill;
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
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
            // trackRed
            // 
            this.trackRed.Width = 200;
            this.trackRed.Maximum = 255;
            this.trackRed.Minimum = -255;
            this.trackRed.Name = "trackRed";
            this.trackRed.Size = new System.Drawing.Size(200, 45);
            this.trackRed.TabIndex = 4;
            this.trackRed.Scroll += new System.EventHandler(this.trackRed_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Red Channel";
            // 
            // trackGreen
            // 
            this.trackGreen.Width = 200;
            this.trackGreen.Maximum = 255;
            this.trackGreen.Minimum = -255;
            this.trackGreen.Name = "trackGreen";
            this.trackGreen.Size = new System.Drawing.Size(200, 45);
            this.trackGreen.TabIndex = 6;
            this.trackGreen.Scroll += new System.EventHandler(this.trackGreen_Scroll);
            // 
            // trackBlue
            // 
            this.trackBlue.Width = 200;
            this.trackBlue.Maximum = 255;
            this.trackBlue.Minimum = -255;
            this.trackBlue.Name = "trackBlue";
            this.trackBlue.Size = new System.Drawing.Size(200, 45);
            this.trackBlue.TabIndex = 7;
            this.trackBlue.Scroll += new System.EventHandler(this.trackBlue_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Green Channel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Blue Channel";
            // 
            // chkRed
            // 
            this.chkRed.AutoSize = true;
            this.chkRed.Name = "chkRed";
            this.chkRed.Size = new System.Drawing.Size(84, 17);
            this.chkRed.TabIndex = 10;
            this.chkRed.Text = "Disable Red";
            this.chkRed.CheckedChanged += new System.EventHandler(this.chkRed_CheckedChanged);
            // 
            // chkGreen
            // 
            this.chkGreen.AutoSize = true;
            this.chkGreen.Name = "chkGreen";
            this.chkGreen.Size = new System.Drawing.Size(93, 17);
            this.chkGreen.TabIndex = 11;
            this.chkGreen.Text = "Disable Green";
            this.chkGreen.CheckedChanged += new System.EventHandler(this.chkGreen_CheckedChanged);
            // 
            // chkBlue
            // 
            this.chkBlue.AutoSize = true;
            this.chkBlue.Name = "chkBlue";
            this.chkBlue.Size = new System.Drawing.Size(85, 17);
            this.chkBlue.TabIndex = 12;
            this.chkBlue.Text = "Disable Blue";
            this.chkBlue.CheckedChanged += new System.EventHandler(this.chkBlue_CheckedChanged);
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
            this.panelSelectedColor.Size = new System.Drawing.Size(200, 50);

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
            this.panelInspector.BackColor = Color.FromArgb(35, 70, 35);

            this.panelInspector.Size = new Size(280, 470);

            this.panelInspector.Location = new Point(20, 20);

            this.panelInspector.BorderStyle = BorderStyle.None;
            this.panelInspector.Padding = new Padding(10);
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

            this.flowSidebar.Controls.Add(this.label1);
            this.flowSidebar.Controls.Add(this.trackRed);

            this.flowSidebar.Controls.Add(this.label2);
            this.flowSidebar.Controls.Add(this.trackGreen);

            this.flowSidebar.Controls.Add(this.label3);
            this.flowSidebar.Controls.Add(this.trackBlue);

            this.flowSidebar.Controls.Add(this.chkRed);
            this.flowSidebar.Controls.Add(this.chkGreen);
            this.flowSidebar.Controls.Add(this.chkBlue);

            this.flowSidebar.Controls.Add(this.btn);
            this.flowSidebar.Controls.Add(this.btnGray);
            this.flowSidebar.Controls.Add(this.btnQuantize);

            this.flowSidebar.Controls.Add(this.lblInfo);

            this.panelSidebar.Controls.Add(this.flowSidebar);

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
            ((System.ComponentModel.ISupportInitialize)(this.trackRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBlue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnopen;
        private System.Windows.Forms.Button btnsave;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TrackBar trackRed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackGreen;
        private System.Windows.Forms.TrackBar trackBlue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkRed;
        private System.Windows.Forms.CheckBox chkGreen;
        private System.Windows.Forms.CheckBox chkBlue;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Panel panelSelectedColor;
        private System.Windows.Forms.Button btnGray;
        private System.Windows.Forms.Button btnQuantize;
        private System.Windows.Forms.Button btn;

        // التصريح عن الكائن في نهاية الكلاس كما تنص معايير البيئة
        private System.Windows.Forms.Panel panelCube;

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelTop;

        private System.Windows.Forms.FlowLayoutPanel flowSidebar;

        private System.Windows.Forms.Panel panelInspector;
        private System.Windows.Forms.Label lblInspectorInfo;

        private System.Windows.Forms.Button btnOpen3DLab;

    }
}