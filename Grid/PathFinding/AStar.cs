﻿using System;
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
        private NodeHandler nodeHandler;
        public AStar(Node startNode,Node endNode,Func<Point,Point, double> heruistic,NodeHandler nodeHandler): base(startNode,endNode)
        {
            _hFunc = heruistic ?? Heuristics.ManhattanDistance;

            this.nodeHandler = nodeHandler;
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
            double h = _hFunc.Invoke(_startNode.TopLeft, _endNode.TopLeft);
            fScore[_startNode] = h;
            openList.Enqueue(_startNode,h);
            visited.Add(_startNode);

            while (openList.Count != 0)
            {
                Node current = openList.Dequeue();
                //orderedOpenList.Remove(current);
                if (current.TopLeft.Equals(_endNode.TopLeft))
                {
                    return ConstructPath(cameFrom, current);
                }
                List<Node> neighbours =  nodeHandler.GetNeighbors(current);
                CurrentMark(current);
                neighbours.ForEach(neighbor =>
                {
                    //VisitedMark(neighbor);
                    double d = 1;
                    if (neighbor.TopLeft.X != current.TopLeft.X && neighbor.TopLeft.Y != current.TopLeft.Y)
                    {
                        d = 1.41421356237;
                    }
                    double tentative_gs = getGScore(current) + d;//path weight is 1
                    if (tentative_gs < getGScore(neighbor))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative_gs;
                        fScore[neighbor] = tentative_gs + _hFunc(neighbor.TopLeft,_endNode.TopLeft);
                        if(!visited.Contains(neighbor)) {
                            visited.Add(neighbor);
                            openList.Enqueue(neighbor,fScore[neighbor]);
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
