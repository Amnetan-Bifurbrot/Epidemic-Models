using System;
using System.Windows;

namespace Epidemic_Models {
    public partial class MainWindow : Window {
        // funkcja wywolana jest w Button_Click
        /*private void CreateRandomGraph() {
            Random rand = new Random();
            int n = Int32.Parse(NTb.Text), max = 5;
            Console.WriteLine("n: " + n);
            nodes = new Node<int>[n];

            //dodanie wierzcholkow do grafu
            for (int k = 0; k < n; k++) {
                nodes[k] = graph.AddNode(k);              
            }

            // int contacts = Int32.Parse(cTb.Text);


            // masakrejsyn, tego fragmentu kodu nikt nie powinien widziec na oczenta
            /*for (int i = 0; i < n; i++) {
                int a = 0;
                for(int j = 0; j < contacts; j++) {
                    a++;
                    Edge<int> newEdge = new Edge<int>();
                    newEdge.From = nodes[i];
                    newEdge.To = nodes[rand.Next(0, n)];

                    if (graph.GetEdges().Count == 0) {
                        graph.AddEdge(newEdge.From, newEdge.To);
                    }

                    foreach(Edge<int> edge in graph.GetEdges()) {
                        
                        Console.WriteLine("NewEdge: " + newEdge.ToString());
                        if(!(edge.From.Index == newEdge.From.Index && edge.To.Index == newEdge.To.Index)) {
                            graph.AddEdge(newEdge.From, newEdge.To);
                        }
                    }
                  
                }
                Console.WriteLine("Counter: " + a);
            }

            

            for (int i = 0; i < n; i++) {       //oj Zuze Zuze
                int N = nodes[i].Neighbours.Count;
                if (nodes[i].Neighbours.Count < max + 1 - N) {
                    for (int j = 0; j < max - N; j++) {
                        int r = rand.Next(n);
                        Edge<int> newEdge = new Edge<int>(nodes[i], nodes[r]);
                        if (!graph.GetEdges().Contains(newEdge) && nodes[r].Neighbours.Count < max) {
                            graph.AddEdge(nodes[i], nodes[r]);
                        }
                    }
                }
            }

            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    if (i != j) {
                        if (rand.NextDouble() <= Convert.ToDouble(pTb.Text)) {
                            graph.AddEdge(nodes[i], nodes[j]);
                        }
                    }
                }
            }
           
            // wypisuje co kto ile ma krawedzi
            for (int l = 0; l < n; l++) {
                Console.WriteLine(nodes[l].ToString());
            }
        }*/
    }
}

