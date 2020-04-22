﻿using DotNumerics.ODE;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
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
        double beta = 0.7;    //time between contacts ^-1
        double gamma = 0.5;   //time until recovery ^-1
        double lambda = 0.0; //birth rate
        double mu = 0.0;     //death rate
        double xi = 0.90;     //vaccination rate
        int N = 100, infectedN = 1;
        Graph<Hooman> graph = new Graph<Hooman>(false, false);
        List<double>[] histogram;

        private double[,] Solve() {
            OdeFunction fun = new OdeFunction(ODEs);
            double[] initialConditions;
            double[,] sol;
            initialConditions = new double[3];
            initialConditions[0] = N - infectedN;
            initialConditions[1] = infectedN;
            initialConditions[2] = 0;
            //initialConditions[3] = 0;
            odeRK.InitializeODEs(fun, 3);
            sol = odeRK.Solve(initialConditions, 0, 0.03, Convert.ToDouble(timeTb.Text));

            return sol;
        }

        private double[] ODEs(double t, double[] y) {
            //SVIR
            /*yprime[0] = lambda - mu*y[0]/4 - beta*y[0]*y[2]/N - xi*y[0];
            yprime[1] = xi * y[0] - mu * y[1] / 4;
            yprime[2] = beta * y[0] * y[2] / N - gamma * y[2] - mu * y[2] / 4;
            yprime[3] = gamma * y[2] - mu*y[3] / 4;*/
            //SIR
            yprime[0] = -beta * y[1] * y[0] / N;
            yprime[1] = beta * y[1] * y[0] / N - gamma * y[1] ;
            yprime[2] = gamma * y[1] ;

            return yprime;
        }

        private void MakeAPlot(double[,] data) {
            var plt = new ScottPlot.Plot(1000, 800);
            double[] x, y1, y2, y3,y4, empX, empY1, empY2, empY3, empY4;
            int linewidth = 2, markersize = 0;
            double[] xx, yy;
            x = GetColumn(data, 0);
            y1 = GetColumn(data, 1);
            y2 = GetColumn(data, 2);
            y3 = GetColumn(data, 3);
           //y4 = GetColumn(data, 4);

            empX = Hooman.graphdata[0];
            empY1 = Hooman.graphdata[1];
            empY2 = Hooman.graphdata[2];
            empY3 = Hooman.graphdata[3];
            //empY4 = Hooman.graphdata[4];

            //histogram
            xx = histogram[1].ToArray();
            yy = histogram[0].ToArray();

            plt.PlotScatter(xx, yy, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Brown, label: "Histogram");

            int t = int.Parse(timeTb.Text) - 1;
            tbN.Text = Convert.ToString(empY1[t] + empY2[t] + empY3[t]);//+ empY4[t]);
            //empY5 = Hooman.graphdata[5];
            plt.PlotScatter(x, y1, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Green, label: "Susceptible", lineStyle: LineStyle.Dot);
            plt.PlotScatter(x, y2, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "Infected", lineStyle: LineStyle.Dot);
            plt.PlotScatter(x, y3, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Blue, label: "Recovered", lineStyle: LineStyle.Dot);
           // plt.PlotScatter(x, y4, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Gray, label: "Vaccinated", lineStyle: LineStyle.Dot);

            plt.PlotScatter(empX, empY1, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Green, label: "Susceptible emp");
            plt.PlotScatter(empX, empY2, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "Infected emp");
            plt.PlotScatter(empX, empY3, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Blue, label: "Recovered emp");
           // plt.PlotScatter(empX, empY4, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Gray, label: "Vaccinated emp");

            plt.PlotAnnotation("Population: " + N + "\nβ = " + beta + "\nγ = " + gamma, 10, 10);
            plt.Legend();
            plt.XLabel("Time");
            plt.YLabel("Population");

            image.Source = CreateBitmapSourceFromGdiBitmap(plt.GetBitmap());
        }

        public double[] GetColumn(double[,] matrix, int columnNumber) {     // bierze kolumne z tablicy [,] 

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

		#region Zabezpieczenia

		private void NTb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void infectedNTb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void betaTb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void gammaTb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void lambdaTb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void muTb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void xiTb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void timeTb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e) {
            graph.Nodes.Clear();
            N = int.Parse(NTb.Text);
            infectedN = int.Parse(infectedNTb.Text);

            for (int i = 0; i < N - infectedN; i++) {   //zdrowe ludzie
                graph.AddNode(new Hooman());
            }
            for (int i = 0; i < infectedN; i++) {   //chore ludzie
                graph.AddNode(new Hooman(false, true, false));
            }
            // ten argument w GenerateRandomEdges teraz nic nie robi bo zakomentowany jest Twoj fragment kodu
            histogram = graph.GenerateRandomEdges(N / 20);
            Hooman.society = graph;

            beta = Double.Parse(betaTb.Text);
            gamma = Double.Parse(gammaTb.Text);
            lambda = Double.Parse(lambdaTb.Text);
            mu = Double.Parse(muTb.Text);
            xi = Double.Parse(xiTb.Text);
           
            Hooman.SpreadDisease(beta, gamma, lambda, mu, xi, int.Parse(timeTb.Text), N / 10);
            MakeAPlot(Solve());

        }
    }
}
