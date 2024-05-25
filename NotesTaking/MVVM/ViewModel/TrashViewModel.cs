using NotesTaking.MVVM.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MySql.Data.MySqlClient;

namespace NotesTaking.MVVM.ViewModel
{
    public class TrashViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Note> _trashNotes;

        public ObservableCollection<Note> TrashNotes
        {
            get => _trashNotes;
            set
            {
                _trashNotes = value;
                OnPropertyChanged(nameof(TrashNotes));
            }
        }

        public TrashViewModel()
        {
            TrashNotes = new ObservableCollection<Note>();
        }

        public void LoadTrashedNotes(int accountId, DatabaseManager dbManager)
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

            TrashNotes = trashedNotes;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
