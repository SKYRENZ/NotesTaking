using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NotesTaking.MVVM.View
{
    public partial class NoteContainer : UserControl
    {
        public NoteContainer()
        {
            InitializeComponent();
           
        }

        private void NoteContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NotePopupWindow popupWindow = new NotePopupWindow
            {
                DataContext = this.DataContext // Pass the data context to the popup
            };
            popupWindow.ShowDialog();
        }

        private void NoteContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            NoteContainerBorder.Background = new SolidColorBrush(Colors.LightYellow);
        }

        private void NoteContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            NoteContainerBorder.Background = new SolidColorBrush(Colors.White);
        }
    }
}
