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
        private const int _pseudoInfinity = Int32.MaxValue;//On the scale it fits the infinity
        private Func<Point, Point, double> _hFunc;
        private NodeHandler nodeHandler;
        public AStar(Node startNode,Node endNode,Func<Point,Point, double> heruistic,NodeHandler nodeHandler): base(startNode,endNode)
        {
            heruistic = ManhattanDistance;
            _hFunc = ManhattanDistance;
            this.nodeHandler = nodeHandler;
        }

        public override List<Node> FindPath()
        {
            List<Node> orderedOpenList = new List<Node>();

            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
            Dictionary<Node, int> gScore = new Dictionary<Node, int>();
            gScore[_startNode] = 0;
            Dictionary<Node, double> fScore = new Dictionary<Node, double>();
            Func<Node, int> getGScore = (node) => { return gScore.ContainsKey(node) ? gScore[node] : _pseudoInfinity; };

            orderedOpenList.Add(_startNode );
            fScore[_startNode] = _hFunc.Invoke(_startNode.TopLeft, _endNode.TopLeft);
            while (orderedOpenList.Count != 0)
            {
                Node current=  orderedOpenList.OrderBy(a => fScore[a]).First();
                if (current.TopLeft.Equals(_endNode.TopLeft))
                {
                    return constructPath(cameFrom, current);
                }
                List<Node> neighbours =  nodeHandler.GetNeighbors(current);
                orderedOpenList.Remove(current);
                neighbours.ForEach(neighbor =>
                {
                    int tentative_gs = getGScore(current) + 1;//path weight is 1
                    if (tentative_gs < getGScore(neighbor))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative_gs;
                        fScore[neighbor] = tentative_gs + _hFunc(neighbor.TopLeft,_endNode.TopLeft);
                        if(!orderedOpenList.Contains(neighbor)) {
                            orderedOpenList.Add(neighbor);
                        }
                    }
                });
            }

            return null;
        }

        private List<Node> constructPath(Dictionary<Node,Node> cameFrom,Node current)
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

        public override void VisitedMark()
        {
        }

        public override void CurrentMark()
        {
        }

        public static double ManhattanDistance(Point a, Point b)
        {
            return (Math.Abs(a.X-b.X)+ Math.Abs(a.Y-b.Y));
        }
    }
}
