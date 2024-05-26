using NotesTaking.MVVM.Model;
using NotesTaking.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace NotesTaking.MVVM.View
{
    public partial class CreateNoteWindow : Window
    {
        public string NoteTitle { get; private set; }
        public string NoteContent { get; private set; }

        public CreateNoteWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            NoteTitle = txtTitle.Text;
            NoteContent = txtContent.Text;
            string loggedInUsername = UserSession.LoggedInUsername;

            if (string.IsNullOrWhiteSpace(NoteTitle) || NoteTitle == "Note Title:")
            {
                MessageBox.Show("Please enter a title for the note.", "Missing Title", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NoteContent) || NoteContent == "Note...")
            {
                MessageBox.Show("Please enter content for the note.", "Missing Content", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(loggedInUsername))
            {
                Console.WriteLine("Error: No user is currently logged in.");
                return;
            }

            DatabaseManager dbManager = new DatabaseManager();
            bool isInserted = dbManager.CreateNoteForLoggedInUser(loggedInUsername, NoteTitle, NoteContent);

            if (isInserted)
            {
                Console.WriteLine("Note added successfully.");
                DialogResult = true;
            }
            else
            {
                Console.WriteLine("Error: Unable to add the note.");
                DialogResult = false;
            }

            this.Close();
        }

        private void txtTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtTitle.Text == "Note Title:")
            {
                txtTitle.Text = string.Empty;
                txtTitle.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                txtTitle.Text = "Note Title:";
                txtTitle.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void txtContent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtContent.Text == "Note...")
            {
                txtContent.Text = string.Empty;
                txtContent.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtContent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                txtContent.Text = "Note...";
                txtContent.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateNoteButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNoteWindow createNoteWindow = new CreateNoteWindow();
            bool? result = createNoteWindow.ShowDialog();

            if (result == true)
            {
                Note newNote = new Note
                {
                    NoteTitle = createNoteWindow.NoteTitle,
                    NoteContent = createNoteWindow.NoteContent // Assign the content from the textbox
                };

                // Add additional logic if needed, e.g., updating the UI
            }
        }
    }
}
