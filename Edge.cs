using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemic_Models {
    public class Edge<T> {
        public Node<T> From { get; set; }
        public Node<T> To { get; set; }
        public int Weight { get; set; }

        public Edge(Node<T> from, Node<T> to) {
            From = from;
            To = to;
        }

        public Edge() { }

        public override string ToString() {
            return $"Egde {From.Data} -> {To.Data}, weight: {Weight}";
        }

    }
}
