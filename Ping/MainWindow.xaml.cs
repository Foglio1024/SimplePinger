using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace PingApp
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Net.NetworkInformation.Ping p;
        System.Timers.Timer tm;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ping.Content = p.Send("172.17.145.193").RoundtripTime.ToString() + " ms";
            });
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            TCTNotifier.WindowsServices.SetWindowExTransparent(hwnd);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = SystemParameters.PrimaryScreenHeight - ActualHeight + 3;
            this.Left = SystemParameters.PrimaryScreenWidth - ActualWidth;
            p = new Ping();
            tm = new System.Timers.Timer(750);
            tm.Elapsed += Tm_Elapsed;
            tm.Enabled = true;
            tm.Start();
        }
    }
}