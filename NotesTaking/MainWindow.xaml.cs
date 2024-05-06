using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotesTaking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SolidColorBrush? originalFill, originalStroke, originalFillMinimize, originalStrokeMinimize;

        public MainWindow()
        {

            InitializeComponent();
            originalFill = btnClose.Fill as SolidColorBrush;
            originalStroke = btnClose.Stroke as SolidColorBrush;
            originalFillMinimize = btnMinimize.Fill as SolidColorBrush;
            originalStrokeMinimize = btnMinimize.Stroke as SolidColorBrush;
        }






        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            btnClose.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C7282D"));
            btnClose.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF343A"));
        }

        private void btnMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMinimize_MouseEnter(object sender, MouseEventArgs e)
        {
            btnMinimize.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C79436"));
            btnMinimize.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBE45"));
        }

        private void btnMinimize_MouseLeave(object sender, MouseEventArgs e)
        {
            btnMinimize.Fill = originalFillMinimize;
            btnMinimize.Stroke = originalStrokeMinimize;
        }

        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClose.Fill = originalFill;
            btnClose.Stroke = originalStroke;
        }
    }
}