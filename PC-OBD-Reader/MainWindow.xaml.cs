using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.AttributeIds;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PC_OBD_Reader
{
    public partial class MainWindow : Window
    {
        // Workspaces hold lists of graphs
        private Dictionary<string, List<ResizableGraphControl>> workspaces = new Dictionary<string, List<ResizableGraphControl>>();
        private string currentWorkspace = string.Empty;

        // Bluetooth-related fields
        private BluetoothClient bluetoothClient;
        private BluetoothDeviceInfo[] devices;
        private DispatcherTimer timer;
        private const string obdDeviceName = "OBDII";

        public MainWindow()
        {
            InitializeComponent();

            AddNewWorkspace("Workspace 1");
            WorkspaceSelector.SelectedIndex = 0;
            // Example: Show some error codes initially (you can update this dynamically)
            DisplayErrorCodes(new List<string> { "P0300", "P0420", "P0171" });

            // Initialize Bluetooth client
            bluetoothClient = new BluetoothClient();
            // Get paired devices
            devices = bluetoothClient.DiscoverDevices(255).ToArray();
            // Initialize timer for live data polling
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void AddNewWorkspace(string name)
        {
            if (!workspaces.ContainsKey(name))
            {
                workspaces[name] = new List<ResizableGraphControl>();
                WorkspaceSelector.Items.Add(name);
            }
        }

        private void AddGraph_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentWorkspace)) return;

            var graph = new ResizableGraphControl();
            graph.DragCompleted += Graph_DragCompleted!;
            workspaces[currentWorkspace].Add(graph);

            RefreshWorkspace();
        }

        private void RefreshWorkspace()
        {
            GraphCanvas.Children.Clear();

            foreach (var graph in workspaces[currentWorkspace])
            {
                GraphCanvas.Children.Add(graph);
            }

            UpdateGraphLayout();
        }

        private void WorkspaceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb != null && cb.SelectedItem != null)
            {
                currentWorkspace = cb.SelectedItem.ToString() ?? string.Empty;
                if (WorkspaceSelector.SelectedItem?.ToString() != currentWorkspace)
                    WorkspaceSelector.SelectedItem = currentWorkspace;
                RefreshWorkspace();
            }
        }

        private void AddWorkspace_Click(object sender, RoutedEventArgs e)
        {
            int newIndex = workspaces.Count + 1;
            string newName = $"Workspace {newIndex}";
            AddNewWorkspace(newName);
            WorkspaceSelector.SelectedItem = newName;
        }

        private void Graph_DragCompleted(object? sender, EventArgs e)
        {
            if (sender is UIElement element)
                SnapToNearestSlot(element);
        }

        private void SnapToNearestSlot(UIElement dragged)
        {
            var count = GraphCanvas.Children.Count;
            var slots = new List<Point>();

            double canvasWidth = GraphCanvas.ActualWidth;
            double canvasHeight = GraphCanvas.ActualHeight;

            for (int i = 0; i < count; i++)
            {
                double x, y;

                if (count == 1)
                {
                    x = 0; y = 0;
                }
                else if (count == 2)
                {
                    x = (i % 2) * (canvasWidth / 2); y = 0;
                }
                else if (count == 3)
                {
                    x = i * (canvasWidth / 3); y = 0;
                }
                else if (count == 4)
                {
                    x = (i % 2) * (canvasWidth / 2);
                    y = (i / 2) * (canvasHeight / 2);
                }
                else
                {
                    x = (i % 2) * (canvasWidth / 2);
                    y = (i / 2) * (canvasHeight / 2);
                }

                slots.Add(new Point(x, y));
            }

            Point draggedPos = new Point(Canvas.GetLeft(dragged), Canvas.GetTop(dragged));
            int closestIndex = 0;
            double closestDistance = double.MaxValue;

            for (int i = 0; i < slots.Count; i++)
            {
                double dist = (slots[i] - draggedPos).Length;
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestIndex = i;
                }
            }

            GraphCanvas.Children.Remove(dragged);
            GraphCanvas.Children.Insert(closestIndex, dragged);

            UpdateGraphLayout();
        }

        private void UpdateGraphLayout()
        {
            int count = GraphCanvas.Children.Count;
            double canvasWidth = GraphCanvas.ActualWidth;
            double canvasHeight = GraphCanvas.ActualHeight;

            for (int i = 0; i < count; i++)
            {
                UIElement child = GraphCanvas.Children[i];
                double width, height, left, top;

                if (count == 1)
                {
                    width = canvasWidth;
                    height = canvasHeight;
                    left = 0;
                    top = 0;
                }
                else if (count == 2)
                {
                    width = canvasWidth / 2;
                    height = canvasHeight;
                    left = i * width;
                    top = 0;
                }
                else if (count == 3)
                {
                    width = canvasWidth / 3;
                    height = canvasHeight;
                    left = i * width;
                    top = 0;
                }
                else if (count == 4)
                {
                    width = canvasWidth / 2;
                    height = canvasHeight / 2;
                    left = (i % 2) * width;
                    top = (i / 2) * height;
                }
                else
                {
                    width = canvasWidth / 2;
                    height = canvasHeight / 2;
                    left = (i % 2) * width;
                    top = (i / 2) * height;
                }

                if (child is FrameworkElement fe)
                {
                    fe.Width = width;
                    fe.Height = height;
                    Canvas.SetLeft(fe, left);
                    Canvas.SetTop(fe, top);
                }
            }
        }

        // Update error codes list
        public void DisplayErrorCodes(List<string> codes)
        {
            ErrorCodesList.Items.Clear();
            foreach (var code in codes)
            {
                ErrorCodesList.Items.Add(code);
            }
        }

        // OBD Bluetooth connection button handlers (to be implemented)
        private void ConnectObdButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Find the OBDII device among paired devices
                var obdDevice = devices.FirstOrDefault(d => d.DeviceName.Contains(obdDeviceName));
                if (obdDevice == null)
                {
                    MessageBox.Show("OBDII device not found. Please pair the device first.", "Device Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Connect to the OBDII device
                bluetoothClient.Connect(obdDevice.DeviceAddress, BluetoothService.SerialPort);
                MessageBox.Show("Connected to OBDII device.", "Connection Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                // Start the timer for live data polling
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to OBDII device: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisconnectObdButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Stop the timer if it's running
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }

                // Disconnect the Bluetooth client
                bluetoothClient.Dispose();
                MessageBox.Show("Disconnected from OBDII device.", "Disconnection Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error disconnecting from OBDII device: {ex.Message}", "Disconnection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Request and read live data from the OBDII device
                var stream = bluetoothClient.GetStream();
                if (stream.CanRead)
                {
                    // Example: Read RPM data (PID 0C)
                    byte[] request = new byte[] { 0x01, 0x0C, 0x00 };
                    stream.Write(request, 0, request.Length);

                    // Read the response
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // Parse the RPM value from the response
                    if (bytesRead >= 3 && buffer[0] == 0x41 && buffer[1] == 0x0C)
                    {
                        int rpm = ((buffer[2] * 256) + buffer[3]) / 4;
                        // Update the graph or UI element with the RPM value
                        // Example: Update a specific graph control in the current workspace
                        var graph = workspaces[currentWorkspace].FirstOrDefault();
                        if (graph != null)
                        {
                            graph.UpdateDataPoint(rpm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading live data: {ex.Message}", "Data Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
