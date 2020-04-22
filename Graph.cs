using System;
using System.Collections.Generic;
using System.Linq;

namespace Epidemic_Models {
    public class Graph<T> {

        private bool _isDirected = false;
        private bool _isWeighted = false;
        public List<Node<T>> Nodes { get; set; } = new List<Node<T>>();

        public Graph(bool isDirected, bool isWeighted) {
            _isDirected = isDirected;
            _isWeighted = isWeighted;
        }

        public Edge<T> this[int from, int to] {     //indeksator - pobiera instancje klasy Node (nodeFrom i nodeTo) i znajduje krawędz
            get {
                Node<T> nodeFrom = Nodes[from];
                Node<T> nodeTo = Nodes[to];
                int i = nodeFrom.Neighbours.IndexOf(nodeTo);
                if (i >= 0) {
                    Edge<T> edge = new Edge<T>() {
                        From = nodeFrom,
                        To = nodeTo,
                        Weight = i < nodeFrom.Weights.Count ? nodeFrom.Weights[i] : 0
                    };
                    return edge;
                }

                return null;
            }
        }
        public Node<T> AddNode(T value) {
            Node<T> node = new Node<T>() { Data = value };
            Nodes.Add(node);
            UpdateIndices();
            return node;
        }

        public void AddEdge(Node<T> from, Node<T> to, int weight = 0) {
            from.Neighbours.Add(to);
            if (_isWeighted) {
                from.Weights.Add(weight);
            }
            if (!_isDirected) {
                to.Neighbours.Add(from);
                if (_isWeighted) {
                    to.Weights.Add(weight);
                }
            }
        }

        public void RemoveNode(Node<T> nodeToRemove) {
            foreach (Node<T> node in Nodes) {
                RemoveEdge(node, nodeToRemove);
            }
            Nodes.Remove(nodeToRemove);
            UpdateIndices();
        }

        public void RemoveEdge(Node<T> from, Node<T> to) {
            int index = from.Neighbours.FindIndex(n => n == to);
            if (index >= 0) {
                from.Neighbours.RemoveAt(index);
                if (_isWeighted) {
                    from.Weights.RemoveAt(index);
                }
            }
        }

        public List<Edge<T>> GetEdges() {
            List<Edge<T>> edges = new List<Edge<T>>();
            foreach (Node<T> from in Nodes) {
                for (int i = 0; i < from.Neighbours.Count; i++) {
                    Edge<T> edge = new Edge<T>() {
                        From = from,
                        To = from.Neighbours[i],
                        Weight = i < from.Weights.Count ? from.Weights[i] : 0
                    };
                    edges.Add(edge);
                }
            }
            return edges;
        }

        private void UpdateIndices() {// przechodzi przez wszystkie wezly i zmienia walsciwosci Index na kolejen liczby zaczynajac od 0
            int i = 0;
            Nodes.ForEach(n => n.Index = i++);
        }

        public void AddNodeWithRandomEdges(int maxDegree, T value) {
            AddNode(value);
            Random rand = new Random();
            for (int i = 0; i < maxDegree; i++) {
                int r = rand.Next(Nodes.Count - 1);
                AddEdge(Nodes[Nodes.Count - 1], Nodes[r]);
            }
        }

        public List<double>[] GenerateRandomEdges(int maxDegree) {
            int n = this.Nodes.Count, r, n1, n2;
            int[] neighbors = new int[n], count = new int[n];
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            List<double>[] data = new List<double>[2];
            data[0] = x;
            data[1] = y;

            Random rand = new Random();
            // wersja Zuzy
            bool[][] matrix = new bool[n][];
            for (int a = 0; a < n; a++) {
                matrix[a] = new bool[n];
                for (int b = 0; b < n; b++) {
                    matrix[a][b] = false;
                    Console.Write(matrix[a][b] + "\t");
                }
                Console.WriteLine();
            }

            for (int i = 0; i < n; i++) {
                do {
                    n1 = rand.Next(0, n);
                } while (n1 == i || (matrix[i][n1] == true && matrix[n1][i] == true));
                matrix[i][n1] = true;
                matrix[n1][i] = true;
                this.AddEdge(this.Nodes[i], this.Nodes[n1]);

            }
            Console.WriteLine();
            for (int a = 0; a < n; a++) {
                for (int b = 0; b < n; b++) {
                    if (matrix[a][b] == true) {
                        count[a] += 1;
                    }
                    Console.Write(matrix[a][b] + "\t");
                }
                Console.WriteLine();
            }

            for (int c = 0; c < n; c++) {
                Console.WriteLine(count[c]);
            }

            //histogram 
            count
                .GroupBy(i => i)
                .Select(g => new {
                    Item = g.Key,
                    Count = g.Count()

                })
                .OrderBy(g => g.Item)
                .ToList()
                .ForEach(g => {
                    x.Add(g.Count);
                    y.Add(g.Item);
                    Console.WriteLine("{0} occurred {1} times", g.Item, g.Count);
                });

            for (int w = 0; w < 2; w++) {
                foreach (int item in data[w]) {
                    Console.Write(item + "\t");
                }
                Console.WriteLine();
            }

            //wersja Adama, ale tam chyba cos wczesniej bylo i zniknelo, przepraszam ;)
            /*for (int i = 0; i < n - 1; i++) {
                if (neighbors[i] < maxDegree) {
                    for (int j = 0; j < maxDegree - neighbors[i]; j++) {
                        r = rand.Next(i + 1, n);
                        if (neighbors[r] < maxDegree) {
                            matrix[i][r] = matrix[r][i] = true;
                            neighbors[i] += 1;
                            neighbors[r] += 1;
                        }
                    }
                }
            }

            //teraz ich połączmy
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < i; j++) {
                    if (matrix[i][j]) {
                        this.AddEdge(this.Nodes[i], this.Nodes[j]);
                    }
                }
            }*/
            // returnuje bo rysuje histogram
            return data;
        }
    }
}
