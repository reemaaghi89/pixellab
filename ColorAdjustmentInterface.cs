using System.Windows.Forms;

namespace pixellab
{
    public class ChannelControl : Panel
    {
        public Label lblName;

        public Label lblValue;

        public TrackBar track;

        public CheckBox chkEnable;
        public bool IsChannelEnabled => chkEnable.Checked;
        public ChannelControl(
            string channelName,int min,int max)
        {
            this.Width = 210;
            this.Height = 90;
            this.BackColor =Color.FromArgb(35, 35, 35);
            this.Margin =new Padding(5);
            lblName = new Label();
            lblName.Text = channelName;
            lblName.ForeColor = Color.White;
            lblName.Font =new Font("Segoe UI", 10, FontStyle.Bold);
            lblName.Location =new Point(5, 5);
            lblName.AutoSize = true;
            lblValue = new Label();
            lblValue.Text = "0";
            lblValue.ForeColor =Color.LightGray;
            lblValue.Location =new Point(170, 8);
            lblValue.AutoSize = true;
            track = new TrackBar();
            track.Minimum = min;
            track.Maximum = max;
            track.Width = 180;
            track.Location =new Point(5, 30);
            track.TickStyle =TickStyle.None;
            track.BackColor =Color.FromArgb(35, 35, 35);
            chkEnable = new CheckBox();
            chkEnable.Checked = true;
            chkEnable.ForeColor = Color.White;
            chkEnable.Location =new Point(185, 32);
            // track.Scroll += (s, e) =>
            // {
            //     lblValue.Text =track.Value.ToString();
            // };
            chkEnable.CheckedChanged += (s, e) =>
            {
                track.Enabled = chkEnable.Checked;
                // اختياري: تغيير لون النص ليوحي بالتعطيل
                lblValue.ForeColor = chkEnable.Checked ? Color.LightGray : Color.Gray;
                lblName.ForeColor = chkEnable.Checked ? Color.White : Color.Gray;
            };
            this.Controls.Add(lblName);
            this.Controls.Add(lblValue);
            this.Controls.Add(track);
            this.Controls.Add(chkEnable);
        }
        public void UpdateValueLabel()
        {   lblValue.Text = track.Value.ToString();   }
    }
   public static class ColorAdjustmentInterface
    {
        // دالة بناء الواجهة
        public static void InitializeSliders(FlowLayoutPanel panel, string systemName, EventHandler onValueChanged)
        {
            panel.Controls.Clear();
            foreach (var channel in ColorSpaceRegistry.GetChannels(systemName))
            {
                var slider = new ChannelControl(channel.Name, channel.Min, channel.Max);
                slider.track.Scroll += onValueChanged;
                slider.chkEnable.CheckedChanged += onValueChanged;
                panel.Controls.Add(slider);
            }
        }

        // دالة استخراج القيم
        public static int GetSliderValue(FlowLayoutPanel panel, string sliderName)
        {
            foreach (Control control in panel.Controls)
            {
                if (control is ChannelControl slider && slider.lblName.Text == sliderName)
                {
                    if (!slider.IsChannelEnabled) return 0;
                    return slider.track.Value;
                }
            }
            return 0;
        }
        public static void SetSliderValue(FlowLayoutPanel panel, string sliderName, int value)
        {
            foreach (Control control in panel.Controls)
            {
                if (control is ChannelControl slider && slider.lblName.Text == sliderName)
                {
                    var handler = slider.track.Tag as EventHandler; // نفترض أننا خزنّا الحدث هنا
                    
                    if (value >= slider.track.Minimum && value <= slider.track.Maximum)
                    {
                        slider.track.Value = value;
                        slider.UpdateValueLabel();
                    }
                    break;
                }
            }
        }

    }
}