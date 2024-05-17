﻿using NotesTaking.MVVM.Model;
using NotesTaking.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace NotesTaking.MVVM.View
{
    public partial class CreateNoteWindow : Window
    {
        public Note NewNote { get; private set; }

        public CreateNoteWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string noteTitle = txtTitle.Text;
            string noteContent = txtContent.Text;
            string loggedInUsername = UserSession.LoggedInUsername; // Retrieve the logged-in username

            if (string.IsNullOrEmpty(loggedInUsername))
            {
                Console.WriteLine("Error: No user is currently logged in.");
                return;
            }

            // Get the logged-in account ID using the DatabaseManager
            DatabaseManager dbManager = new DatabaseManager();
            int accountId = dbManager.GetLoggedInAccountId(loggedInUsername);

            if (accountId == -1)
            {
                Console.WriteLine("Error: Unable to find account for logged-in user.");
                return;
            }

            // Insert the note into the database
            bool isInserted = dbManager.InsertNote(accountId, noteTitle, noteContent);

            if (isInserted)
            {
                Console.WriteLine("Note added successfully.");
                NewNote = new Note
                {
                    NoteTitle = noteTitle,
                    NoteContent = noteContent
                };
                DialogResult = true; // Indicate the note was successfully created
            }
            else
            {
                Console.WriteLine("Error: Unable to add the note.");
                DialogResult = false; // Indicate the note creation failed
            }

            // Close the window
            this.Close();
        }

        private void txtTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtTitle.Text == "Note Title:")
            {
                txtTitle.Text = string.Empty;
                txtTitle.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                txtTitle.Text = "Note Title:";
                txtTitle.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void txtContent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtContent.Text == "Note...")
            {
                txtContent.Text = string.Empty;
                txtContent.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtContent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                txtContent.Text = "Note...";
                txtContent.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
