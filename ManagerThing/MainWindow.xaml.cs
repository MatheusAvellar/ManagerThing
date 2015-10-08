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
using System.Windows.Threading;

namespace ManagerThing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                this.timeText.Text = DateTime.Now.ToString("HH:mm:ss");
                this.weekText.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("dddd"));
                this.dateText.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.dateText.ToolTip = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMMM"));
            }, this.Dispatcher);
        }

        private void gridOnMouseEnter(object sender, MouseEventArgs e)
        {
            weekText.Foreground = dateText.Foreground = (Brush)new BrushConverter().ConvertFromString("#ffeeeeee");
        }

        private void gridOnMouseLeave(object sender, MouseEventArgs e)
        {
            weekText.Foreground = dateText.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff444a59");
        }



        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CloseWindowEnter(object sender, MouseEventArgs e)
        {
            closeWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#ffeeeeee");
        }

        private void CloseWindowLeave(object sender, MouseEventArgs e)
        {
            closeWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff1c1f25");
        }
    }
}
