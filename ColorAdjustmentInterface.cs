using System.Windows.Forms;

namespace pixellab
{
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
                    return slider.track.Value;
                }
            }
            return 0;
        }
    }
}