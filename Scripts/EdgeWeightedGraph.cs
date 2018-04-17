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
    public class EdgeWeightedGraph {
        private readonly int _v;
        private int _e;
        private LinkedList<Edge>[] _adj;

        public EdgeWeightedGraph(int V) {
            if (V < 0)
                throw new Exception("Number of vertices in a Graph must be nonnegative");

            this._v = V;

            this._e = 0;

            _adj = new LinkedList<Edge>[V];

            for (int v = 0; v < V; v++) {
                _adj[v] = new LinkedList<Edge>();
            }
        }

        public int V() {
            return _v;
        }

        public int E() {
            return _e;
        }

        public void AddEdge(Edge e) {
            int v = e.Source();
            int w = e.Target(v);
            _adj[v].AddFirst(e);
            _adj[w].AddFirst(e);
            _e++;
        }

        public IEnumerable<Edge> Adj(int v) {
            return _adj[v];
        }

        public IEnumerable<Edge> Edges() {
            LinkedList<Edge> list = new LinkedList<Edge>();

            for (int v = 0; v < _v; v++) {
                int selfLoops = 0;

                foreach (Edge e in Adj(v)) {
                    if (e.Target(v) > v) {
                        list.AddFirst(e);
                    }
                    else if (e.Target(v) == v) {
                        if (selfLoops % 2 == 0)
                            list.AddFirst(e);
                        selfLoops++;
                    }
                }
            }

            return list;
        }

        public String toString() {
            String NEWLINE = Environment.NewLine;

            StringBuilder s = new StringBuilder();

            s.Append(_v + " " + _e + NEWLINE);

            for (int v = 0; v < _v; v++) {
                s.Append(v + ": ");

                foreach (Edge e in _adj[v]) {
                    s.Append(e.toString() + "  ");
                }

                s.Append(NEWLINE);
            }
            return s.ToString();
        }
        // DIJKSTRA ALGORITHM, to find distance between all nodes
        double[] dijkstra(int source) {
            List<int> Q = new List<int>();
            int v;
            double[] dist = new double[_v];
            dist[source] = 0; // Distance from source to source is set to 0
            for (v = 0; v < _v; v++) { // Initializations
                if (v != source) {
                    dist[v] = int.MaxValue; // Unknown distance function from source to each node set to infinity
                }
                Q.Add(v); // All nodes initially in Q
            }
            while (Q.Count > 0) { // the main loop
                double minDist=int.MaxValue;
                int minDistVertex=0;
                foreach (int i in Q) {
                    if (dist[i] < minDist) {
                        minDist = dist[i];
                        minDistVertex = i;
                    }
                }
                /*for (int i = 0; i < _v; i++) {
                    if (dist[i] < minDist) {
                        minDist = dist[i];
                        minDistVertex = i;
                    }
                }*/
                v = minDistVertex; // In the first run-through, this vertex is the source node
                Q.Remove(v);

                foreach (Edge u in _adj[v]) { // for each neighbor u of v, where neighbor u has not yet been removed from Q.
                    double alt = dist[v] + u.Weight();
                    if (alt < dist[u.Target(v)]) { // A shorter path to u has been found NOTE: UNSURE ABOUT "u.Source()"
                        dist[u.Source()] = alt; // Update distance of u 
                    }
                }
            }
            return dist;
        }
        // TO FIND A PATH BETWEEN SOURCE AND TARGET, run:
        // pathfind(source, target, findParents())
        public int[] findParents(int source) {
            int[] parents = new int[_v];
            List<int> Q = new List<int>();
            int v;
            double[] dist = new double[_v];
            dist[source] = 0; // Distance from source to source is set to 0
            for (v = 0; v < _v; v++) { // Initializations
                if (v != source) {
                    dist[v] = int.MaxValue; // Unknown distance function from source to each node set to infinity
                }
                Q.Add(v); // All nodes initially in Q
            }
            while (Q.Count > 0) { // the main loop
                // first we find the vertex in Q with minimum distance from v: dist[v]
                // In the first run-through, this vertex is the source node
                double minDist = int.MaxValue;
                int minDistVertex = -1;
                foreach (int i in Q) {
                    if (dist[i] < minDist) {
                        minDist = dist[i];
                        minDistVertex = i;
                    }
                }
                v = minDistVertex;
                Q.Remove(v);

                foreach (Edge e in _adj[v]) { // for each neighbor u of v, where neighbor u has not yet been removed from Q.
                    int u = e.Target(v); //NOTE: UNSURE ABOUT "e.Source()" vs "e.Target(v)"
                    if (Q.Contains(u)) {
                        double alt = dist[v] + e.Weight(); // distance of v plus weight of edge u
                        if (alt < dist[u]) { // A shorter path to u has been found
                            dist[u] = alt; // Update distance of u 
                            parents[u] = v;
                        }
                    }
                }
            }
            return parents;
        }
        // recursive algorithm for finding path from source to currentVertex
        public List<int> pathfind(int source, int currentVertex, int[] parents) {
            // Base case : Source node has
            // been processed
            if (currentVertex == source) {
                return new List<int>(); ;
            }
            List<int> tempPath = pathfind(source,parents[currentVertex], parents);
            tempPath.Add(currentVertex);
            return tempPath;
            //System.out.print(currentVertex + " ");
        }

        public EdgeWeightedGraph Clone() {
            EdgeWeightedGraph clone = new EdgeWeightedGraph(_v);
            IEnumerable<Edge> edges = Edges();
            Edge tempEdge;
            foreach (Edge e in edges) {
                tempEdge = new Edge(e.Source(), e.Target(e.Source()), e.Weight());
                clone.AddEdge(tempEdge);
            }
            return clone;
        }
    }
}