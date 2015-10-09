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
using System.Net;
using System.IO;
using System.Net.Sockets;

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

            int iterationCount = 0;

            bool hasInternetConnection = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            this.ipText.Text = (Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToString();

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                iterationCount++;
                this.timeText.Text = DateTime.Now.ToString("HH:mm:ss");
                this.weekText.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("dddd"));
                this.dateText.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.dateText.ToolTip = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMMM"));

                hasInternetConnection = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

                this.ipText_local.Text = (Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToString();
                this.hasInternet.Text = (hasInternetConnection) ? "Has internet connection" : "No internet connection";

                if (!hasInternetConnection) {
                    this.hasInternet.Foreground = (Brush)new BrushConverter().ConvertFromString("#ffc42e3b");
                    this.ipText.Text = this.ipText_local.Text;
                } else {
                    this.hasInternet.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff90ad2f");
                    if (this.ipText.Text == "127.0.0.1" || iterationCount == 5) {
                        this.WindowTitle.Content = "Checking IP address...";
                        this.ipText.Text = GetPublicIP().ToString();
                    }
                }
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

        public string GetPublicIP()
        {
            try {
                String direction = "";
                WebRequest request = WebRequest.Create("http://matheus.avellar.c9.io/ip");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream())) {
                    direction = stream.ReadToEnd();
                }
                this.WindowTitle.Content = "Manager Thing";
                return direction;
            } catch {
                this.WindowTitle.Content = "Manager Thing";
                return "127.0.0.1";
            }
        }

        private void IpRefresh(object sender, MouseButtonEventArgs e)
        {
            this.WindowTitle.Content = "Checking IP address...";
            this.ipText.Text = (Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToString();
            this.ipText.Text = GetPublicIP().ToString();
        }
    }
}
