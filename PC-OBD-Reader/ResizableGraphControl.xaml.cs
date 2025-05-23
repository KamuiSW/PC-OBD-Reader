using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace PC_OBD_Reader
{
    public partial class ResizableGraphControl : UserControl
    {
        public event EventHandler DragCompleted = delegate { };

        public ResizableGraphControl()
        {
            InitializeComponent();
            Loaded += ResizableGraphControl_Loaded;
        }

        private void ResizableGraphControl_Loaded(object sender, RoutedEventArgs e)
        {
            GraphTypeComboBox.SelectedIndex = 0;
            UpdateChart();
        }

        private void GraphTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            string selected = (GraphTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "RPM";
            Chart.Series.Clear();
            Chart.AxisX.Clear();
            Chart.AxisY.Clear();

            switch (selected)
            {
                case "RPM":
                    Chart.Series.Add(new LineSeries
                    {
                        Title = "RPM",
                        Values = new ChartValues<double> { 800, 1200, 2000, 2500, 3000, 3500, 4000 },
                        Stroke = Brushes.LimeGreen,
                        Fill = Brushes.Transparent
                    });
                    Chart.AxisY.Add(new Axis { Title = "RPM", MinValue = 0, MaxValue = 5000 });
                    break;
                case "Speed":
                    Chart.Series.Add(new LineSeries
                    {
                        Title = "Speed",
                        Values = new ChartValues<double> { 0, 10, 30, 50, 70, 60, 80 },
                        Stroke = Brushes.DodgerBlue,
                        Fill = Brushes.Transparent
                    });
                    Chart.AxisY.Add(new Axis { Title = "km/h", MinValue = 0, MaxValue = 120 });
                    break;
                case "Coolant Temp":
                    Chart.Series.Add(new LineSeries
                    {
                        Title = "Coolant Temp",
                        Values = new ChartValues<double> { 60, 70, 80, 90, 95, 100, 105 },
                        Stroke = Brushes.OrangeRed,
                        Fill = Brushes.Transparent
                    });
                    Chart.AxisY.Add(new Axis { Title = "°C", MinValue = 40, MaxValue = 120 });
                    break;
            }
            Chart.AxisX.Add(new Axis { Title = "Time" });
        }

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

        // Add a method to update the chart with a new data point (for live OBD2 data)
        public void UpdateDataPoint(double value)
        {
            // Determine which graph type is selected
            string selected = (this.GraphTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "RPM";
            if (this.Chart.Series.Count == 0)
            {
                UpdateChart(); // Ensure chart is initialized
            }
            if (this.Chart.Series[0] is LineSeries lineSeries)
            {
                // Limit to last 50 points for performance
                if (lineSeries.Values.Count >= 50)
                    lineSeries.Values.RemoveAt(0);
                lineSeries.Values.Add(value);
            }
        }
    }
}