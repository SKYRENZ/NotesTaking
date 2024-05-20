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

        private void EditNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Note noteToEdit = button.DataContext as Note;
                if (noteToEdit != null)
                {
                    // Toggle visibility of edit panel
                    StackPanel editPanel = button.FindName("EditPanel") as StackPanel;
                    if (editPanel != null)
                    {
                        editPanel.Visibility = (editPanel.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

                        // Toggle visibility of Edit and Save buttons
                        Button saveButton = editPanel.FindName("SaveButton") as Button;
                        if (saveButton != null)
                        {
                            saveButton.Visibility = (saveButton.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
                        }
                        button.Visibility = (button.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
        }

        // Removed SaveNoteButton_Click method

        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;
            if (deleteButton != null)
            {
                Note noteToDelete = deleteButton.DataContext as Note;
                if (noteToDelete != null)
                {
                    // Remove note from UI
                    Notes.Remove(noteToDelete);

                    try
                    {
                        // Get the logged-in user's account ID
                        string loggedInUsername = UserSession.LoggedInUsername;
                        int accountId = dbManager.GetLoggedInAccountId(loggedInUsername);

                        if (accountId != -1)
                        {
                            using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                            {
                                connection.Open();

                                // Prepare the SQL INSERT query to move note to trash table
                                string insertTrashQuery = "INSERT INTO trash (AccountID, NoteID, TrashedDate, TrashTitle, TrashContent) " +
                                                         "SELECT @AccountID, NotesID, NOW(), NoteTitle, NoteContent FROM notes WHERE AccountID = @AccountID AND NotesID = @NotesID";
                                MySqlCommand insertTrashCommand = new MySqlCommand(insertTrashQuery, connection);
                                insertTrashCommand.Parameters.AddWithValue("@AccountID", accountId);
                                insertTrashCommand.Parameters.AddWithValue("@NotesID", noteToDelete.NotesID);
                                insertTrashCommand.ExecuteNonQuery();

                                string deleteNoteQuery = "DELETE FROM notes WHERE AccountID = @AccountID AND NotesID = @NotesID";
                                MySqlCommand deleteNoteCommand = new MySqlCommand(deleteNoteQuery, connection);
                                deleteNoteCommand.Parameters.AddWithValue("@AccountID", accountId);
                                deleteNoteCommand.Parameters.AddWithValue("@NotesID", noteToDelete.NotesID); // Pass the note's ID

                                deleteNoteCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error moving note to trash: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
