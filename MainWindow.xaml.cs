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
        double[] yprime = new double[3];


        private double[,] Solve() {
            OdeFunction fun = new OdeFunction(ODEs);
            double[] y0 = new double[3];
            y0[0] = 0;
            y0[1] = 1;
            y0[2] = 1;
            odeRK.InitializeODEs(fun, 3);
            double[,] sol = odeRK.Solve(y0, 0, 0.003, 15);


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

        private double[] ODEs(double t, double[] y) {
            double sigma = 10, r = 99.96, b = 10 / 3;
            // tutaj som rownania do attractora lorentza
            yprime[0] = sigma * (y[1] - y[0]);
            yprime[1] = -y[0] * y[2] + r * y[0] - y[1];
            yprime[2] = y[0] * y[1] - b * y[2];
            return yprime;
        }

        private void MakeAPlot(double[,] data) {
            var plt = new ScottPlot.Plot(1000, 800);
            double[] x, y;
            int linewidth = 2, markersize = 0;

            double[] t = GetColumn(data, 0);
            Console.WriteLine("t0:" + t[0]);
            x = GetColumn(data, 1);
            y = GetColumn(data, 3);
            plt.Title("Jakis fajny tutulik :)");

            plt.PlotScatter(x, y, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "jakas fajna legenda");
            plt.Legend();


            plt.YLabel("y");
            plt.XLabel("x");

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
