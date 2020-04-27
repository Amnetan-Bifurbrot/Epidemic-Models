using DotNumerics.ODE;
using ScottPlot;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        double[] yprime = new double[4];
        double beta = 0.7;    //time between contacts ^-1
        double gamma = 0.5;   //time until recovery ^-1
        double lambda = 0.0; //birth rate
        double mu = 0.0;     //death rate
        double xi = 0.90;     //vaccination rate
        int N = 100, infectedN = 1, T = 0;
        Graph<Hooman> graph = new Graph<Hooman>(false, false);
        private BackgroundWorker calcWorker = null;
        Popup pop = new Popup();
        double[,] data;

        private double[,] Solve() {
            OdeFunction fun = new OdeFunction(ODEs);
            double[] initialConditions;
            double[,] sol;
            initialConditions = new double[4];
            initialConditions[0] = N - infectedN;
            initialConditions[1] = infectedN;
            initialConditions[2] = 0;
            initialConditions[3] = 0;
            odeRK.InitializeODEs(fun, 4);
            sol = odeRK.Solve(initialConditions, 0, 0.03, double.Parse(timeTb.Text));

            return sol;
        }

        private double[] ODEs(double t, double[] y) {

            //yprime[0] = lambda - beta * y[0] * y[1] / N - mu * y[0];          //S
            //yprime[1] = beta * y[0] * y[1] / N - gamma * y[1] - mu * y[1];    //I    
            //yprime[2] = gamma * y[1] - mu * y[2];                             //R


            yprime[0] = lambda - beta * y[0] * y[1] / N - (mu + xi) * y[0];     //S
            yprime[1] = beta * y[0] * y[1] / N - gamma * y[1] - mu * y[1];      //I
            yprime[2] = xi * y[0] - mu * y[2];                                  //V
            yprime[3] = gamma * y[1] - mu * y[3];                               //R
            return yprime;
        }

        private void MakeAPlot(double[,] data, double[,] dataEmp) {
            var plt = new ScottPlot.Plot(1000, 800);
            double[] x, y1, y2, y3, y4, empX, empY1, empY2, empY3, empY4;
            int linewidth = 2, markersize = 0;

            x = GetColumn(data, 0);
            y1 = GetColumn(data, 1);
            y2 = GetColumn(data, 2);
            y3 = GetColumn(data, 3);
            y4 = GetColumn(data, 4);

            empX = GetRow(dataEmp, 0);
            empY1 = GetRow(dataEmp, 1);
            empY2 = GetRow(dataEmp, 2);
            empY3 = GetRow(dataEmp, 3);
            empY4 = GetRow(dataEmp, 4);

            plt.PlotScatter(x, y1, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Green, label: "Susceptible", lineStyle: LineStyle.Dot);
            plt.PlotScatter(x, y2, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "Infected", lineStyle: LineStyle.Dot);
            plt.PlotScatter(x, y3, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Gray, label: "Vaccinated", lineStyle: LineStyle.Dot);
            plt.PlotScatter(x, y4, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Blue, label: "Recovered", lineStyle: LineStyle.Dot);

            plt.PlotScatter(empX, empY1, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Green, label: "Susceptible emp");
            plt.PlotScatter(empX, empY2, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "Infected emp");
            plt.PlotScatter(empX, empY3, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Gray, label: "Vaccinated emp");
            plt.PlotScatter(empX, empY4, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Blue, label: "Recovered emp");


            //plt.PlotAnnotation("Population: " + N + "\nβ = " + beta + "\nγ = " + gamma, 10, 10);
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
        public double[] GetRow(double[,] matrix, int rowNumber) {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
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

        public static Bitmap ControlToBmp(Visual target, double dpiX, double dpiY) {
            if (target == null) {
                return null;
            }
            // render control content
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                                                            (int)(bounds.Height * dpiY / 96.0),
                                                            dpiX,
                                                            dpiY,
                                                            PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen()) {
                VisualBrush vb = new VisualBrush(target);
                ctx.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            rtb.Render(dv);
             
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);
            return new Bitmap(stream);
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            pop.Close();
            Application.Current.Shutdown();
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

        private void SaveGraph_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "graph";
            dialog.DefaultExt = ".png";
            dialog.Filter = "Image files (*.png) | *.png";

            // Show save file dialog box
            Nullable<bool> result = dialog.ShowDialog();

            // Process save file dialog box results
            if (result == true) {
                string filename = dialog.FileName;
                ControlToBmp(image, 96, 96).Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            } else {
                MessageBox.Show("File Save Error.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

            beta = Double.Parse(betaTb.Text);
            gamma = Double.Parse(gammaTb.Text);
            lambda = Double.Parse(lambdaTb.Text);
            mu = Double.Parse(muTb.Text);
            xi = Double.Parse(xiTb.Text);

            data = new double[5, int.Parse(timeTb.Text)];

            N = int.Parse(NTb.Text);
            infectedN = int.Parse(infectedNTb.Text);
            T = int.Parse(timeTb.Text);

            if (null == calcWorker) {
                calcWorker = new BackgroundWorker();
                calcWorker.DoWork += new DoWorkEventHandler(calcWorker_DoWork);
                calcWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(calcWorker_RunWorkerCompleted);
                calcWorker.ProgressChanged += new ProgressChangedEventHandler(calcWorker_ProgressChanged);
                calcWorker.WorkerReportsProgress = true;
                calcWorker.WorkerSupportsCancellation = true;
                pop.CancelHappened += new EventHandler(pop_CancelHappened);
            }

            pop.Show();
            if (!calcWorker.IsBusy)
                calcWorker.RunWorkerAsync();
        }


        void pop_CancelHappened(object sender, EventArgs e) {
            if ((null != calcWorker) && calcWorker.IsBusy) {
                calcWorker.CancelAsync();
            }
        }
    }
}
