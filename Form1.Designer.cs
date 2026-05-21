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
            this.btnGray = new System.Windows.Forms.Button();
            this.btnQuantize = new System.Windows.Forms.Button();
            this.btn = new System.Windows.Forms.Button();
            this.btnOpen3DLab = new System.Windows.Forms.Button();
            // تهيئة أداة البانل الجديدة للمكعب هنا
            this.panelCube = new System.Windows.Forms.Panel();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBlue)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(20, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(700, 500);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // btnopen
            // 
            this.btnopen.Location = new System.Drawing.Point(760, 40);
            this.btnopen.Name = "btnopen";
            this.btnopen.Size = new System.Drawing.Size(120, 40);
            this.btnopen.TabIndex = 1;
            this.btnopen.Text = "open Image";
            this.btnopen.UseVisualStyleBackColor = true;
            this.btnopen.Click += new System.EventHandler(this.btnopen_Click);
            // 
            // btnsave
            // 
            this.btnsave.Location = new System.Drawing.Point(760, 100);
            this.btnsave.Name = "btnsave";
            this.btnsave.Size = new System.Drawing.Size(120, 40);
            this.btnsave.TabIndex = 2;
            this.btnsave.Text = "save Image";
            this.btnsave.UseVisualStyleBackColor = true;
            this.btnsave.Click += new System.EventHandler(this.btnsave_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(760, 160);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 40);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // trackRed
            // 
            this.trackRed.Location = new System.Drawing.Point(760, 280);
            this.trackRed.Maximum = 255;
            this.trackRed.Minimum = -255;
            this.trackRed.Name = "trackRed";
            this.trackRed.Size = new System.Drawing.Size(300, 45);
            this.trackRed.TabIndex = 4;
            this.trackRed.Scroll += new System.EventHandler(this.trackRed_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(749, 224);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Red Channel";
            // 
            // trackGreen
            // 
            this.trackGreen.Location = new System.Drawing.Point(760, 380);
            this.trackGreen.Maximum = 255;
            this.trackGreen.Minimum = -255;
            this.trackGreen.Name = "trackGreen";
            this.trackGreen.Size = new System.Drawing.Size(300, 45);
            this.trackGreen.TabIndex = 6;
            this.trackGreen.Scroll += new System.EventHandler(this.trackGreen_Scroll);
            // 
            // trackBlue
            // 
            this.trackBlue.Location = new System.Drawing.Point(760, 480);
            this.trackBlue.Maximum = 255;
            this.trackBlue.Minimum = -255;
            this.trackBlue.Name = "trackBlue";
            this.trackBlue.Size = new System.Drawing.Size(300, 45);
            this.trackBlue.TabIndex = 7;
            this.trackBlue.Scroll += new System.EventHandler(this.trackBlue_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(752, 332);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Green Channel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(752, 432);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Blue Channel";
            // 
            // chkRed
            // 
            this.chkRed.AutoSize = true;
            this.chkRed.Location = new System.Drawing.Point(760, 532);
            this.chkRed.Name = "chkRed";
            this.chkRed.Size = new System.Drawing.Size(84, 17);
            this.chkRed.TabIndex = 10;
            this.chkRed.Text = "Disable Red";
            this.chkRed.UseVisualStyleBackColor = true;
            this.chkRed.CheckedChanged += new System.EventHandler(this.chkRed_CheckedChanged);
            // 
            // chkGreen
            // 
            this.chkGreen.AutoSize = true;
            this.chkGreen.Location = new System.Drawing.Point(760, 555);
            this.chkGreen.Name = "chkGreen";
            this.chkGreen.Size = new System.Drawing.Size(93, 17);
            this.chkGreen.TabIndex = 11;
            this.chkGreen.Text = "Disable Green";
            this.chkGreen.UseVisualStyleBackColor = true;
            this.chkGreen.CheckedChanged += new System.EventHandler(this.chkGreen_CheckedChanged);
            // 
            // chkBlue
            // 
            this.chkBlue.AutoSize = true;
            this.chkBlue.Location = new System.Drawing.Point(760, 578);
            this.chkBlue.Name = "chkBlue";
            this.chkBlue.Size = new System.Drawing.Size(85, 17);
            this.chkBlue.TabIndex = 12;
            this.chkBlue.Text = "Disable Blue";
            this.chkBlue.UseVisualStyleBackColor = true;
            this.chkBlue.CheckedChanged += new System.EventHandler(this.chkBlue_CheckedChanged);
            // 
            // lblInfo
            // 
            this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblInfo.Location = new System.Drawing.Point(420, 532);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(300, 120);
            this.lblInfo.TabIndex = 13;
            this.lblInfo.Text = "label4";
            // 
            // btnGray
            // 
            this.btnGray.Location = new System.Drawing.Point(317, 555);
            this.btnGray.Name = "btnGray";
            this.btnGray.Size = new System.Drawing.Size(75, 23);
            this.btnGray.TabIndex = 14;
            this.btnGray.Text = "Black&White";
            this.btnGray.UseVisualStyleBackColor = true;
            this.btnGray.Click += new System.EventHandler(this.btnConvertBlackAndWhite_Click);
            // 
            // btnQuantize
            // 
            this.btnQuantize.Location = new System.Drawing.Point(317, 603);
            this.btnQuantize.Name = "btnQuantize";
            this.btnQuantize.Size = new System.Drawing.Size(80, 30);
            this.btnQuantize.TabIndex = 15;
            this.btnQuantize.Text = "ReduceColors";
            this.btnQuantize.UseVisualStyleBackColor = true;
            this.btnQuantize.Click += new System.EventHandler(this.btnQuantizeColors_Click);
            // 
            // btn
            // 
            this.btn.Location = new System.Drawing.Point(214, 532);
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(75, 23);
            this.btn.TabIndex = 16;
            this.btn.Text = "Gray scale";
            this.btn.UseVisualStyleBackColor = true;
            this.btn.Click += new System.EventHandler(this.btnConvertGrayscale_Click);
            // 
            // panelCube
            // 
            this.btnOpen3DLab.Location = new System.Drawing.Point(760, 12); // قعد فوق الأزرار تماماً وبشكل أنيق
            this.btnOpen3DLab.Name = "btnOpen3DLab";
            this.btnOpen3DLab.Size = new System.Drawing.Size(120, 25);
            this.btnOpen3DLab.TabIndex = 18;
            this.btnOpen3DLab.Text = "🚀 3D Lab";
            this.btnOpen3DLab.UseVisualStyleBackColor = true;
            this.btnOpen3DLab.Click += new System.EventHandler(this.btnOpen3DLab_Click);



            this.panelCube.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelCube.Location = new System.Drawing.Point(20, 532);
            this.panelCube.Name = "panelCube";
            this.panelCube.Size = new System.Drawing.Size(180, 120);
            this.panelCube.TabIndex = 17;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.btnOpen3DLab);
            this.Controls.Add(this.panelCube); // ربط البانل بالـ Controls الخاصة بالنافذة بشكل صحيح
            this.Controls.Add(this.btn);
            this.Controls.Add(this.btnQuantize);
            this.Controls.Add(this.btnGray);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.chkBlue);
            this.Controls.Add(this.chkGreen);
            this.Controls.Add(this.chkRed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackBlue);
            this.Controls.Add(this.trackGreen);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackRed);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnsave);
            this.Controls.Add(this.btnopen);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "pixellab";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBlue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button btnGray;
        private System.Windows.Forms.Button btnQuantize;
        private System.Windows.Forms.Button btn;
        
        // التصريح عن الكائن في نهاية الكلاس كما تنص معايير البيئة
        private System.Windows.Forms.Panel panelCube;
        private System.Windows.Forms.Button btnOpen3DLab;
    }
}