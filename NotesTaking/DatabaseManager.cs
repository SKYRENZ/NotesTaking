using MySql.Data.MySqlClient;

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
            catch (Exception )
            {
                // Log or handle the exception appropriately
                return false;
            }
        }
    }
}
