using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;

namespace PingApp
{

    public static class UI
    {
        public static Window GW;
        public static Canvas PingCanvas;
        public static Rectangle avgLine;
    }

    /// <summary>
    /// Logica di interazione per GraphWindow.xaml
    /// </summary>
    /// 
    public partial class GraphWindow : Window
    {
        const int UPDATE_FREQ = 10;
        System.Net.NetworkInformation.Ping _ping;
        Graph ping;
        System.Timers.Timer tm;

        long lastPing;

        private void Tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            lastPing = _ping.Send("google.com").RoundtripTime;

            Dispatcher.Invoke(() =>
            {

                ping.Update((int)lastPing);
                pl.Text = String.Format("{0} ({1}) ms", lastPing.ToString(), ping.Average);

                //if (FocusMonitor.IsTeraActive() || disableAutoHide)
                //{
                //    this.ShowActivated = false;
                //    this.Show();
                //}
                //else
                //{
                //    this.Hide();
                //}
            });
        }

        bool disableAutoHide = true;

        NotifyIcon notifyIcon;

        public GraphWindow()
        {
            InitializeComponent();

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
            notifyIcon.Icon = new System.Drawing.Icon(Environment.CurrentDirectory + "\\network-icon-1877-16x16.ico");

            this.Top = Properties.Settings.Default.Y;
            this.Left = Properties.Settings.Default.X;

            MouseDown += Window_MouseDown;
            MouseUp += Window_MouseUp;

        }








        bool transparent = true;

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                disableAutoHide = false;
            }

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
            {
                //disableAutoHide = true;
                DragMove();
            }    
        }
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;

            Properties.Settings.Default.Y = Top;
            Properties.Settings.Default.X = Left;

            Properties.Settings.Default.Save();

            Environment.Exit(0);
        }
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            //if (transparent)
            //{
            //    TCTNotifier.WindowsServices.UnsetWindowExTransparent(hwnd);
            //    transparent = false;
            //}
            //else
            //{
            //    TCTNotifier.WindowsServices.SetWindowExTransparent(hwnd);
            //    transparent = true;
            //}


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UI.GW = this;
            UI.PingCanvas = c;
            UI.avgLine = avgLine;
            _ping = new Ping();
            ping = new Graph();

            tm = new System.Timers.Timer(UPDATE_FREQ);
            tm.Elapsed += Tm_Elapsed;
            tm.Enabled = true;



            Topmost = true;
            notifyIcon.Visible = true;

            tm.Start();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //var hwnd = new WindowInteropHelper(this).Handle;
            //TCTNotifier.WindowsServices.SetWindowExTransparent(hwnd);
        }

    }
}
