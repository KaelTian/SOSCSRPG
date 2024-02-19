using Engine.Models;
using Engine.Services;
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
using System.Windows.Shapes;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        private GameDetails _gameDetails;
        public Startup()
        {
            InitializeComponent();
            _gameDetails = GameDetailsService.ReadGameDetails();
            DataContext = _gameDetails;
        }

        private void StartNewGame_OnClick(object sender, RoutedEventArgs e)
        {
            CharacterCreation characterCreation = new CharacterCreation();
            characterCreation.Show();
            Close();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
