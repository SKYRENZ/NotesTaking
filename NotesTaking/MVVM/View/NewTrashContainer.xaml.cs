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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotesTaking.MVVM.View
{
    /// <summary>
    /// Interaction logic for NewTrashContainer.xaml
    /// </summary>
    public partial class NewTrashContainer : UserControl
    {
        public NewTrashContainer()
        {
            InitializeComponent();
        }

        private void TrashContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TrashPopupWindow trashPopup = new TrashPopupWindow();
            trashPopup.ShowDialog();
        }
    }
}
