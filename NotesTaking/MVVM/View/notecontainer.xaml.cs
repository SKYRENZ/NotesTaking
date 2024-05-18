using System;
using System.Windows;
using System.Windows.Controls;

namespace NotesTaking.MVVM.View
{
    public partial class NoteContainer : UserControl
    {
        public NoteContainer()
        {
            InitializeComponent();
            Loaded += NoteContainer_Loaded;
        }

        private void NoteContainer_Loaded(object sender, RoutedEventArgs e)
        {
        
        }
    }
}
