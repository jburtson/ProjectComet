/* Jonathan Burtson
 * 4/17/2018
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Programming.WeightedUndirected {
    public class Edge : IComparable<Edge> {
        private readonly int _v;
        private readonly int _w;
        private readonly double _weight;

        public Edge(int v, int w, double weight) {
            this._v = v;
            this._w = w;
            this._weight = weight;
        }

        public double Weight() {
            return _weight;
        }

        public int CompareTo(Edge that) {
            if (this.Weight() < that.Weight())
                return -1;
            else if (this.Weight() > that.Weight())
                return +1;
            else
                return 0;
        }

        public int Source() {
            return _v;
        }

        public int Target(int vertex) {
            if (vertex == _v) return _w;
            else if (vertex == _w) return _v;
            else throw new Exception("Illegal endpoint");
        }

        public String toString() {
            return String.Format("{0:d}-{1:d} {2:f5}", _v, _w, _weight);
        }
    }
}