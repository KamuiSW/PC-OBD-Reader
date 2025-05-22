using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PC_OBD_Reader
{
    public partial class MainWindow : Window
    {
        // Workspaces hold lists of graphs
        private Dictionary<string, List<ResizableGraphControl>> workspaces = new Dictionary<string, List<ResizableGraphControl>>();
        private string currentWorkspace;

        public MainWindow()
        {
            InitializeComponent();

            AddNewWorkspace("Workspace 1");
            WorkspaceSelector.SelectedIndex = 0;

            // Example: Show some error codes initially (you can update this dynamically)
            DisplayErrorCodes(new List<string> { "P0300", "P0420", "P0171" });
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
            graph.DragCompleted += Graph_DragCompleted;
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
            if (WorkspaceSelector.SelectedItem != null)
            {
                currentWorkspace = WorkspaceSelector.SelectedItem.ToString();
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

        private void Graph_DragCompleted(object sender, EventArgs e)
        {
            SnapToNearestSlot(sender as UIElement);
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
    }
}
