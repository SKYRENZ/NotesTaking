using NotesTaking.MVVM.Model;
using NotesTaking.MVVM.ViewModel;
using Org.BouncyCastle.Utilities;
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
            string loggedInUsername = UserSession.LoggedInUsername; // Retrieve the logged-in username

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

            // Get the logged-in account ID using the DatabaseManager
            DatabaseManager dbManager = new DatabaseManager();
            int accountId = dbManager.GetLoggedInAccountId(loggedInUsername);

            if (accountId == -1)
            {
                Console.WriteLine("Error: Unable to find account for logged-in user.");
                return;
            }

            // Insert the note into the database
            bool isInserted = dbManager.InsertNote(accountId, NoteTitle, NoteContent);

            if (isInserted)
            {
                Console.WriteLine("Note added successfully.");
                DialogResult = true; // Indicate the note was successfully created
            }
            else
            {
                Console.WriteLine("Error: Unable to add the note.");
                DialogResult = false; // Indicate the note creation failed
            }

            // Close the window
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
                
            }
        }

    }
}
