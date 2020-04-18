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

        }

        private OdeExplicitRungeKutta45 odeRK = new OdeExplicitRungeKutta45();
        double[] yprime = new double[3];
        double beta = 0.7;    //time between contacts ^-1
        double gamma = 0.5;   //time until recovery ^-1
        double lambda = 0.1;  //birth rate
        double mu = 0.1;     //death rate
        double xi = 0.75;     //vaccination rate
        int N = 100, infectedN = 1;
        Graph<Hooman> graph = new Graph<Hooman>(false, false);

        private double[,] Solve() {
            OdeFunction fun = new OdeFunction(ODEs);
            double[] initialConditions;
            double[,] sol;
            initialConditions = new double[3];
            initialConditions[0] = N - infectedN;
            initialConditions[1] = infectedN;
            initialConditions[2] = 0;
            odeRK.InitializeODEs(fun, 3);
            sol = odeRK.Solve(initialConditions, 0, 0.03, 20);

            return sol;
        }

        private double[] ODEs(double t, double[] y) {

            yprime[0] = -beta * y[1] * y[0] / (N - infectedN);
            yprime[1] = beta * y[1] * y[0] / (N - infectedN) - gamma * y[1];
            yprime[2] = gamma * y[1];

            return yprime;
        }

        private void MakeAPlot(double[,] data) {
            var plt = new ScottPlot.Plot(1000, 800);
            double[] x, y1, y2, y3, empX, empY1, empY2, empY3, empY4, empY5;
            int linewidth = 2, markersize = 0;

            x = GetColumn(data, 0);
            y1 = GetColumn(data, 1);
            y2 = GetColumn(data, 2);
            y3 = GetColumn(data, 3);

            empX = Hooman.graphdata[0];
            empY1 = Hooman.graphdata[1];
            empY2 = Hooman.graphdata[2];
            empY3 = Hooman.graphdata[3];
            empY4 = Hooman.graphdata[4];
            empY5 = Hooman.graphdata[5];
            //plt.PlotScatter(x, y1, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Green, label: "Susceptible");
            //plt.PlotScatter(x, y2, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "Infected");
            //plt.PlotScatter(x, y3, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Blue, label: "Recovered");

            plt.PlotScatter(empX, empY1, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Green, label: "Susceptible emp");
            plt.PlotScatter(empX, empY2, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "Infected emp");
            plt.PlotScatter(empX, empY3, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Blue, label: "Recovered emp");
            plt.PlotScatter(empX, empY4, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Gray, label: "Vaccinated emp");
            plt.PlotScatter(empX, empY5, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Black, label: "Deceased :C");


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

        private void NTb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {

        }

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

            graph.GenerateRandomEdges(N / 20);
            Hooman.society = graph;

            beta = Double.Parse(betaTb.Text);
            gamma = Double.Parse(gammaTb.Text);
             
            Hooman.SpreadDisease(beta, gamma, lambda, mu, xi, 50, N / 20);
            MakeAPlot(Solve());

        }
    }
}
