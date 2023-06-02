using System.Collections.Generic;
using System.Linq;

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
            List<Node> visited = new List<Node>();
            List<Node> Q = new List<Node>();
            List<List<Node>> nodesM = _nodeHandler.GetNodes();
            nodesM.ForEach(nodesL =>
            {
                nodesL.ForEach(node =>
                {
                    if (node.IsPassable())
                    {
                        dist[node] = _pseudoInfinity;
                        Q.Add(node);
                    }
                });
            });
            dist[_startNode] = 0;
            visited.Add(_startNode);
            while (Q.Count != 0)
            {
                Node current = Q.OrderBy(e => dist[e]).First();
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
                          //  QueueMark(neighbor);
                        }

                        if (!visited.Contains(neighbor))
                        {
                            QueueMark(neighbor);
                            visited.Add(neighbor);
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
            List<List<Node>> nodesM = _nodeHandler.GetNodes();
            
            nodesM.ForEach(nodesL =>
            {
                nodesL.ForEach(node =>
                {
                    if (node.IsPassable())
                    {
                        dist[node] = _pseudoInfinity;
                        Q.Add(node); ;
                    }
                });
            });
            dist[_startNode] = 0;

            while (Q.Count != 0)
            {
                Node current = Q.OrderBy(e => dist[e]).First();
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
                        QueueMark(neighbor);

                    }
                });
                VisitedMark(current);

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
