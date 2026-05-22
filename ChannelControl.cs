using System;
using System.Drawing;
using System.Windows.Forms;

namespace pixellab
{
    public class ChannelControl : Panel
    {
        public Label lblName;

        public Label lblValue;

        public TrackBar track;

        public CheckBox chkEnable;

        public ChannelControl(
            string channelName,
            int min,
            int max)
        {
            this.Width = 210;

            this.Height = 90;

            this.BackColor =
                Color.FromArgb(35, 35, 35);

            this.Margin =
                new Padding(5);

            // اسم القناة
            lblName = new Label();

            lblName.Text = channelName;

            lblName.ForeColor = Color.White;

            lblName.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            lblName.Location =
                new Point(5, 5);

            lblName.AutoSize = true;

            // القيمة
            lblValue = new Label();

            lblValue.Text = "0";

            lblValue.ForeColor =
                Color.LightGray;

            lblValue.Location =
                new Point(170, 8);

            lblValue.AutoSize = true;

            // التراك بار
            track = new TrackBar();

            track.Minimum = min;

            track.Maximum = max;

            track.Width = 180;

            track.Location =
                new Point(5, 30);

            track.TickStyle =
                TickStyle.None;

            track.BackColor =
                Color.FromArgb(35, 35, 35);

            // checkbox
            chkEnable = new CheckBox();

            chkEnable.Checked = true;

            chkEnable.ForeColor = Color.White;

            chkEnable.Location =
                new Point(185, 32);

            // تحديث القيمة
            track.Scroll += (s, e) =>
            {
                lblValue.Text =
                    track.Value.ToString();
            };

            // إضافة العناصر
            this.Controls.Add(lblName);

            this.Controls.Add(lblValue);

            this.Controls.Add(track);

            this.Controls.Add(chkEnable);
        }
    }
}