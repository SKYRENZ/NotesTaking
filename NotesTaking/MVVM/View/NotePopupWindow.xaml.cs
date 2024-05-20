using MySql.Data.MySqlClient;
using NotesTaking.MVVM.Model;
using NotesTaking.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NotesTaking.MVVM.View
{
    /// <summary>
    /// Interaction logic for NotePopupWindow.xaml
    /// </summary>
    public partial class NotePopupWindow : Window
    {
        public NotePopupWindow()
        {
            InitializeComponent();

        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

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
                    // Get the logged-in user's account ID
                    string loggedInUsername = UserSession.LoggedInUsername;
                    DatabaseManager dbManager = new DatabaseManager();
                    int accountId = dbManager.GetLoggedInAccountId(loggedInUsername);

                    if (accountId != -1)
                    {
                        try
                        {
                            using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                            {
                                connection.Open();

                                // Prepare the SQL SELECT query to get the NotesID
                                string selectQuery = "SELECT NotesID FROM notes WHERE AccountID = @AccountID";
                                MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
                                selectCommand.Parameters.AddWithValue("@AccountID", accountId);

                                // Execute the SQL SELECT query
                                int notesId = (int)selectCommand.ExecuteScalar();

                                if (notesId > 0)
                                {
                                    // Prepare the SQL UPDATE query
                                    string updateQuery = "UPDATE notes SET NoteTitle = @NoteTitle, NoteContent = @NoteContent WHERE NotesID = @NotesID";
                                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                                    updateCommand.Parameters.AddWithValue("@NoteTitle", noteToSave.NoteTitle);
                                    updateCommand.Parameters.AddWithValue("@NoteContent", noteToSave.NoteContent);
                                    updateCommand.Parameters.AddWithValue("@NotesID", notesId);

                                    // Execute the SQL UPDATE query
                                    int rowsAffected = updateCommand.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        MessageBox.Show("Note successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Failed to update note.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("No note found for the account.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating note: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        }
    }
}
