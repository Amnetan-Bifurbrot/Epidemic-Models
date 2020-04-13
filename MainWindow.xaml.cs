using DotNumerics.ODE;
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

            MakeAPlot(Solve());
        }

        private OdeExplicitRungeKutta45 odeRK = new OdeExplicitRungeKutta45();
        double[] yprime = new double[4];
        double beta = 10;    //time between contacts ^-1
        double gamma = 0.1;   //time until recovery ^-1
        double lambda = 0.01;  //birth rate
        double mu = 0.01;      //death rate
        double a = 3;       //incubation period
        int N = 100;

        private double[,] Solve() {
            OdeFunction fun = new OdeFunction(ODEs);
            double[] initialConditions = new double[4];
            initialConditions[0] = N;
            initialConditions[1] = 0;
            initialConditions[2] = 1;
            initialConditions[3] = 0;
            odeRK.InitializeODEs(fun, 4);
            double[,] sol = odeRK.Solve(initialConditions, 0, 0.003, 50);


            /* for (int i = 0; i < sol.GetLength(0); i++) {
                 for (int j = 0; j < sol.GetLength(1); j++) {
                     Console.Write(sol[i, j] + "\t");
                 }
                 Console.WriteLine();
             }*/

            //Console.WriteLine("solLength0:" + sol.GetLength(0));
            //Console.WriteLine("solLength:" + sol.GetLength(1));
            return sol;
        }

        private double[] ODEs(double t,double[] y) {
            yprime[0] = lambda - mu *  y[0] - beta * y[2] * y[0] / N;
            yprime[1] = beta * y[2] * y[0] / N - (mu + a) * y[1];
            yprime[2] = a * y[1] - (gamma + mu) * y[2];
            yprime[3] = gamma * y[2] - mu * y[3];
            return yprime;
        }

        private void MakeAPlot(double[,] data) {
            var plt = new ScottPlot.Plot(1000, 800);
            double[] x, y1, y2, y3, y4;
            int linewidth = 2, markersize = 0;

            double[] t = GetColumn(data, 0);
            Console.WriteLine("t0:" + t[0]);
            x = GetColumn(data, 0);
            y1 = GetColumn(data, 1);
            y2 = GetColumn(data, 2);
            y3 = GetColumn(data, 3);
            y4 = GetColumn(data, 4);

            plt.PlotScatter(x, y1, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Green, label: "Healthy");
            plt.PlotScatter(x, y2, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Orange, label: "Exposed");
            plt.PlotScatter(x, y3, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "Infected");
            plt.PlotScatter(x, y4, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Blue, label: "Recovered");
            plt.PlotAnnotation("Population: " + N + "\nβ = " + beta + "\nγ = " + gamma + "\nΛ = " + lambda + "\nμ = " + mu + "\na = " + a, 10, 10);
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

    }
}
