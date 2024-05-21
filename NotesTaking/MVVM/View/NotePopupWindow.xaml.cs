using MySql.Data.MySqlClient;
using NotesTaking.MVVM.Model;
using NotesTaking.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotesTaking.MVVM.View
{
    /// <summary>
    /// Interaction logic for NotePopupWindow.xaml
    /// </summary>
    public partial class NotePopupWindow : Window
    {

        private readonly NoteViewModel _noteViewModel;
        public NotePopupWindow()
        {
            InitializeComponent();
            _noteViewModel = new NoteViewModel();
            DataContext = _noteViewModel;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Add your delete button logic here if needed
        }

        private void btnClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Button saveButton = sender as Button;
            if (saveButton != null)
            {
                Note noteToSave = saveButton.DataContext as Note;
                if (noteToSave != null)
                {
                    // Instantiate DatabaseManager to access the connection string
                    DatabaseManager dbManager = new DatabaseManager();

                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                        {
                            connection.Open();

                            if (noteToSave.NotesID > 0)
                            {
                                // Prepare the SQL UPDATE query
                                string updateQuery = "UPDATE notes SET NoteTitle = @NoteTitle, NoteContent = @NoteContent WHERE NotesID = @NotesID";
                                MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                                updateCommand.Parameters.AddWithValue("@NoteTitle", noteToSave.NoteTitle);
                                updateCommand.Parameters.AddWithValue("@NoteContent", noteToSave.NoteContent);
                                updateCommand.Parameters.AddWithValue("@NotesID", noteToSave.NotesID);

                                // Execute the SQL UPDATE query
                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Note successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Failed to update note. Note not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid note ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating note: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            if (this.DataContext is NoteViewModel noteViewModel)
            {
                MessageBox.Show("Note saved.");
            }
            this.DialogResult = true;
            this.Close();
        }

        private void ArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            Button archiveButton = sender as Button;
            if (archiveButton != null)
            {
                Note noteToArchive = archiveButton.DataContext as Note;
                if (noteToArchive != null)
                {
                    try
                    {
                        // Move note to archive
                        ArchiveNote(noteToArchive);

                        // Delete note from notes table
                        DeleteNoteFromNotes(noteToArchive);

                        MessageBox.Show("Note archived successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Close the window after archiving
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error archiving note: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        private void ArchiveNote(Note note)
        {
            // Instantiate DatabaseManager to access the connection string
            DatabaseManager dbManager = new DatabaseManager();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                {
                    connection.Open();

                    // Prepare the SQL INSERT query to move note to archive table
                    string insertArchiveQuery = "INSERT INTO archive (AccountID, NotesID, ArchivedDate, ArchiveTitle, ArchiveContent) " +
                                                "SELECT AccountID, NotesID, NOW(), NoteTitle, NoteContent FROM notes WHERE NotesID = @NotesID";
                    MySqlCommand insertArchiveCommand = new MySqlCommand(insertArchiveQuery, connection);
                    insertArchiveCommand.Parameters.AddWithValue("@NotesID", note.NotesID);
                    insertArchiveCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error archiving note: {ex.Message}");
            }
        }

        private void DeleteNoteFromNotes(Note note)
        {
            // Instantiate DatabaseManager to access the connection string
            DatabaseManager dbManager = new DatabaseManager();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                {
                    connection.Open();

                    // Prepare the SQL DELETE query to remove note from notes table
                    string deleteNoteQuery = "DELETE FROM notes WHERE NotesID = @NotesID";
                    MySqlCommand deleteNoteCommand = new MySqlCommand(deleteNoteQuery, connection);
                    deleteNoteCommand.Parameters.AddWithValue("@NotesID", note.NotesID);
                    deleteNoteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting note from notes table: {ex.Message}");
            }
        }


    }
}
