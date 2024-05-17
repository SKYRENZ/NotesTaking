using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using NotesTaking.MVVM.ViewModel;

namespace NotesTaking.MVVM.View
{
    public partial class NotesControl : UserControl
    {
        public ObservableCollection<Note> Notes { get; set; }

        public NotesControl()
        {
            InitializeComponent();
            Notes = new ObservableCollection<Note>();
            NotesItemsControl.ItemsSource = Notes; // Updated line
            LoadNotes();
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
                    NoteContent = createNoteWindow.NoteContent
                };
                Notes.Add(newNote);
            }
        }

        private void LoadNotes()
        {
            string loggedInUsername = UserSession.LoggedInUsername;
            DatabaseManager dbManager = new DatabaseManager();
            int accountId = dbManager.GetLoggedInAccountId(loggedInUsername);

            if (accountId != -1)
            {
                using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT NoteTitle, NoteContent FROM notes WHERE AccountID = @AccountID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@AccountID", accountId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Note note = new Note
                            {
                                NoteTitle = reader.GetString("NoteTitle"),
                                NoteContent = reader.GetString("NoteContent")
                            };
                            Notes.Add(note);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    

    public class Note
    {
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
    }

    private void NoteItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        {
            // Retrieve the clicked note
            Border border = sender as Border;
            if (border != null)
            {
                Note clickedNote = border.DataContext as Note;
                if (clickedNote != null)
                {
                    // Expand the clicked note, for example, show details in a popup or navigate to a detailed view
                    MessageBox.Show($"Title: {clickedNote.NoteTitle}\nContent: {clickedNote.NoteContent}", "Note Details");
                }
            }
        }
    }
}
}



