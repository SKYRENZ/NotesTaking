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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ArhiveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
