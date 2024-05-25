using MySql.Data.MySqlClient;
using NotesTaking.MVVM.Model;
using System;
using System.Collections.ObjectModel;

namespace NotesTaking
{
    public class DatabaseManager
    {
        public string ConnectionString { get; private set; }

        public DatabaseManager()
        {
            // Set your default connection string here or pass it as a parameter to the constructor
            ConnectionString = "server=localhost;user=root;database=notetaking_test;port=3306";
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Ensure that the query respects case sensitivity by default
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
                // Log or handle the exception appropriately
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
            catch (Exception)
            {
                // Log or handle the exception appropriately
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
                        // Log: User not found
                        Console.WriteLine($"User {loggedInUsername} not found.");
                        return -1; // User not found
                    }
                }
            }
            catch (Exception ex)
            {
                // Log: Exception
                Console.WriteLine($"Exception: {ex.Message}");
                return -1; // Return a default or error value
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

            return notes;
        }

        public bool RestoreNoteFromTrash(int accountId, int noteId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Begin a transaction
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        // Retrieve the note data from the trash table
                        string retrieveQuery = "SELECT TrashTitle, TrashContent FROM trash WHERE TrashID = @TrashID";
                        MySqlCommand retrieveCommand = new MySqlCommand(retrieveQuery, connection, transaction);
                        retrieveCommand.Parameters.AddWithValue("@TrashID", noteId);

                        string noteTitle = "";
                        string noteContent = "";

                        using (MySqlDataReader reader = retrieveCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                noteTitle = reader.GetString("TrashTitle");
                                noteContent = reader.GetString("TrashContent");
                            }
                            else
                            {
                                // Note not found in trash
                                transaction.Rollback();
                                return false;
                            }
                        }

                        // Insert the note data into the notes table
                        string insertQuery = "INSERT INTO notes (AccountID, NoteTitle, NoteContent) VALUES (@accountId, @noteTitle, @noteContent)";
                        MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection, transaction);
                        insertCommand.Parameters.AddWithValue("@accountId", accountId);
                        insertCommand.Parameters.AddWithValue("@noteTitle", noteTitle);
                        insertCommand.Parameters.AddWithValue("@noteContent", noteContent);

                        int rowsInserted = insertCommand.ExecuteNonQuery();

                        if (rowsInserted > 0)
                        {
                            // Delete the note data from the trash table
                            string deleteQuery = "DELETE FROM trash WHERE TrashID = @TrashID";
                            MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection, transaction);
                            deleteCommand.Parameters.AddWithValue("@TrashID", noteId);
                            int rowsDeleted = deleteCommand.ExecuteNonQuery();

                            if (rowsDeleted > 0)
                            {
                                // Commit the transaction if both operations succeed
                                transaction.Commit();
                                return true;
                            }
                        }

                        // Rollback the transaction if any operation fails
                        transaction.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                // Add additional error handling/logging as needed
            }

            return false;
        }

        public bool DeleteNoteFromTrash(int accountId, int noteId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM trash WHERE AccountID = @AccountId AND TrashID = @NoteId";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    command.Parameters.AddWithValue("@NoteId", noteId);

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
    }
}
