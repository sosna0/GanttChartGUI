using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    /// Logika interakcji dla klasy TeamLabel.xaml
    /// </summary>
    public partial class TeamLabel : UserControl
    {
        public static readonly DependencyProperty TeamNameProperty =
            DependencyProperty.Register(
            nameof(TeamName),
            typeof(string),
            typeof(TeamLabel),
            new PropertyMetadata(string.Empty));

        public string TeamName
        {
            get => (string)GetValue(TeamNameProperty);
            set => SetValue(TeamNameProperty, value);
        }

        public TeamLabel()
        {
            InitializeComponent();
        }
    }
}
