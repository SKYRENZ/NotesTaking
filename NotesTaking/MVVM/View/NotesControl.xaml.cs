using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using NotesTaking.MVVM.Model;
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
                Note newNote = createNoteWindow.NewNote;
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
                Notes = dbManager.LoadNotes(accountId);
                NotesItemsControl.ItemsSource = Notes;
            }
            else
            {
                MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Button saveButton = sender as Button;
            if (saveButton != null)
            {
                Note noteToSave = saveButton.DataContext as Note;
                if (noteToSave != null)
                {
                    // Get the logged-in user's account ID
                    string loggedInUsername = UserSession.LoggedInUsername;
                    DatabaseManager dbManager = new DatabaseManager();
                    int accountId = dbManager.GetLoggedInAccountId(loggedInUsername);

                    if (accountId != -1)
                    {
                        try
                        {
                            using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                            {
                                connection.Open();

                                // Prepare the SQL UPDATE query
                                string query = "UPDATE notes SET NoteTitle = @NoteTitle, NoteContent = @NoteContent WHERE AccountID = @AccountID OR NotesID = @NotesID";
                                MySqlCommand command = new MySqlCommand(query, connection);
                                command.Parameters.AddWithValue("@NoteTitle", noteToSave.NoteTitle);
                                command.Parameters.AddWithValue("@NoteContent", noteToSave.NoteContent);
                                command.Parameters.AddWithValue("@AccountID", accountId);
                                command.Parameters.AddWithValue("@NotesID", noteToSave.NotesID);

                                // Execute the SQL UPDATE query
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Note successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Failed to update note.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating note: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    // Hide the Save button and show the Edit button
                    StackPanel editPanel = saveButton.Parent as StackPanel;
                    if (editPanel != null)
                    {
                        Button editButton = editPanel.FindName("EditButton") as Button;
                        if (editButton != null)
                        {
                            editButton.Visibility = Visibility.Visible;
                        }
                        saveButton.Visibility = Visibility.Collapsed;
                        editPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }



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
                        DatabaseManager dbManager = new DatabaseManager();
                        int accountId = dbManager.GetLoggedInAccountId(loggedInUsername);

                        if (accountId != -1)
                        {
                            using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                            {
                                connection.Open();

                                // Prepare the SQL INSERT query to move note to trash table
                                string insertTrashQuery = "INSERT INTO trash (AccountID, NoteID, TrashedDate, TrashTitle, TrashContent) " +
                                                         "SELECT @AccountID, NotesID, NOW(), Title, Content FROM notes WHERE AccountID = @AccountID AND NotesID = @NotesID";
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
