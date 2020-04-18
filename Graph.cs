using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if(i >= 0) {
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
            foreach(Node<T> node in Nodes) {
                RemoveEdge(node, nodeToRemove);
            }
            Nodes.Remove(nodeToRemove);
            UpdateIndices();
        }

        public void RemoveEdge(Node<T> from, Node<T> to) {
            int index = from.Neighbours.FindIndex(n => n == to);
            if(index >= 0) {
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
                Console.WriteLine(Nodes.Count);
                AddEdge(Nodes[Nodes.Count], Nodes[rand.Next(1, Nodes.Count - 1)]);
            }
        }

        public void GenerateRandomEdges(int maxDegree) {
            int n = this.Nodes.Count, r;
            int[] neighbors = new int[n];
            Random rand = new Random();

            bool[][] matrix = new bool[n][];
            for (int a = 0; a < n; a++) matrix[a] = new bool[n];

            for(int i = 0; i < n - 1; i++) {
                if (neighbors[i] < maxDegree) {
                    for (int j = 0; j < maxDegree - neighbors[i]; j++) {
                        r = rand.Next(i + 1, n);
                        if(neighbors[r] < maxDegree) {
                            matrix[i][r] = matrix[r][i] = true;
                            neighbors[i] += 1;
                            neighbors[r] += 1;
                        }
                    }
                }
            }

            //teraz ich połączmy
            for(int i = 0; i < n; i++) {
                for(int j = 0; j < i; j++) {
                    if (matrix[i][j]) {
                        this.AddEdge(this.Nodes[i], this.Nodes[j]);
                    }
                }
            }
        }
    }
}
