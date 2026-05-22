using System.Drawing;
using System.Windows.Forms;

namespace pixellab
{
    public class DraggablePanelHandler
    {
        private bool _isDragging = false;
        private Point _dragCursorPoint;
        private Point _dragPanelPoint;
        private readonly Control _targetControl;

        public DraggablePanelHandler(Control targetControl)
        {
            _targetControl = targetControl;
            _targetControl.MouseDown += OnMouseDown;
            _targetControl.MouseMove += OnMouseMove;
            _targetControl.MouseUp += OnMouseUp;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _isDragging = true;
            _dragCursorPoint = Cursor.Position;
            _dragPanelPoint = _targetControl.Location;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point difference = Point.Subtract(Cursor.Position, new Size(_dragCursorPoint));
                _targetControl.Location = Point.Add(_dragPanelPoint, new Size(difference));
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }
    }
}