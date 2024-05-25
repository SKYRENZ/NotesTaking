using System;
using System.Windows;
using System.Windows.Controls;
using NotesTaking.MVVM.ViewModel;

namespace NotesTaking.MVVM.View
{
    public partial class TrashControl : UserControl
    {
        public TrashViewModel ViewModel { get; set; }
        private DatabaseManager dbManager;

        public TrashControl()
        {
            InitializeComponent();
            ViewModel = new TrashViewModel();
            DataContext = ViewModel;

            dbManager = new DatabaseManager();

            try
            {
                int accountId = dbManager.GetLoggedInAccountId(UserSession.LoggedInUsername);
                if (accountId != -1)
                {
                    ViewModel.LoadTrashedNotes(accountId, dbManager);
                }
                else
                {
                    MessageBox.Show("Error: Unable to find account for logged-in user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
