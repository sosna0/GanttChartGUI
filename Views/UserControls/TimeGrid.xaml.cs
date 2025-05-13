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

namespace Project.Views.UserControls
{
    /// <summary>
    /// Interaction logic for TimeGrid.xaml
    /// </summary>
    public partial class TimeGrid : UserControl
    {
        // aktywnośc -> (czas, ile_trwało)
        private Dictionary<string, Tuple<TimeOnly, int>> Activities;

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

        public TimeGrid()
        {
            InitializeComponent();
            Activities = new() {
                {
                    "gaszenie pożaru", new Tuple<TimeOnly, int>(new TimeOnly(9, 0), 60)
                },
                {
                    "pierwsza pomoc", new Tuple<TimeOnly, int>(new TimeOnly(10, 30), 30)
                },
                {
                    "zabezpieczanie terenu", new Tuple<TimeOnly, int>(new TimeOnly(11, 35), 30)
                },
                {
                    "wyważanie drzwi", new Tuple<TimeOnly, int>(new TimeOnly(12, 45), 10)
                },
                {
                    "wyniesienie poszkodowanych z pożaru", new Tuple<TimeOnly, int>(new TimeOnly(15, 35), 60)
                }
            };
        }

        // Przesunięte o 60 minut do przodu
        private int GetTotalMinutes(TimeOnly time)
        {
            return 60 + time.Minute + (time.Hour * 60);
        }

        private void Draw()
        {
            double canvasHeight = CanvasContent.ActualHeight;
            double canvasWidth = CanvasContent.ActualWidth;

            double minutesInDay = 26 * 60;

            // Chce naprzemian rysować etykietę na górze prostokąta i na dole (inaczej będa na siebie nachodzić)
            bool drawLabelTop = true;

            foreach (var activity in Activities)
            {
                int startMinutes = GetTotalMinutes(activity.Value.Item1);

                double xPosition = (startMinutes / minutesInDay) * canvasWidth;

                double rectWidth = (activity.Value.Item2 / minutesInDay) * canvasWidth;

                // Prostokąt aktywności
                Rectangle rect = new Rectangle
                {
                    Width = rectWidth,
                    Height = canvasHeight * 0.4,
                    Fill = TeamColor,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Canvas.SetLeft(rect, xPosition);
                Canvas.SetTop(rect, (canvasHeight - rect.Height)/ 2);  // Centruj prostokąt na Y

                // Etykieta aktywności
                TextBlock label = new TextBlock
                {
                    Text = activity.Key,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12
                };

                // Pomiar aby dostać szerokośc etykiey
                label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                label.Arrange(new Rect(label.DesiredSize));

                Canvas.SetLeft(label, xPosition + rectWidth/2 - label.DesiredSize.Width/2);
                Canvas.SetTop(label, drawLabelTop ? 0 : canvasHeight - label.DesiredSize.Height);
                drawLabelTop = !drawLabelTop;

                // Dodajemy prostokąt i etykiętę do Canvas
                CanvasContent.Children.Add(rect);
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
