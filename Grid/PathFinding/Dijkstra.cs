using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grid.PathFinding
{
    class Dijkstra : BasePathFinding
    {
        private NodeHandler _nodeHandler;
        public Dijkstra(Node startNode, Node endNode, NodeHandler nodeHandler) : base(startNode, endNode)
        {
            _nodeHandler = nodeHandler;
        }

        public override List<Node> FindPath()
        {
            Dictionary<Node, double> dist = new Dictionary<Node, double>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
            List<Node> Q = new List<Node>();
            List<List<Node>> nodesM = _nodeHandler.GetNodes();
            nodesM.ForEach(nodesL =>
            {
                nodesL.ForEach(node =>
                {
                    dist[node] = _pseudoInfinity;
                    Q.Add(node);
                });
            });
            dist[_startNode] = 0;
            while (Q.Count != 0)
            {
               Node current =  Q.OrderBy(e => dist[e]).First();
               Q.Remove(current);
               CurrentMark(current);
               if (current.TopLeft.Equals(_endNode.TopLeft))
               {
                   return ConstructPath(prev, current);
               }

               _nodeHandler.GetNeighbors(current).ForEach( neighbor =>
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
            }

            return null;
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
            totalPath.Reverse();
            return totalPath;
        }
    }
}
