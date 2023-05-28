using System;
using System.Collections.Generic;
using System.Windows;

namespace Grid.PathFinding
{
    public class BestFirstSearch:BasePathFinding
    {
        private NodeHandler nodeHandler;
        private Func<Point, Point, double> _hFunc;

        public BestFirstSearch(Node startNode, Node endNode, NodeHandler nodeHandler) : base(startNode, endNode)
        {
            this.nodeHandler = nodeHandler;
            _hFunc = Heuristics.EuclideanDistance;
        }

        public override List<Node> FindPath()
        {
            PriorityQueue<Node, double> priorityQueue = new PriorityQueue<Node, double>();
            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
            Dictionary<Node, double> gScore = new Dictionary<Node, double>();

            List<Node> visited = new List<Node>();
            double h = _hFunc(_startNode.TopLeft, _endNode.TopLeft);
            priorityQueue.Enqueue(_startNode, h);
            gScore[_startNode] = h;
            Func<Node, double> getGScore = (node) => { return gScore.ContainsKey(node) ? gScore[node] : _pseudoInfinity; };

            while (priorityQueue.Count != 0)
            {
                Node current = priorityQueue.Dequeue();
                CurrentMark(current);
                if (current.TopLeft.Equals(_endNode.TopLeft))
                {
                    return ConstructPath(cameFrom, current);
                }
                List<Node> neighbours = nodeHandler.GetNeighbors(current);
                neighbours.ForEach(n =>
                {
                    if (!visited.Contains(n))
                    {
                    

                    double d = getGScore(current)+25;
                    if (d < getGScore(n))
                    {
                        double h = _hFunc(n.TopLeft,_endNode.TopLeft);
                        gScore[n] = h;
                        cameFrom[n] = current;
                        priorityQueue.Enqueue(n,h);
                        visited.Add(n);
                        QueueMark(n);
                    }
                    }
                });
                VisitedMark(current);
            }

            return null;
        }
        private List<Node> ConstructPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            List<Node> totalPath = new List<Node>();
            totalPath.Add(current);
            Node localCurrent = current;
            while (cameFrom.ContainsKey(localCurrent))
            {
                localCurrent = cameFrom[localCurrent];
                totalPath.Add(localCurrent);
            }
            totalPath.Reverse();
            return totalPath;
        }
    }
}