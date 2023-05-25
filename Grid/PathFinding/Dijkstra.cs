using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grid.PathFinding
{
    class Dijkstra : BasePathFinding
    {
        private NodeHandler _nodeHandler;
        private bool _shortest;
        public Dijkstra(Node startNode, Node endNode, NodeHandler nodeHandler,bool shortest = false) : base(startNode, endNode)
        {
            _nodeHandler = nodeHandler;
            _shortest = shortest;
        }

        public override List<Node> FindPath()
        {
            return _shortest ? ShortestPath() : AnyPath();
        }

        private List<Node> AnyPath()
        {
            Dictionary<Node, double> dist = new Dictionary<Node, double>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
            
            List<Node> Q = new List<Node>();
            PriorityQueue<Node, double> pQ = new PriorityQueue<Node, double>();
            List<List<Node>> nodesM = _nodeHandler.GetNodes();
            nodesM.ForEach(nodesL =>
            {
                nodesL.ForEach(node =>
                {
                    if (node.IsPassable())
                    {
                        dist[node] = _pseudoInfinity;
                        Q.Add(node);
                        if(!node.Equals(_startNode))
                            pQ.Enqueue(node,_pseudoInfinity);
                    }
                });
            });
            dist[_startNode] = 0;
            pQ.Enqueue(_startNode,0);
            while (pQ.Count != 0)
            {
                Node current = pQ.Dequeue();
                Q.Remove(current);
                CurrentMark(current);
                if (current.TopLeft.Equals(_endNode.TopLeft))
                {
                    return ConstructPath(prev, current);
                }

                _nodeHandler.GetNeighbors(current).ForEach(neighbor =>
                {
                    if (Q.Contains(neighbor))
                    {
                        double d = 1;
                        if (neighbor.TopLeft.X != current.TopLeft.X && neighbor.TopLeft.Y != current.TopLeft.Y)
                        {
                            d = 1.41421356237;
                        }

                        double alt = dist[current] + d;
                        if (alt < dist[neighbor])
                        {
                            dist[neighbor] = alt;
                            prev[neighbor] = current;
                        }
                    }
                });
                VisitedMark(current);
            }

            return null;
        }

        private List<Node> ShortestPath()
        {
            bool hasPath = false;
            Dictionary<Node, double> dist = new Dictionary<Node, double>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
            List<Node> Q = new List<Node>();
            PriorityQueue<Node, double> pQ = new PriorityQueue<Node, double>();
            List<List<Node>> nodesM = _nodeHandler.GetNodes();
            
            nodesM.ForEach(nodesL =>
            {
                nodesL.ForEach(node =>
                {
                    if (node.IsPassable())
                    {
                        dist[node] = _pseudoInfinity;
                        Q.Add(node);

                        if (!node.Equals(_startNode))
                            pQ.Enqueue(node, _pseudoInfinity);
                    }
                });
            });
            dist[_startNode] = 0;
            pQ.Enqueue(_startNode, 0);

            while (pQ.Count != 0)
            {
                Node current = pQ.Dequeue();
                Q.Remove(current);
                CurrentMark(current);
                _nodeHandler.GetNeighbors(current).ForEach(neighbor =>
                {
                    if (Q.Contains(neighbor))
                    {
                        double d = 1;
                        if (neighbor.TopLeft.X != current.TopLeft.X && neighbor.TopLeft.Y != current.TopLeft.Y)
                        {
                            d = 1.41421356237;
                        }

                        double alt = dist[current] + d;
                        if (alt < dist[neighbor])
                        {
                            dist[neighbor] = alt;
                            prev[neighbor] = current;
                        }
                        VisitedMark(neighbor);
                    }
                });
            }

            return ConstructPath(prev,_endNode) ;
        }

        private List<Node> ConstructPath(Dictionary<Node,Node> prev, Node current)
        {
            List<Node> totalPath = new List<Node>();
            totalPath.Add(current);
            Node localCurrent = current;
            while (prev.ContainsKey(localCurrent))
            {
                localCurrent = prev[localCurrent];
                totalPath.Add(localCurrent);
            }
            if (totalPath.Count == 1)
            {
                return null;
            }
            totalPath.Reverse();
            return totalPath;
        }

    }
}
