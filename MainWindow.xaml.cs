using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Project.Models;
using Project.Views.UserControls;

namespace Project {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Dictionary<string, Dictionary<string, Tuple<TimeOnly, int>>> Teams;

        private const int LABEL_WIDTH = 120;
        private const int ROW_HEIGHT = 70;

        private static readonly Random rand = new Random();

        public MainWindow() {
            Teams = new();
            InitializeComponent();
            this.StateChanged += Window_StateChanged;
            DrawLogo();
        }

        //Opcjonalnie możemy wyświetlać na początku logo - zastanowić się, czy tak robimy
        private void DrawLogo() 
        {
            Logo logo = new Logo();
            logo.Width = 500;
            logo.Height = 500;
            logo.HorizontalAlignment = HorizontalAlignment.Center;
            TeamGantt.Children.Add(logo);
        }

        private void SethoursTimeAxis(int startHour, int endHour)
        {
            TimeAxis.StartHour = startHour;
            TimeAxis.EndHour = endHour;
        }

        private void DrawTimeGrids()
        {
            var (HourStart, HourEnd) = GetStartEndHours();
            SethoursTimeAxis(HourStart, HourEnd);

            var startingColorIndex = rand.Next(360);
            //Debug.WriteLine(startingColorIndex);
            var colors = GenerateColors(Teams.Count, startingColorIndex, startingColorIndex + 120);

            int i = 0;
            foreach (var team in Teams)
            {
                // Etykieta
                TeamLabel teamLabel = new TeamLabel();
                teamLabel.Width = LABEL_WIDTH;
                teamLabel.Height = ROW_HEIGHT;
                teamLabel.TeamName = team.Key;
                TeamLabels.Children.Add(teamLabel);

                // Wykres
                TimeGrid timeGrid = new TimeGrid(team.Value);
                timeGrid.Height = ROW_HEIGHT;
                timeGrid.StartHour = HourStart;
                timeGrid.EndHour = HourEnd;
                timeGrid.TeamColor = colors[i];
                i++;

                // Szerokośc skali = szerokośc wykresu
                var binding = new Binding("ActualWidth")
                {
                    Source = TimeAxis,   // kontrolka, do której chcemy się powiązać
                    Mode = BindingMode.OneWay
                };

                timeGrid.SetBinding(WidthProperty, binding);
                TeamGantt.Children.Add(timeGrid);   
            }
        }

        private Tuple<int, int> GetStartEndHours()
        {
            TimeOnly? earliestStart = null;
            TimeOnly? latestEnd = null;

            foreach (var team in Teams)
            {
                foreach (var activity in team.Value)
                {
                    TimeOnly start = activity.Value.Item1;
                    int durationMinutes = activity.Value.Item2;
                    TimeOnly end = start.AddMinutes(durationMinutes);

                    if (earliestStart == null || start < earliestStart)
                        earliestStart = start;

                    if (latestEnd == null || end > latestEnd)
                        latestEnd = end;
                }
            }

            if(earliestStart == null || latestEnd == null)
                throw new InvalidOperationException("Nie można znaleźć godzin rozpoczęcia i zakończenia.");

            int startHour = earliestStart.Value.Hour;
            int endHour = latestEnd.Value.Minute > 0 ? latestEnd.Value.Hour + 1 : latestEnd.Value.Hour;

            return new Tuple<int, int>(startHour, endHour);
        }

        private void Reset()
        {
            TeamLabels.Children.Clear();
            TeamGantt.Children.Clear();
            Teams.Clear();
            Teams = new Dictionary<string, Dictionary<string, Tuple<TimeOnly, int>>>();
            SethoursTimeAxis(0, 24);
        }

        private void FileLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // Czyszczenie starego wykresu
                Reset();
                string zawartosc = File.ReadAllText(openFileDialog.FileName);

                try
                {
                    this.Teams = InputParser.Parse(zawartosc);

                    // Tutaj jeszcze można zrobić walidacje

                    // Rzucić wyjątek jak czasy się na siebie nakładają

                    DrawTimeGrids();
                }
                catch (Exception ex)
                {
                    Errors.Text = ex.Message;
                }
                   
            }

        }


        // Funkcje do ustawienia kolorów dla wykresów

        private static List<SolidColorBrush> GenerateColors(int count, double hueStart, double hueEnd, double saturation = 0.5) {
            var colors = new List<SolidColorBrush>();
            double hueRange = (hueEnd + 360 - hueStart) % 360;
            double step = hueRange / count;

            for (int i = 0; i < count; i++) {
                double hue = (hueStart + step * i) % 360;
                Color color = FromHSV(hue, saturation, 1.0);
                colors.Add(new SolidColorBrush(color));
            }

            return colors;
        }


        private static Color FromHSV(double hue, double saturation, double value) {
            int hue_index = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = (byte)value;
            byte p = (byte)(value * (1 - saturation));
            byte q = (byte)(value * (1 - f * saturation));
            byte t = (byte)(value * (1 - (1 - f) * saturation));

            return hue_index switch {
                0 => Color.FromRgb(v, t, p),
                1 => Color.FromRgb(q, v, p),
                2 => Color.FromRgb(p, v, t),
                3 => Color.FromRgb(p, q, v),
                4 => Color.FromRgb(t, p, v),
                5 => Color.FromRgb(v, p, q),
                _ => throw new ArgumentOutOfRangeException()
            };
        }


        //Funkcje do customowego paska narzędzi


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        //Tu zakładam że domyślnie nie mamy fullscreen'u
        private void btnMaximize_Click(object sender, RoutedEventArgs e) {
            if(WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
                btnMaximize.Content = "\uE922";
            }
            else {
                WindowState = WindowState.Maximized;
                btnMaximize.Content = "\uE923";
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            Close();
            //Application.Current.Shutdown();
        }

        private void Window_StateChanged(object sender, EventArgs e) {
            if (this.WindowState == WindowState.Maximized) {
                MainGrid.Margin = new Thickness(6); // żeby nie ucinało 
            }
            else {
                MainGrid.Margin = new Thickness(0);
            }
        }
    }
}