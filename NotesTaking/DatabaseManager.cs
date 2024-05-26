using MySql.Data.MySqlClient;
using NotesTaking.MVVM.Model;
using System.Collections.ObjectModel;

public class DatabaseManager
{
    public string ConnectionString { get; private set; }

    public DatabaseManager()
    {
        ConnectionString = "server=localhost;user=root;database=notetaking_test;port=3306";
    }

    public bool ValidateUser(string username, string password)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM account WHERE AccountUser = @username AND BINARY AccountPass = @password";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(command.ExecuteScalar());

                return count > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }

    public bool InsertNote(int accountId, string noteTitle, string noteContent)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "INSERT INTO notes (AccountID, NoteTitle, NoteContent) VALUES (@accountId, @noteTitle, @noteContent)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@accountId", accountId);
                command.Parameters.AddWithValue("@noteTitle", noteTitle);
                command.Parameters.AddWithValue("@noteContent", noteContent);

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }

    public int GetLoggedInAccountId(string loggedInUsername)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT AccountID FROM account WHERE AccountUser = @loggedInUsername";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@loggedInUsername", loggedInUsername);

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    Console.WriteLine($"User {loggedInUsername} not found.");
                    return -1;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return -1;
        }
    }

    public ObservableCollection<Note> LoadNotes(int accountId)
    {
        ObservableCollection<Note> notes = new ObservableCollection<Note>();

        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
                            NotesID = reader.GetInt32("NotesID"),
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
            Console.WriteLine($"Exception: {ex.Message}");
        }

        return notes;
    }

    public bool ArchiveNoteFromNotes(int accountId, int noteId)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "UPDATE notes SET IsArchived = 1 WHERE AccountID = @AccountID AND NotesID = @NotesID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountID", accountId);
                command.Parameters.AddWithValue("@NotesID", noteId);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }
    public bool CreateNoteForLoggedInUser(string loggedInUsername, string noteTitle, string noteContent)
    {
        int accountId = GetLoggedInAccountId(loggedInUsername);
        if (accountId == -1)
        {
            Console.WriteLine("Error: Unable to find account for logged-in user.");
            return false;
        }

        return InsertNote(accountId, noteTitle, noteContent);
    }

    public bool UpdateNote(Note noteToSave)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                if (noteToSave.NotesID > 0)
                {
                    string updateQuery = "UPDATE notes SET NoteTitle = @NoteTitle, NoteContent = @NoteContent WHERE NotesID = @NotesID";
                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@NoteTitle", noteToSave.NoteTitle);
                    updateCommand.Parameters.AddWithValue("@NoteContent", noteToSave.NoteContent);
                    updateCommand.Parameters.AddWithValue("@NotesID", noteToSave.NotesID);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                else
                {
                    Console.WriteLine("Invalid note ID.");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating note: {ex.Message}");
            return false;
        }
    }

    public bool ArchiveNote(Note note)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string insertArchiveQuery = "INSERT INTO archive (AccountID, NotesID, ArchivedDate, ArchiveTitle, ArchiveContent) " +
                                            "SELECT AccountID, NotesID, NOW(), NoteTitle, NoteContent FROM notes WHERE NotesID = @NotesID";
                MySqlCommand insertArchiveCommand = new MySqlCommand(insertArchiveQuery, connection);
                insertArchiveCommand.Parameters.AddWithValue("@NotesID", note.NotesID);
                insertArchiveCommand.ExecuteNonQuery();

                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error archiving note: {ex.Message}");
            return false;
        }
    }

    public bool DeleteNoteFromNotes(int noteId)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "DELETE FROM notes WHERE NotesID = @NotesID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@NotesID", noteId);

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            // Log or handle the exception appropriately
            Console.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }

    public void MoveNoteToTrash(int noteId)
    {
        // Implementation to move the note with the specified ID to trash
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // Prepare the SQL INSERT query to move note to trash table
                string insertTrashQuery = "INSERT INTO trash (AccountID, NoteID, TrashedDate, TrashTitle, TrashContent) " +
                                          "SELECT AccountID, NotesID, NOW(), NoteTitle, NoteContent FROM notes WHERE NotesID = @NotesID";
                MySqlCommand insertTrashCommand = new MySqlCommand(insertTrashQuery, connection);
                insertTrashCommand.Parameters.AddWithValue("@NotesID", noteId);
                insertTrashCommand.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error moving note to trash: {ex.Message}");
        }
    }

    public bool DeleteNoteFromDatabase(Note note)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string deleteNoteQuery = "DELETE FROM notes WHERE NotesID = @NotesID";
                MySqlCommand deleteNoteCommand = new MySqlCommand(deleteNoteQuery, connection);
                deleteNoteCommand.Parameters.AddWithValue("@NotesID", note.NotesID);
                deleteNoteCommand.ExecuteNonQuery();

                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting note from notes table: {ex.Message}");
            return false;
        }
    }

    public bool UnarchiveNote(Note note)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string insertNoteQuery = "INSERT INTO notes (AccountID, NoteTitle, NoteContent) " +
                                         "SELECT AccountID, @NoteTitle, @NoteContent FROM archive WHERE ArchiveID = @ArchiveID";
                MySqlCommand insertNoteCommand = new MySqlCommand(insertNoteQuery, connection);
                insertNoteCommand.Parameters.AddWithValue("@NoteTitle", note.NoteTitle);
                insertNoteCommand.Parameters.AddWithValue("@NoteContent", note.NoteContent);
                insertNoteCommand.Parameters.AddWithValue("@ArchiveID", note.NotesID);
                insertNoteCommand.ExecuteNonQuery();

                string deleteArchiveQuery = "DELETE FROM archive WHERE ArchiveID = @ArchiveID";
                MySqlCommand deleteArchiveCommand = new MySqlCommand(deleteArchiveQuery, connection);
                deleteArchiveCommand.Parameters.AddWithValue("@ArchiveID", note.NotesID);
                deleteArchiveCommand.ExecuteNonQuery();

                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error unarchiving note: {ex.Message}");
            return false;
        }
    }

    public ObservableCollection<Note> LoadTrashedNotes(int accountId)
    {
        ObservableCollection<Note> trashedNotes = new ObservableCollection<Note>();

        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
            throw new Exception($"Error loading trashed notes: {ex.Message}");
        }

        return trashedNotes;
    }

    public bool RestoreNoteFromTrash(int accountId, int noteId)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // Prepare the SQL INSERT query to move note back to notes table
                string insertNoteQuery = "INSERT INTO notes (AccountID, NoteTitle, NoteContent) " +
                                         "SELECT AccountID, TrashTitle, TrashContent FROM trash WHERE TrashID = @TrashID AND AccountID = @AccountID";
                MySqlCommand insertNoteCommand = new MySqlCommand(insertNoteQuery, connection);
                insertNoteCommand.Parameters.AddWithValue("@TrashID", noteId);
                insertNoteCommand.Parameters.AddWithValue("@AccountID", accountId);

                int rowsInserted = insertNoteCommand.ExecuteNonQuery();

                if (rowsInserted > 0)
                {
                    // Delete the note from the trash table
                    string deleteTrashQuery = "DELETE FROM trash WHERE TrashID = @TrashID AND AccountID = @AccountID";
                    MySqlCommand deleteTrashCommand = new MySqlCommand(deleteTrashQuery, connection);
                    deleteTrashCommand.Parameters.AddWithValue("@TrashID", noteId);
                    deleteTrashCommand.Parameters.AddWithValue("@AccountID", accountId);

                    deleteTrashCommand.ExecuteNonQuery();

                    return true;
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error restoring note from trash: {ex.Message}");
        }
    }

    public bool DeleteNoteFromTrash(int accountId, int noteId)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // Prepare the SQL DELETE query to remove note from trash table
                string deleteNoteQuery = "DELETE FROM trash WHERE TrashID = @TrashID AND AccountID = @AccountID";
                MySqlCommand deleteNoteCommand = new MySqlCommand(deleteNoteQuery, connection);
                deleteNoteCommand.Parameters.AddWithValue("@TrashID", noteId);
                deleteNoteCommand.Parameters.AddWithValue("@AccountID", accountId);

                int rowsDeleted = deleteNoteCommand.ExecuteNonQuery();

                return rowsDeleted > 0;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting note from trash: {ex.Message}");
        }
    }
    
    public ObservableCollection<Note> GetArchivedNotes(int accountId)
    {
        ObservableCollection<Note> archivedNotes = new ObservableCollection<Note>();

        try
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
                            NoteContent = reader.GetString("ArchiveContent"),
                            IsArchived = true // Set IsArchived to true for archived notes
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

        return archivedNotes;
    }
}

