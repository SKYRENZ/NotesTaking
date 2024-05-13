using NotesTaking.MVVM.View;
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

namespace NotesTaking
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {

        public CornerRadius CornerRadius { get; set; }
        private SolidColorBrush? originalFill, originalStroke, originalFillMinimize, originalStrokeMinimize;

        public Dashboard()
        {
            InitializeComponent();
            //Revert Color of Close Button
            originalFill = btnClose.Fill as SolidColorBrush;
            originalStroke = btnClose.Stroke as SolidColorBrush;

            //Revert Color of Minimize Button
            originalFillMinimize = btnMinimize.Fill as SolidColorBrush;
            originalStrokeMinimize = btnMinimize.Stroke as SolidColorBrush;
        }

       

        private void btnMinimize_MouseEnter(object sender, MouseEventArgs e)
        {
            //Changes the color of minimize button when hovered
            btnMinimize.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C79436"));
            btnMinimize.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBE45"));
        }

        private void btnMinimize_MouseLeave(object sender, MouseEventArgs e)
        {
            //reverts color to original minimize button
            btnMinimize.Fill = originalFillMinimize;
            btnMinimize.Stroke = originalStrokeMinimize;
        }

        private void btnMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            //Changes the color of close button when hovered
            btnClose.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C7282D"));
            btnClose.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF343A"));
        }

        private void btnNotes_Click(object sender, RoutedEventArgs e)
        {
            // Load the Notes control into the content area
            contentArea.Content = new NotesControl();
        }

        private void btnArchive_Click(object sender, RoutedEventArgs e)
        {
            contentArea.Content = new ArchiveControl();
        }

        private void btnReminders_Click(object sender, RoutedEventArgs e)
        {
            contentArea.Content = new RemindersControl();
        }

        private void btnTrash_Click(object sender, RoutedEventArgs e)
        {
            contentArea.Content = new TrashControl();
        }

        private void Dashboard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            //reverts color to original Close button
            btnClose.Fill = originalFill;
            btnClose.Stroke = originalStroke;
        }

        private void btnClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //When Clicked
            Close();
        }


      
    }
}
