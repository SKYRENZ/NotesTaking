using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using NotesTaking.MVVM.Model;
using NotesTaking.MVVM.ViewModel;

namespace NotesTaking.MVVM.View
{
    public partial class ArchiveControl : UserControl
    {
        public ObservableCollection<Note> ArchivedNotes { get; set; }
        private DatabaseManager dbManager;

        public ArchiveControl()
        {
            InitializeComponent();
            ArchivedNotes = new ObservableCollection<Note>();
            ArchiveItemsControl.ItemsSource = ArchivedNotes;

            dbManager = new DatabaseManager();
            int accountId = dbManager.GetLoggedInAccountId(UserSession.LoggedInUsername);
            if (accountId != -1)
            {
                LoadArchivedNotes(accountId);
            }
            else
            {
                MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadArchivedNotes(int accountId)
        {
            ObservableCollection<Note> archivedNotes = new ObservableCollection<Note>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT ArchiveID, ArchiveTitle, ArchiveContent FROM archive WHERE AccountID = @AccountID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@AccountID", accountId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Note note = new Note
                            {
                                NotesID = reader.GetInt32("ArchiveID"),
                                NoteTitle = reader.GetString("ArchiveTitle"),
                                NoteContent = reader.GetString("ArchiveContent")
                            };
                            archivedNotes.Add(note);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            ArchivedNotes = archivedNotes;
            ArchiveItemsControl.ItemsSource = ArchivedNotes;
        }
    }
}
