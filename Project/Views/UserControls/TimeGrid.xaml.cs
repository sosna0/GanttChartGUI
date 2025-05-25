using System;
using System.Collections.Generic;
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
using Parser.Models;

namespace Project.Views.UserControls
{
    /// <summary>
    /// Interaction logic for TimeGrid.xaml
    /// </summary>
    public partial class TimeGrid : UserControl
    {
        // aktywnośc -> (czas, ile_trwało)
        private ScheduleMap Activities;

        public static readonly DependencyProperty TeamColorProperty =
            DependencyProperty.Register(
            nameof(TeamColor),
            typeof(SolidColorBrush),
            typeof(TimeGrid),
            new PropertyMetadata(Brushes.Black));

        public SolidColorBrush TeamColor
        {
            get => (SolidColorBrush)GetValue(TeamColorProperty);
            set => SetValue(TeamColorProperty, value);
        }

        public static readonly DependencyProperty StartHourProperty =
            DependencyProperty.Register(
            nameof(StartHour),
            typeof(int),
            typeof(TimeGrid),
            new PropertyMetadata(0));

        public static readonly DependencyProperty EndHourProperty =
            DependencyProperty.Register(
                nameof(EndHour),
                typeof(int),
                typeof(TimeGrid),
                new PropertyMetadata(24));

        public int StartHour
        {
            get => (int)GetValue(StartHourProperty);
            set => SetValue(StartHourProperty, value);
        }

        public int EndHour
        {
            get => (int)GetValue(EndHourProperty);
            set => SetValue(EndHourProperty, value);
        }

        public TimeGrid(ScheduleMap activities)
        {
            InitializeComponent();
            Activities = activities;
        }

        private int GetTotalMinutes(TimeOnly time)
        {
            return (time.Minute + (time.Hour * 60)) - (StartHour * 60);
        }

        private void Draw()
        {
            double canvasHeight = CanvasContent.ActualHeight;
            double canvasWidth = CanvasContent.ActualWidth;

            double totalMinutes = (EndHour - StartHour) * 60;

            // Chce naprzemian rysować etykietę na górze prostokąta i na dole (inaczej będa na siebie nachodzić)
            bool drawLabelTop = true;

            foreach (var activity in Activities)
            {
                int startMinutes = GetTotalMinutes(activity.Value.Start);

                double xPosition = (startMinutes / totalMinutes) * canvasWidth;

                double rectWidth = (activity.Value.Duration / totalMinutes) * canvasWidth;

                // Prostokąt aktywności
                Border border = new Border
                {
                    Width = rectWidth,
                    Height = canvasHeight * 0.4,
                    Background = TeamColor,
                    BorderBrush = Brushes.Black,      // kolor obramowania
                    BorderThickness = new Thickness(2.5), // grubość obramowania
                    CornerRadius = new CornerRadius(10)
                };

                Canvas.SetLeft(border, xPosition);
                Canvas.SetTop(border, (canvasHeight - border.Height)/ 2);  // Centruj prostokąt na Y

                // Etykieta aktywności
                TextBlock label = new TextBlock
                {
                    Text = activity.Key,
                    Foreground = new SolidColorBrush(Colors.Black),
                    Background = new SolidColorBrush(Colors.White),
                    FontSize = 11
                };

                // Pomiar aby dostać szerokośc etykiey
                label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                label.Arrange(new Rect(label.DesiredSize));

                Canvas.SetLeft(label, xPosition + rectWidth/2 - label.DesiredSize.Width/2);
                Canvas.SetTop(label, drawLabelTop ? 0 : canvasHeight - label.DesiredSize.Height);
                drawLabelTop = !drawLabelTop;

                // Dodajemy prostokąt i etykiętę do Canvas
                CanvasContent.Children.Add(border);
                CanvasContent.Children.Add(label);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasContent.Children.Clear();
            Draw();
        }
    }
}
