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

        public MainWindow() {
            Teams = new();
            InitializeComponent();
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
    }
}