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
    }
}

