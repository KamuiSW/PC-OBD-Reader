using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PC_OBD_Reader
{
    public partial class ResizableGraphControl : UserControl
    {
        public ResizableGraphControl()
        {
            InitializeComponent();
        }

        public event EventHandler DragCompleted;

        private Point dragStartPoint;
        private bool isDragging = false;

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.Width = Math.Max(this.Width + e.HorizontalChange, 50);
            this.Height = Math.Max(this.Height + e.VerticalChange, 50);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            dragStartPoint = e.GetPosition(null);
            isDragging = true;
            CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(Parent as Canvas);
                Canvas.SetLeft(this, pos.X - (ActualWidth / 2));
                Canvas.SetTop(this, pos.Y - (ActualHeight / 2));
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (isDragging)
            {
                isDragging = false;
                ReleaseMouseCapture();
                DragCompleted?.Invoke(this, EventArgs.Empty); // Tell main window we're done
            }
        }
    }
}