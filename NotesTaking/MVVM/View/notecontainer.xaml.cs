using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotesTaking.MVVM.View
{
    public partial class notecontainer : UserControl
    {
        public notecontainer()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += Notecontainer_MouseLeftButtonDown;
        }

        private void Notecontainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Toggle the visibility of the note content
            if (NoteContentTextBlock.Visibility == Visibility.Collapsed)
                NoteContentTextBlock.Visibility = Visibility.Visible;
            else
                NoteContentTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}
