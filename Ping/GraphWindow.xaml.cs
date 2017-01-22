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

namespace PingApp
{

    public static class UI
    {
        public static Window GW;
    }

    /// <summary>
    /// Logica di interazione per GraphWindow.xaml
    /// </summary>
    /// 
    public partial class GraphWindow : Window
    {
        const int UPDATE_FREQ = 1000;
        const int RANGE = 500;
        bool spike;
        System.Net.NetworkInformation.Ping _ping;
        System.Timers.Timer tm;
        long lastPing;
        List<Ellipse> points;
        List<Line> lines;
        int index = 0;
        int n =1;
        int avg = 0;
        int sum = 0;

        private void Tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            lastPing = _ping.Send("172.17.145.193").RoundtripTime;

            Dispatcher.Invoke(() =>
            {
                if (index == -1)
                {
                    index = points.Count - 1;
                }

                sum = sum + (int)lastPing;
                avg = sum / n;

                pl.Text = String.Format("{0} ({1})", lastPing.ToString(), avg);
                avgLine.Margin = new Thickness(0, ActualHeight - ActualHeight * avg / RANGE, 0, 0);

                points.Last().Margin = new Thickness(points.Last().Margin.Left, ActualHeight - lastPing * ActualHeight / RANGE, 0, 0);
                if (lastPing > avg * 1.2)
                {
                    lines.Last().Stroke = new SolidColorBrush(Colors.Red);
                    spike = true;
                }
                else
                {
                    if (spike)
                    {
                        spike = false;
                    }
                    else
                    {
                        lines.Last().Stroke = new SolidColorBrush(Colors.LightGreen);
                    }


                }
                foreach (var item in points)
                {
                    var ind = points.IndexOf(item);

                    if (ind < points.Count - 1) item.Margin = new Thickness(item.Margin.Left, points[ind + 1].Margin.Top, 0, 0);

                    else
                    {
                        if (ind < lines.Count - 1)
                        {
                            lines[ind + 1].Stroke = new SolidColorBrush(Colors.LightGreen);
                        }
                    }


                }


                foreach (var item in lines)
                {
                    var ind = lines.IndexOf(item);
                    if (ind < points.Count - 1)
                    {
                        item.X1 = points[ind].Margin.Left;
                        item.Y1 = points[ind].Margin.Top;
                        item.X2 = points[ind + 1].Margin.Left;
                        item.Y2 = points[ind + 1].Margin.Top;

                        if (ind + 1 < lines.Count)
                        {
                            item.Stroke = lines[ind + 1].Stroke;
                        }
                    }
                }


                index--;
                n++;

                if (FocusMonitor.IsTeraActive())
                {
                    this.ShowActivated = false;
                    this.Show();
                }
                else
                {
                    this.Hide();
                }
            });
        }

        NotifyIcon notifyIcon;

        public GraphWindow()
        {
            InitializeComponent();

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
            notifyIcon.Icon = new System.Drawing.Icon(Environment.CurrentDirectory + "\\network-icon-1877-16x16.ico");
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Environment.Exit(0);
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (this.IsVisible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UI.GW = this;
            _ping = new Ping();
            tm = new System.Timers.Timer(UPDATE_FREQ);
            tm.Elapsed += Tm_Elapsed;
            tm.Enabled = true;
            points = new List<Ellipse>();
            lines = new List<Line>();

            for (int i = 0; i < this.Width; i++)
            {
                var ell = new Ellipse
                {
                    Margin = new Thickness(i, ActualHeight+2, 0, 0),
                    //Fill = new SolidColorBrush(Colors.White),
                    Height = 2,
                    Width = 2,
                    VerticalAlignment = VerticalAlignment.Top,
                };

                points.Add(ell);
                c.Children.Add(ell);

                if (i != 0)
                {
                    Line l = new Line
                    {
                        X1 = points[i - 1].Margin.Left,
                        Y1 = points[i - 1].Margin.Top,
                        X2 = points[i].Margin.Left,
                        Y2 = points[i].Margin.Top,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(new Color { A=255, R = 120, G = 200, B = 255 })
                    };
                    lines.Add(l);
                    c.Children.Add(l);

                }


            }
            Top = 0;
            Left = SystemParameters.PrimaryScreenWidth - Width;
            Topmost = true;
            index = points.Count - 1;
            notifyIcon.Visible = true;

            tm.Start();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            TCTNotifier.WindowsServices.SetWindowExTransparent(hwnd);
        }

    }
}
