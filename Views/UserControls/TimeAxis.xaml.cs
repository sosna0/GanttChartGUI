using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project.Views.UserControls
{
    /// <summary>
    /// Logika interakcji dla klasy Axis.xaml
    /// </summary>
    public partial class TimeAxis : UserControl
    {
        public TimeAxis()
        {
            InitializeComponent();
            SizeChanged += TimeAxis_SizeChanged;
        }

        private void TimeAxis_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Redraw();
        }

        private void Redraw()
        {
            CanvasAxis.Children.Clear();

            double canvasWidth = CanvasAxis.ActualWidth;
            double canvasHeight = CanvasAxis.ActualHeight;
            // 25aby miec godzine odstępu od początku i końca
            double totalMinutes = 26 * 60;

            for (int hour = 0; hour <= 24; hour++)
            {
                // przesunięte o 60 minut
                double x = ((60 + hour*60) / totalMinutes) * canvasWidth;
 
                // Linia pionowa (główna godzina)
                Line hourMark = new Line
                {
                    X1 = x,
                    Y1 = canvasHeight / 2 - 10,
                    X2 = x,
                    Y2 = canvasHeight - 10,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                CanvasAxis.Children.Add(hourMark);

                // Etykieta czasu
                TextBlock label = new TextBlock
                {
                    Text = hour < 24 ? new TimeOnly(hour, 0).ToString() : "00:00",
                    Foreground = Brushes.Black,
                    FontSize=12
                };
                Canvas.SetLeft(label, x - 15);
                Canvas.SetTop(label, 0);
                CanvasAxis.Children.Add(label);

                // Mniejsze podziałki co 10 min
                if (hour < 24)
                {
                    for (int i = 1; i < 6; i++) // 10, 20, ..., 50
                    {
                        double subX = x + ((i * 10.0 / totalMinutes) * canvasWidth);

                        Line tick = new Line
                        {
                            X1 = subX,
                            Y1 = canvasHeight / 2 - 5,
                            X2 = subX,
                            Y2 = canvasHeight - 15,
                            Stroke = Brushes.Gray,
                            StrokeThickness = 0.5
                        };
                        CanvasAxis.Children.Add(tick);
                    }
                }
            }
        }
    }
}
