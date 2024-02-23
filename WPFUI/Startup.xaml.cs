using Microsoft.Win32;
using SOSCSRPG.Models;
using SOSCSRPG.Services;
using System.Windows;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        private const string SAVE_GAME_FILE_EXTENSION = "soscsrpg";
        public Startup()
        {
            InitializeComponent();
            DataContext = GameDetailsService.ReadGameDetails();
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

        private void LoadSavedGame_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = $"Saved games (*.{SAVE_GAME_FILE_EXTENSION})|*.{SAVE_GAME_FILE_EXTENSION}"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                GameState gameState =
                    SaveGameService.LoadLastSaveOrCreateNew(openFileDialog.FileName);

                MainWindow mainWindow =
                    new MainWindow(gameState.Player,
                                   gameState.XCoordinate,
                                   gameState.YCoordinate);
                mainWindow.Show();
                Close();
            }
        }
    }
}
