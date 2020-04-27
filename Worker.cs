using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Epidemic_Models {
    public partial class MainWindow : Window {


        void calcWorker_DoWork(object sender, DoWorkEventArgs e) {

            int runs = 1000;
            for (int i = 0; i < runs; i++) {

                if (calcWorker.CancellationPending) {
                    e.Cancel = true;
                    break;
                }

                graph.Nodes.Clear();
                
                for (int n = 0; n < N - infectedN; n++) {   //zdrowe ludzie
                    graph.AddNode(new Hooman());
                }
                for (int m = 0; m < infectedN; m++) {   //chore ludzie
                    graph.AddNode(new Hooman(false, true, false));
                }

                graph.GenerateRandomEdges(N / 20);
                Hooman.society = graph;

                Hooman.SpreadDisease(beta, gamma, lambda, mu, xi, T, N / 10);

                for (int k = 0; k < data.GetLength(1); k++) {
                    data[0, k] = Hooman.graphdata[0][k];
                    data[1, k] += Hooman.graphdata[1][k];
                    data[2, k] += Hooman.graphdata[2][k];
                    data[3, k] += Hooman.graphdata[3][k];

                }
                calcWorker.ReportProgress((int)System.Math.Floor(i * 100.0 / (double)runs));
            }
            
            for (int j = 0; j < data.GetLength(1); j++) {

                data[1, j] = data[1, j] / runs;
                data[2, j] = data[2, j] / runs;
                data[3, j] = data[3, j] / runs;
            }

        }


        void calcWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            pop.pgBar.Value = e.ProgressPercentage;
        }

        void calcWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            pop.Hide();
            MakeAPlot(Solve(), data);
        }

    }
}