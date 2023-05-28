using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Linq;
using System.Text;

namespace Grid.PathFinding
{
    class AStar : BasePathFinding
    {
        private Func<Point, Point, double> _hFunc;
        private Func<Node, List<Node>> getNeighbour;
        private NodeHandler nodeHandler;
        private bool dir4;
        public AStar(Node startNode,Node endNode,Func<Point,Point, double> heruistic,NodeHandler nodeHandler, bool d4 = false): base(startNode,endNode)
        {
            _hFunc = heruistic ?? Heuristics.ManhattanDistance;
            this.dir4 = d4;
            this.nodeHandler = nodeHandler;
            getNeighbour = d4 ? this.nodeHandler.GetNeighbors4 : this.nodeHandler.GetNeighbors;
            
        }

        public override List<Node> FindPath()
        {
            PriorityQueue<Node, double> openList = new PriorityQueue<Node, double>();
            List<Node> visited = new List<Node>();

            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
            Dictionary<Node, double> gScore = new Dictionary<Node, double>();
            gScore[_startNode] = 0;
            Dictionary<Node, double> fScore = new Dictionary<Node, double>();
            Func<Node, double> getGScore = (node) => { return gScore.ContainsKey(node) ? gScore[node] : _pseudoInfinity; };
            //orderedOpenList.Add(_startNode);
            double h = _hFunc.Invoke(_startNode.TopLeft, _endNode.TopLeft)/5;
            fScore[_startNode] = h;
            openList.Enqueue(_startNode,h);
            visited.Add(_startNode);

            while (openList.Count != 0)
            {
                Node current = openList.Dequeue();
                CurrentMark(current);
                //orderedOpenList.Remove(current);
                if (current.TopLeft.Equals(_endNode.TopLeft))
                {
                    return ConstructPath(cameFrom, current);
                }
                List<Node> neighbours = getNeighbour(current);
                neighbours.ForEach(neighbor =>
                {
                    //VisitedMark(neighbor);
                    double d = 1;

                    if ( neighbor.TopLeft.X != current.TopLeft.X && neighbor.TopLeft.Y != current.TopLeft.Y)
                    {
                        d = 1.41421356237;
                    }
                    double tentative_gs = getGScore(current) + d;
                    if (tentative_gs < getGScore(neighbor))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative_gs;
                        fScore[neighbor] = tentative_gs + _hFunc(neighbor.TopLeft,_endNode.TopLeft)/25;
                        openList.Enqueue(neighbor, fScore[neighbor]);

                        if (!visited.Contains(neighbor)) {
                            visited.Add(neighbor);
                            QueueMark(neighbor);
                        }
                    }
                });
                VisitedMark(current);
            }

            return null;
        }

        private List<Node> ConstructPath(Dictionary<Node,Node> cameFrom,Node current)
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
