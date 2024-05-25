using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using NotesTaking.MVVM.Model;
using NotesTaking.MVVM.ViewModel;

namespace NotesTaking.MVVM.View
{
    public partial class NotesControl : UserControl
    {
        public ObservableCollection<Note> Notes { get; set; }
        private DatabaseManager dbManager; // Add an instance of DatabaseManager

        public NotesControl()
        {
            InitializeComponent();
            Notes = new ObservableCollection<Note>();
            NotesItemsControl.ItemsSource = Notes; // Set the item source

            dbManager = new DatabaseManager(); // Initialize DatabaseManager instance
            int accountId = dbManager.GetLoggedInAccountId(UserSession.LoggedInUsername); // Get the logged-in user's account ID
            if (accountId != -1)
            {
                LoadNotes(accountId);
            }
            else
            {
                MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        public void LoadNotes(int accountId) // Make this method void and take accountId as parameter
        {
            ObservableCollection<Note> notes = new ObservableCollection<Note>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString)) // Use dbManager.ConnectionString
                {
                    connection.Open();

                    string query = "SELECT NotesID, NoteTitle, NoteContent FROM notes WHERE AccountID = @AccountID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@AccountID", accountId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Note note = new Note
                            {
                                NotesID = reader.GetInt32("NotesID"), // Ensure NotesID is read from the database
                                NoteTitle = reader.GetString("NoteTitle"),
                                NoteContent = reader.GetString("NoteContent")
                            };
                            notes.Add(note);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log: Exception
                Console.WriteLine($"Exception: {ex.Message}");
            }

            Notes = notes; // Set the loaded notes
            NotesItemsControl.ItemsSource = Notes; // Refresh the item source
        }

    }
}
