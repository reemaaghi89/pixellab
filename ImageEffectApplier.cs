using System.Drawing;
using System.Windows.Forms;

namespace pixellab
{
    public static class ImageEffectApplier
    {
        public static void Apply(string colorSpace, Bitmap original, FlowLayoutPanel panel, PictureBox pb)
        {
            if (original == null) return;

            Bitmap result = null;

            switch (colorSpace)
            {
                case "RGB":
                    result = ImageProcessor.ApplyRGBFast(original, 
                        ColorAdjustmentInterface.GetSliderValue(panel, "Red"), 
                        ColorAdjustmentInterface.GetSliderValue(panel, "Green"), 
                        ColorAdjustmentInterface.GetSliderValue(panel, "Blue"));
                    break;

                case "HSV":
                    result = ImageProcessor.ApplyHSVAdjustment(original,
                        ColorAdjustmentInterface.GetSliderValue(panel, "Hue"),
                        ColorAdjustmentInterface.GetSliderValue(panel, "Saturation") / 100.0,
                        ColorAdjustmentInterface.GetSliderValue(panel, "Value") / 100.0);
                    break;

                case "CMYK":
                    result = ImageProcessor.ApplyCMYKAdjustment(original,
                        ColorAdjustmentInterface.GetSliderValue(panel, "Cyan") / 100.0,
                        ColorAdjustmentInterface.GetSliderValue(panel, "Magenta") / 100.0,
                        ColorAdjustmentInterface.GetSliderValue(panel, "Yellow") / 100.0,
                        ColorAdjustmentInterface.GetSliderValue(panel, "Black") / 100.0);
                    break;

                case "LAB":
                    result = ImageProcessor.ApplyLABAdjustment(original,
                        ColorAdjustmentInterface.GetSliderValue(panel, "L"),
                        ColorAdjustmentInterface.GetSliderValue(panel, "A"),
                        ColorAdjustmentInterface.GetSliderValue(panel, "B"));
                    break;

                case "YUV":
                    result = ImageProcessor.ApplyYUVAdjustment(original,
                        ColorAdjustmentInterface.GetSliderValue(panel, "Y"),
                        ColorAdjustmentInterface.GetSliderValue(panel, "U"),
                        ColorAdjustmentInterface.GetSliderValue(panel, "V"));
                    break;

                case "YCbCr":
                    result = ImageProcessor.ApplyYCbCrAdjustment(original,
                        ColorAdjustmentInterface.GetSliderValue(panel, "Y"),
                        ColorAdjustmentInterface.GetSliderValue(panel, "Cb"),
                        ColorAdjustmentInterface.GetSliderValue(panel, "Cr"));
                    break;
            }

            if (result != null)
            {
                pb.Image = result;
            }
        }
    }
}