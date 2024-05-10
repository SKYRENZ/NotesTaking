using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace NotesTaking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SolidColorBrush? originalFill, originalStroke, originalFillMinimize, originalStrokeMinimize;
        private string connectionString = "server=localhost;user=root;database=notetaking_test;port=3306";
        public MainWindow()
        {

            InitializeComponent();

            //Revert Color of Close Button
            originalFill = btnClose.Fill as SolidColorBrush;
            originalStroke = btnClose.Stroke as SolidColorBrush;

            //Revert Color of Minimize Button
            originalFillMinimize = btnMinimize.Fill as SolidColorBrush;
            originalStrokeMinimize = btnMinimize.Stroke as SolidColorBrush;
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        
        private void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            //Changes the color of close button when hovered
            btnClose.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C7282D"));
            btnClose.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF343A"));
        }

        private void btnMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        
        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //Shows Dashboard
            // Dashboard mainDashboard = new Dashboard();
            //this.Visibility = Visibility.Hidden;
            // mainDashboard.Show();
            string username = txtUsername.Text.Trim(); // Get the username from the TextBox
            string password = txtPassword.Password.Trim(); // Get the password from the PasswordBox

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM account WHERE AccountUser = @username AND AccountPass = @password";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count > 0)
                    {
                        // Login successful, show dashboard
                        Dashboard mainDashboard = new Dashboard();
                        this.Visibility = Visibility.Hidden;
                        mainDashboard.Show();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    



        private void btnCreateAcc(object sender, MouseButtonEventArgs e)
        {

        }
        
        private void btnMinimize_MouseEnter(object sender, MouseEventArgs e)
        {
            //Changes the color of minimize button when hovered
            btnMinimize.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C79436"));
            btnMinimize.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBE45"));
        }

        private void btnMinimize_MouseLeave(object sender, MouseEventArgs e)
        {
            //reverts color to original minimize button
            btnMinimize.Fill = originalFillMinimize;
            btnMinimize.Stroke = originalStrokeMinimize;
        }

        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {   
            //reverts color to original Close button
            btnClose.Fill = originalFill;
            btnClose.Stroke = originalStroke;
        }
    }
}