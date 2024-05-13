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
    /// Interaction logic for CreateNoteWindow.xaml
    /// </summary>
    public partial class CreateNoteWindow : Window
    {
        public CreateNoteWindow()
        {
            InitializeComponent();

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string noteTitle = txtTitle.Text;
            string noteContent = txtContent.Text;
            string loggedInUsername = "username"; // Replace with actual logged-in username

            // Get the logged-in account ID using the DatabaseManager
            DatabaseManager dbManager = new DatabaseManager();
            int accountId = dbManager.GetLoggedInAccountId(loggedInUsername);

            // Insert the note into the database
            dbManager.InsertNote(accountId, noteTitle, noteContent);

            // Close the window
            this.Close();
        }
    }
}
