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
using Project.Views.UserControls;

namespace Project {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Dictionary<string, Dictionary<string, Tuple<TimeOnly, int>>> Teams = new()
        {
            ["Straż Pożarna"] = new Dictionary<string, Tuple<TimeOnly, int>>
            {
                ["Gaszenie pożaru magazynu"] = new Tuple<TimeOnly, int>(new TimeOnly(8, 30), 90),
                ["Kontrola hydrantów"] = new Tuple<TimeOnly, int>(new TimeOnly(11, 0), 45),
                ["Ćwiczenia z użyciem drabin"] = new Tuple<TimeOnly, int>(new TimeOnly(13, 15), 60),
                ["Obsługa festynu rodzinnego"] = new Tuple<TimeOnly, int>(new TimeOnly(18, 00), 60*4),
            },

            ["Policja"] = new Dictionary<string, Tuple<TimeOnly, int>>
            {
                ["Kontrola drogowa"] = new Tuple<TimeOnly, int>(new TimeOnly(7, 0), 60*2),
                ["Patrol w centrum miasta"] = new Tuple<TimeOnly, int>(new TimeOnly(9, 0), 60*2),
                ["Zabezpieczenie miejsca wypadku"] = new Tuple<TimeOnly, int>(new TimeOnly(12, 30), 75),
                ["Kontrola drogowa1"] = new Tuple<TimeOnly, int>(new TimeOnly(15, 0), 60),
                ["Kontrola drogowa2"] = new Tuple<TimeOnly, int>(new TimeOnly(17, 0), 60),
                ["Kontrola drogowa3"] = new Tuple<TimeOnly, int>(new TimeOnly(18, 0), 60)
            },

            ["Zespół Ratownictwa Medycznego"] = new Dictionary<string, Tuple<TimeOnly, int>>
            {
                ["Transport pacjenta"] = new Tuple<TimeOnly, int>(new TimeOnly(7, 45), 60),
                ["Udzielenie pomocy po wypadku"] = new Tuple<TimeOnly, int>(new TimeOnly(10, 30), 90),
                ["Szkolenie z resuscytacji"] = new Tuple<TimeOnly, int>(new TimeOnly(14, 0), 45),
                ["Udzielenie pomocy po wypadku1"] = new Tuple<TimeOnly, int>(new TimeOnly(16, 0), 30),
                ["Udzielenie pomocy po wypadku2"] = new Tuple<TimeOnly, int>(new TimeOnly(17, 0), 30)
            },

            ["Grupa WOPR"] = new Dictionary<string, Tuple<TimeOnly, int>>
            {
                ["Patrolowanie kąpieliska"] = new Tuple<TimeOnly, int>(new TimeOnly(9, 15), 120),
                ["Reakcja na wezwanie o topiącym się"] = new Tuple<TimeOnly, int>(new TimeOnly(13, 0), 30),
                ["Szkolenie z użyciem łodzi"] = new Tuple<TimeOnly, int>(new TimeOnly(15, 30), 60)
            },

            ["GOPR"] = new Dictionary<string, Tuple<TimeOnly, int>>
            {
                ["Akcja ratunkowa w górach"] = new Tuple<TimeOnly, int>(new TimeOnly(8, 0), 180),
                ["Szkolenie z użyciem helikoptera"] = new Tuple<TimeOnly, int>(new TimeOnly(12, 15), 90),
                ["Oznaczanie niebezpiecznych tras"] = new Tuple<TimeOnly, int>(new TimeOnly(15, 0), 60)
            },

            ["TOPR"] = new Dictionary<string, Tuple<TimeOnly, int>>
            {
                ["Akcja ratunkowa w górach"] = new Tuple<TimeOnly, int>(new TimeOnly(6, 0), 250),
                ["Szkolenie z użyciem helikoptera"] = new Tuple<TimeOnly, int>(new TimeOnly(10, 30), 90),
                ["Oznaczanie niebezpiecznych tras"] = new Tuple<TimeOnly, int>(new TimeOnly(12, 0), 60),
                ["Patrolowanie szlaków turystycznych"] = new Tuple<TimeOnly, int>(new TimeOnly(14, 0), 120)
            }
        };

        private const int LABEL_WIDTH = 120;
        private const int ROW_HEIGHT = 70;

        public MainWindow() {
            InitializeComponent();
            DrawTimeGrids();
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
                timeGrid.TeamColor = new SolidColorBrush(Color.FromRgb((byte)(new Random().Next(256)), (byte)(new Random().Next(256)), (byte)(new Random().Next(256))));

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
                // Tutaj parsowanie i sprawdzenie błędów
                Errors.Text = zawartosc;

                // Jak git to
                // DrawTimeGrids()
            }

        }
    }
}