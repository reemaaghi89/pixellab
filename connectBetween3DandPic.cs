using System;
using System.Drawing;
using System.Windows.Forms;
using pixellab.Converters;

namespace pixellab
{
    public class ColorSystemManager
    {   


        private FlowLayoutPanel _panel;
        public bool IsUpdatingFrom3D = false;
        public ColorSystemManager(FlowLayoutPanel panel)
        {
            _panel = panel;
        }
//////////////////////////////////////////////////////////////////////////
/// This method reads the current slider values and calculates the 
/// corresponding RGB color based on the active color space.
///  This allows the 3D color picker to reflect changes made in any
///  color space, ensuring synchronization across all systems.    
        public Color GetColorFromSliders(string currentColorSpace)
        {
            Color calculatedColor = Color.White;

            if (currentColorSpace == "RGB")
            {
                int r = ColorAdjustmentInterface.GetSliderValue(_panel, "Red");
                int g = ColorAdjustmentInterface.GetSliderValue(_panel, "Green");
                int b = ColorAdjustmentInterface.GetSliderValue(_panel, "Blue");
                calculatedColor = Color.FromArgb(r, g, b);
            }
            else if (currentColorSpace == "HSV")
            {
                double h = ColorAdjustmentInterface.GetSliderValue(_panel, "Hue");
                double s = ColorAdjustmentInterface.GetSliderValue(_panel, "Saturation") / 100.0;
                double v = ColorAdjustmentInterface.GetSliderValue(_panel, "Value") / 100.0;
                calculatedColor = HsvConverter.ToRgb(h, s, v);
            }
            else if (currentColorSpace == "LAB")
            {
                double l = ColorAdjustmentInterface.GetSliderValue(_panel, "L");
                double a = ColorAdjustmentInterface.GetSliderValue(_panel, "A");
                double b = ColorAdjustmentInterface.GetSliderValue(_panel, "B");
                calculatedColor = LabConverter.ToRgb(l, a, b);
            }
            else if (currentColorSpace == "CMYK")
            {
                double c = ColorAdjustmentInterface.GetSliderValue(_panel, "Cyan") / 100.0;
                double m = ColorAdjustmentInterface.GetSliderValue(_panel, "Magenta") / 100.0;
                double y = ColorAdjustmentInterface.GetSliderValue(_panel, "Yellow") / 100.0;
                double k = ColorAdjustmentInterface.GetSliderValue(_panel, "Black") / 100.0;
                calculatedColor = CmykConverter.ToRgb(c, m, y, k);
            }
            else if (currentColorSpace == "YUV")
            {
                double y = ColorAdjustmentInterface.GetSliderValue(_panel, "Y") / 255.0;
                double u = ColorAdjustmentInterface.GetSliderValue(_panel, "U");
                double v = ColorAdjustmentInterface.GetSliderValue(_panel, "V");
                calculatedColor = YuvConverter.ToRgb(y, u, v);
            }
            else if (currentColorSpace == "YCbCr")
            {
                double y = ColorAdjustmentInterface.GetSliderValue(_panel, "Y");
                double cb = ColorAdjustmentInterface.GetSliderValue(_panel, "Cb");
                double cr = ColorAdjustmentInterface.GetSliderValue(_panel, "Cr");
                calculatedColor = YcbcrConverter.ToRgb(y, cb, cr);
            }

            return calculatedColor;
        }


       public void UpdateSlidersFrom3D(Color pickedColor, string systemName)
        {
        
            switch (systemName)
            {
                case "RGB Cube":
                case "RGB":
                    UpdateSingleSlider("Red", pickedColor.R);
                    UpdateSingleSlider("Green", pickedColor.G);
                    UpdateSingleSlider("Blue", pickedColor.B);
                    break;

                case "HSV Cone":
                case "HSV":
                    var hsv = pixellab.Converters.HsvConverter.FromRgb(pickedColor);
                    UpdateSingleSlider("Hue", (int)Math.Round(hsv.Hue));
                    UpdateSingleSlider("Saturation", (int)Math.Round(hsv.Saturation * 100));
                    UpdateSingleSlider("Value", (int)Math.Round(hsv.Value * 100));
                    break;

                case "Lab Space":
                case "Lab":
                case "LAB":
                    var lab = pixellab.Converters.LabConverter.FromRgb(pickedColor);
                    UpdateSingleSlider("L", (int)Math.Round(lab.L));
                    UpdateSingleSlider("A", (int)Math.Round(lab.A));
                    UpdateSingleSlider("B", (int)Math.Round(lab.B));
                    break;

                case "CMYK Space":
                case "CMYK":
                    var cmyk = pixellab.Converters.CmykConverter.FromRgb(pickedColor);
                    UpdateSingleSlider("Cyan", (int)Math.Round(cmyk.C * 100));
                    UpdateSingleSlider("Magenta", (int)Math.Round(cmyk.M * 100));
                    UpdateSingleSlider("Yellow", (int)Math.Round(cmyk.Y * 100));
                    UpdateSingleSlider("Black", (int)Math.Round(cmyk.K * 100));
                    break;

                case "YUV Space":
                case "YUV":
                    var yuv = pixellab.Converters.YuvConverter.FromRgb(pickedColor);
                    UpdateSingleSlider("Y", (int)Math.Round(yuv.Y * 255));
                    UpdateSingleSlider("U", (int)Math.Round(yuv.U));
                    UpdateSingleSlider("V", (int)Math.Round(yuv.V));
                    break;

                case "YCbCr Space":
                case "YCbCr":
                    var ycc = pixellab.Converters.YcbcrConverter.FromRgb(pickedColor);
                    UpdateSingleSlider("Y", (int)Math.Round(ycc.Y));
                    UpdateSingleSlider("Cb", (int)Math.Round(ycc.Cb));
                    UpdateSingleSlider("Cr", (int)Math.Round(ycc.Cr));
                    break;
            }
        }

        // التابع المساعد الذي يلف على السلايدرات
        private void UpdateSingleSlider(string name, int value)
        {
            Console.WriteLine($"Trying to update {name} to {value}");
            foreach (Control ctrl in _panel.Controls)
            {
                if (ctrl is ChannelControl cc && cc.lblName.Text == name)
                {
                    int safeValue = Math.Max(cc.track.Minimum, Math.Min(cc.track.Maximum, value));
                    cc.track.Value = safeValue;
                    cc.lblValue.Text = safeValue.ToString();
                }
            }
        }
        

    }
}