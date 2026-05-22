using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pixellab
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}



/*
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


public partial class InspectorForm : Form
{
    private Panel panelPreview;

    private Label lblInspectorInfo;
    public InspectorForm()
    {
        InitializeComponent();

        InitializeInspectorUI();

        ApplyDarkTheme();
    }

    private void InitializeInspectorUI()
{
    // مربع اللون
    panelPreview = new Panel();

    panelPreview.Size = new Size(200, 50);

    panelPreview.Location = new Point(20, 20);

    panelPreview.BackColor = Color.Black;

    // معلومات اللون
    lblInspectorInfo = new Label();

    lblInspectorInfo.Location = new Point(20, 90);

    lblInspectorInfo.Size = new Size(220, 150);

    lblInspectorInfo.ForeColor = Color.White;

    lblInspectorInfo.Font = new Font("Segoe UI", 10);

    lblInspectorInfo.Text = "No color selected";

    // إضافة العناصر للفورم
    this.Controls.Add(panelPreview);

    this.Controls.Add(lblInspectorInfo);
}

private void ApplyDarkTheme()
{
    this.BackColor = Color.FromArgb(30, 30, 30);

    this.ForeColor = Color.White;

    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

    this.StartPosition = FormStartPosition.Manual;

    this.Size = new Size(260, 280);

    this.TopMost = true;
}
public void UpdateInspector(Color color, string info)
{
    panelPreview.BackColor = color;

    lblInspectorInfo.Text = info;
}
}

*/