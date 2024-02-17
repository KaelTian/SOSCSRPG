using System.Windows;

namespace WPFUI.Windows
{
    /// <summary>
    /// Interaction logic for YesNoWindow.xaml
    /// </summary>
    public partial class YesNoWindow : Window
    {
        public bool ClickedYes { get; private set; }
        public YesNoWindow(string title, string message)
        {
            InitializeComponent();
            Title = title;
            Message.Content = message;
        }

        private void No_OnClick(object sender, RoutedEventArgs e)
        {
            ClickedYes = false;
            Close();
        }

        private void Yes_OnClick(object sender, RoutedEventArgs e)
        {
            ClickedYes = true;
            Close();
        }
    }
}
