// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Data;
// using System.Drawing;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using System.Windows.Forms;
// using pixellab.Converters;
// using System.IO;
// using System.Drawing.Drawing2D;
// using pixellab.Renderers;
// //==========================================================
// // من السلايدرات للمجسم
// namespace pixellab
// {
//     public partial class connectBetween3DandPic
//     {
//         private bool isUpdatingFrom3D = false;
//         private void ChannelTrackChanged(object sender, EventArgs e)
//         {
//             if (isUpdatingFrom3D) return;

//             Color calculatedColor = Color.White; 
//             if (currentColorSpace == "RGB")
//             {
//                 int r = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Red");
//                 int g = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Green");
//                 int b = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Blue");
//                 calculatedColor = Color.FromArgb(r, g, b);
//             }
//             else if (currentColorSpace == "HSV")
//             {
//                 double h = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Hue");
//                 double s = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Saturation") / 100.0;
//                 double v = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Value") / 100.0;
//                 calculatedColor = HsvConverter.ToRgb(h, s, v); // استخدام المحول الخاص بكِ
//             }
//             else if (currentColorSpace == "LAB")
//             {
//                 double l = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "L");
//                 double a = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "A");
//                 double b = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "B");
//                 calculatedColor = LabConverter.ToRgb(l, a, b); // استخدام المحول الخاص بكِ
//             }
//             else if (currentColorSpace == "CMYK")
//             {
//                 double c = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Cyan") / 100.0;
//                 double m = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Magenta") / 100.0;
//                 double y = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Yellow") / 100.0;
//                 double k = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Black") / 100.0;
//                 calculatedColor = CmykConverter.ToRgb(c, m, y, k);
//             }
//             else if (currentColorSpace == "YUV")
//             {
//                 // لاحظي هنا بنقسم الـ Y على 255 إذا كان المحول عندك بيتعامل مع مجال 0-1
//                 double y = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Y") / 255.0;
//                 double u = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "U");
//                 double v = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "V");
//                 calculatedColor = YuvConverter.ToRgb(y, u, v);
//             }
//             else if (currentColorSpace == "YCbCr")
//             {
//                 double y = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Y");
//                 double cb = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Cb");
//                 double cr = ColorAdjustmentInterface.GetSliderValue(flowColorControls, "Cr");
//                 calculatedColor = YcbcrConverter.ToRgb(y, cb, cr);
//             }
            
//             panelSelectedColor.BackColor = calculatedColor;

//             if (_spaceForm != null && !_spaceForm.IsDisposed)
//             {
//                 _spaceForm.UpdateFromForm1(calculatedColor);
//             }

//             livePreviewTimer.Stop();
//             livePreviewTimer.Start();
//         }
        

//         private void BuildColorControls(string systemName)
//         {
//             currentColorSpace = systemName;
//             ColorAdjustmentInterface.InitializeSliders(flowColorControls, systemName, ChannelTrackChanged);
//             UpdateSlidersFrom3D(panelSelectedColor.BackColor, systemName);
//         }
//     ////////////////////////////////////////////////////////////////////////////
//     /// /////////////////////////////////////////////////////////////////////////////
//     /// 
//     ///// من المجسم للسلايدرات    
//         public void UpdateSlidersFrom3D(Color pickedColor, string systemName)
//         {
//             livePreviewTimer.Stop();
//             isUpdatingFrom3D = true;
            
//             // تحديث لون المعاينة فوراً ليكون متناسق مع اللي اخترتيه من الـ 3D
//             panelSelectedColor.BackColor = pickedColor;

//             // "الرجعة": تحويل اللون المختار لقيم سلايدرات حسب النظام النشط
//             if (systemName == "RGB Cube" || systemName == "RGB")
//             {
//                 UpdateSingleSlider("Red", pickedColor.R);
//                 UpdateSingleSlider("Green", pickedColor.G);
//                 UpdateSingleSlider("Blue", pickedColor.B);
//             }
//             else if (systemName == "HSV Cone" || systemName == "HSV")
//             {
//                 var hsv = HsvConverter.FromRgb(pickedColor);
//                 UpdateSingleSlider("Hue", (int)hsv.Hue);
//                 UpdateSingleSlider("Saturation", (int)(hsv.Saturation * 100));
//                 UpdateSingleSlider("Value", (int)(hsv.Value * 100));
//             }
//             else if (systemName == "Lab Space" || systemName == "LAB")
//             {
//                 var lab = LabConverter.FromRgb(pickedColor);
//                 UpdateSingleSlider("L", (int)lab.L);
//                 UpdateSingleSlider("A", (int)lab.A);
//                 UpdateSingleSlider("B", (int)lab.B);
//             }
//             else if (systemName == "CMYK Space" || systemName == "CMYK")
//             {
//                 var cmyk = CmykConverter.FromRgb(pickedColor); // تأكدي من وجود FromRgb عندك
//                 UpdateSingleSlider("Cyan", (int)(cmyk.C * 100));
//                 UpdateSingleSlider("Magenta", (int)(cmyk.M * 100));
//                 UpdateSingleSlider("Yellow", (int)(cmyk.Y * 100));
//                 UpdateSingleSlider("Black", (int)(cmyk.K * 100));
//             }
//             else if (systemName == "YUV Space" || systemName == "YUV")
//             {
//                 var yuv = YuvConverter.FromRgb(pickedColor);
//                 UpdateSingleSlider("Y", (int)(yuv.Y * 255));
//                 UpdateSingleSlider("U", (int)yuv.U);
//                 UpdateSingleSlider("V", (int)yuv.V);
//             }
//             else if (systemName == "YCbCr Space" || systemName == "YCbCr")
//             {
//                 var ycc = YcbcrConverter.FromRgb(pickedColor);
//                 UpdateSingleSlider("Y", (int)ycc.Y);
//                 UpdateSingleSlider("Cb", (int)ycc.Cb);
//                 UpdateSingleSlider("Cr", (int)ycc.Cr);
//             }

//             Form1.livePreviewTimer.Start();
//             isUpdatingFrom3D = false;
//         }

//         private void UpdateSingleSlider(string name, int value)
//         {
//             foreach (Control ctrl in flowColorControls.Controls)
//             {
//                 if (ctrl is ChannelControl cc && cc.lblName.Text == name)
//                 {
//                     cc.track.Value = value; 
//                     cc.lblValue.Text = value.ToString(); // تحديث النص
//                 }
//             }
//         }
//     }
        
// }
        