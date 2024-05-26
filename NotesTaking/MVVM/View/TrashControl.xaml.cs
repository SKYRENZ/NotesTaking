using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using NotesTaking.MVVM.Model;
using NotesTaking.MVVM.ViewModel;

namespace NotesTaking.MVVM.View
{
    public partial class TrashControl : UserControl
    {
        public ObservableCollection<Note> TrashedNotes { get; set; }
        private DatabaseManager dbManager;

        public TrashControl()
        {
            InitializeComponent();
            TrashedNotes = new ObservableCollection<Note>();
            TrashItemsControl.ItemsSource = TrashedNotes;

            dbManager = new DatabaseManager();
            int accountId = dbManager.GetLoggedInAccountId(UserSession.LoggedInUsername);
            if (accountId != -1)
            {
                LoadTrashedNotes(accountId);
            }
            else
            {
                MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTrashedNotes(int accountId)
        {
            ObservableCollection<Note> trashedNotes = new ObservableCollection<Note>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT TrashID, TrashTitle, TrashContent FROM trash WHERE AccountID = @AccountID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@AccountID", accountId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Note note = new Note
                            {
                                NotesID = reader.GetInt32("TrashID"),
                                NoteTitle = reader.GetString("TrashTitle"),
                                NoteContent = reader.GetString("TrashContent")
                            };
                            trashedNotes.Add(note);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            TrashedNotes = trashedNotes;
            TrashItemsControl.ItemsSource = TrashedNotes;
        }

        private void RestoreNoteFromTrash(int accountId, int noteId)
        {
            if (accountId != -1)
            {
                if (dbManager.RestoreNoteFromTrash(accountId, noteId))
                {
                    // Note restored successfully, update UI
                    LoadTrashedNotes(accountId); // Reload trashed notes
                }
                else
                {
                    MessageBox.Show("Failed to restore note.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            Button restoreButton = (Button)sender;
            if (restoreButton.DataContext is Note selectedNote)
            {
                int accountId = dbManager.GetLoggedInAccountId(UserSession.LoggedInUsername);
                RestoreNoteFromTrash(accountId, selectedNote.NotesID);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = (Button)sender;
            if (deleteButton.DataContext is Note selectedNote)
            {
                int accountId = dbManager.GetLoggedInAccountId(UserSession.LoggedInUsername);
                DeleteNoteFromTrash(accountId, selectedNote.NotesID);
            }
        }


        private void DeleteNoteFromTrash(int accountId, int noteId)
        {
            if (accountId != -1)
            {
                if (dbManager.DeleteNoteFromTrash(accountId, noteId))
                {
                    // Note deleted successfully, update UI
                    LoadTrashedNotes(accountId); // Reload trashed notes
                }
                else
                {
                    MessageBox.Show("Failed to delete note.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
