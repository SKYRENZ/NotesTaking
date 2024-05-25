using System.Windows;

namespace NotesTaking
{
    public partial class LogoutConfirmationWindow : Window
    {
        public LogoutConfirmationWindow()
        {
            InitializeComponent();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            // Perform logout actions
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            Close(); // Close the current window
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Close(); // Close the current window without performing logout actions
        }
    }
}
