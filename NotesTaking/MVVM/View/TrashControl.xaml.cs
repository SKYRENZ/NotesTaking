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
           
            TrashedNotes = new ObservableCollection<Note>();
           

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
            
        }
    }
}
