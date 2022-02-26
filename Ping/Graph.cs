using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PingApp
{
    public class Graph
    {
        int range = 50;
        int min = 0;
        bool spike;

        List<Ellipse> points;
        List<Line> lines;

        int index = 0;
        int n = 1;
        int avg = 0;
        int sum = 0;

        public int Range {
            get { return range; }
            set { range = value; }
        }
        public bool Spike
        {
            get { return spike; }
            set { spike = value; }
        }
        public int Index
        {
            get
            {
                if(index == -1)
                {
                    index = points.Count - 1;
                }
                return index;
            }
            set
            {
                index = value;
            }
        }
        public int Sum
        {
            get
            {
                return sum;
            }
            set
            {
                sum = value;
            }
        }
        public int Average
        {
            get
            {
                return avg;
            }
            set
            {
                avg = value;
            }
        }

        private void Move()
        {
            //move points
            foreach (var point in points)
            {
                var ind = points.IndexOf(point);

                if (ind < points.Count - 1) point.Margin = new Thickness(point.Margin.Left, 0, 0, points[ind + 1].Margin.Bottom);

                else
                {
                    if (ind < lines.Count - 1)
                    {
                        lines[ind + 1].Stroke = new SolidColorBrush(Colors.LightGreen);
                    }
                }

            }

            //move lines
            foreach (var line in lines)
            {
                var ind = lines.IndexOf(line);
                if (ind < points.Count - 1)
                {
                    line.X1 = points[ind].Margin.Left;
                    line.Y1 = points[ind].Margin.Bottom;
                    line.X2 = points[ind + 1].Margin.Left;
                    line.Y2 = points[ind + 1].Margin.Bottom;

                    if (ind + 1 < lines.Count)
                    {
                        line.Stroke = lines[ind + 1].Stroke;
                    }
                }
            }

        }
        public void Update(int lastValue)
        {
            Sum = Sum + lastValue;
            Average = Sum / n;


            if(lastValue > range -15)
            {
                lastValue = range -15;
            }
            double newBottom = UI.GW.ActualHeight - (lastValue - min) * UI.GW.ActualHeight / range;
            points.Last().Margin = new Thickness(points.Last().Margin.Left, 0, 0, newBottom);
            UI.avgLine.Margin = new Thickness(0, 0, 0, UI.GW.ActualHeight * (Average - min) / Range);

            if (lastValue > avg * 1.2)
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



            Move();
            
            n++;
            index--;
        }

        public Graph()
        {
            points = new List<Ellipse>();
            lines = new List<Line>();

            //add points
            bool alt = true;
            for (int i = 0; i < UI.GW.Width; i++)
            {
                if (alt)
                {
                    var ell = new Ellipse
                    {
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(i, 0, 0, 0),
                        //Fill = new SolidColorBrush(Colors.White),
                        Height = 2,
                        Width = 2,
                    };

                    points.Add(ell);
                    UI.PingCanvas.Children.Add(ell);

                }
                alt = !alt;
            }
            foreach (var p in points)
            {
                p.Margin = new Thickness(p.Margin.Left, p.Margin.Top, p.Margin.Right, UI.GW.ActualHeight);
            }

            //add lines
            for (int i = 1; i < points.Count; i++)
            {
                Line l = new Line
                {
                    X1 = points[i - 1].Margin.Left,
                    Y1 = points[i - 1].Margin.Bottom,
                    X2 = points[i].Margin.Left,
                    Y2 = points[i].Margin.Bottom,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(new Color { A = 255, R = 120, G = 200, B = 255 })
                };
                lines.Add(l);
                UI.PingCanvas.Children.Add(l);
            }

            index = points.Count - 1;

        }
    }
}
