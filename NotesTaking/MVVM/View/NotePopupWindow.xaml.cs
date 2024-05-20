using MySql.Data.MySqlClient;
using NotesTaking.MVVM.Model;
using NotesTaking.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            // Add your delete button logic here if needed
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
                    // Instantiate DatabaseManager to access the connection string
                    DatabaseManager dbManager = new DatabaseManager();

                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(dbManager.ConnectionString))
                        {
                            connection.Open();

                            if (noteToSave.NotesID > 0)
                            {
                                // Prepare the SQL UPDATE query
                                string updateQuery = "UPDATE notes SET NoteTitle = @NoteTitle, NoteContent = @NoteContent WHERE NotesID = @NotesID";
                                MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                                updateCommand.Parameters.AddWithValue("@NoteTitle", noteToSave.NoteTitle);
                                updateCommand.Parameters.AddWithValue("@NoteContent", noteToSave.NoteContent);
                                updateCommand.Parameters.AddWithValue("@NotesID", noteToSave.NotesID);

                                // Execute the SQL UPDATE query
                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Note successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Failed to update note. Note not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid note ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating note: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            // Add your archive button logic here if needed
        }
    }
}
