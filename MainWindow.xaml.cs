﻿using DotNumerics.ODE;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Epidemic_Models {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            
        }

        private OdeExplicitRungeKutta45 odeRK = new OdeExplicitRungeKutta45();
        double[] yprime = new double[3];
        double beta = 10;    //time between contacts ^-1
        double gamma = 0.1;   //time until recovery ^-1
        //double lambda = 0.01;  //birth rate
        //double mu = 0.01;      //death rate
        //double a = 3;       //incubation period
        int N = 100;
        Graph<int> graph = new Graph<int>(false, false);
        Node<int>[] nodes;

        private double[,] Solve() {
            OdeFunction fun = new OdeFunction(ODEs);
            double[] initialConditions;
            double[,] sol;
            // S + I + R = N !!!!!!!!!!!!!!!
            initialConditions = new double[3];
            initialConditions[0] = N - 1;
            initialConditions[1] = 1;
            initialConditions[2] = 0;
            odeRK.InitializeODEs(fun, 3);
            sol = odeRK.Solve(initialConditions, 0, 0.03, 10);

            return sol;
        }

        private double[] ODEs(double t, double[] y) {

            yprime[0] = -beta * y[1] * y[0] / N;
            yprime[1] = beta * y[1] * y[0] / N - gamma * y[1];
            yprime[2] = gamma * y[1];

            return yprime;
        }

        private void MakeAPlot(double[,] data) {
            var plt = new ScottPlot.Plot(1000, 800);
            double[] x, y1, y2, y3;
            int linewidth = 2, markersize = 0;

            x = GetColumn(data, 0);

            y1 = GetColumn(data, 1);
            y2 = GetColumn(data, 2);
            y3 = GetColumn(data, 3);
            plt.PlotScatter(x, y1, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Green, label: "Susceptible");
            plt.PlotScatter(x, y2, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "Infected");
            plt.PlotScatter(x, y3, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Blue, label: "Recovered");

            plt.PlotAnnotation("Population: " + N + "\nβ = " + beta + "\nγ = " + gamma, 10, 10);
            plt.Legend();
            plt.XLabel("Time");
            plt.YLabel("Population");

            image.Source = CreateBitmapSourceFromGdiBitmap(plt.GetBitmap());
        }

        public double[] GetColumn(double[,] matrix, int columnNumber) {     // bierze kolumne z tablicy [,] - uzyte w linijce 63 i 64

            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap) {     // zeby z wykresu plt (bitmapa) zrobic sourca do imagea
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            } finally {
                bitmap.UnlockBits(bitmapData);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MakeAPlot(Solve());

            CreateRandomGraph();

        }
    }
}
