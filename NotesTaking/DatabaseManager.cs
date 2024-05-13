using MySql.Data.MySqlClient;
using System;

namespace NotesTaking
{
    public class DatabaseManager
    {
        private string connectionString;

        public DatabaseManager()
        {
            // Set your default connection string here or pass it as a parameter to the constructor
            connectionString = "server=localhost;user=root;database=notetaking_test;port=3306";
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM account WHERE AccountUser = @username AND AccountPass = @password";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
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
                using (MySqlConnection connection = new MySqlConnection(connectionString))
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
                        // User not found
                        return -1; // Or handle this case accordingly
                    }
                }
            }
            catch (Exception)
            {
                // Log or handle the exception appropriately
                return -1; // Return a default or error value
            }
        }

        public bool InsertNote(int accountId, string noteTitle, string noteContent)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
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
    }
}