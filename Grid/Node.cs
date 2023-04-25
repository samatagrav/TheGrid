using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Grid
{
    public class Node
    {
        private Size _size = new Size(1.0, 1.0);//TODO:use a provider maybe
        private double a = 0.5;
        private double c = 0.7071;
        private Point _topLeft;
        public Node(Point topLeft)
        {
            _topLeft = topLeft;
        }

        public bool Contains(Point point)
        {
            return (new Rect(_topLeft, _size)).Contains(point);
        }

        public override string ToString()
        {
            return "Node_ "+_topLeft.ToString();
        }

        public Point GetTopLeft()
        {
            return _topLeft;
        }

    }

    public enum Direction
    {
        North,East,South,West,NorthEast,SouthEast,SouthWest,NorthWest
    }

    public class NodeHandler
    {
        private List<List<Node>> _nodeList;
        private int _offset;
        public NodeHandler(int capacityRows, int capacityColumns,int offset)
        {
            _offset = offset;
            _nodeList = new List<List<Node>>(capacityColumns);
            for (int i = 0; i < capacityColumns; i++)
            {
                List<Node> list = new List<Node>(capacityRows);
                _nodeList.Add(list);
            }
        }

        public List<List<Node>> GetNodes() => _nodeList;

        public void CreateNode(int x, int y, Point point)
        {
            _nodeList[x].Insert(y,new Node(point));
        }

        public Node GetNode(Point position)
        {
            return _nodeList[applyOffset((int)position.X)][applyOffset((int)position.Y)];
        }
        public Node GetNode(int x, int y)
        {
            return _nodeList[x][y];
        }
        public Node GetAdjacent(Node referenceNode,Direction direction)
        {
            Point topLeft = referenceNode.GetTopLeft();
            int x = (int)topLeft.X, y = (int)topLeft.Y;
            x = applyOffset(x);
            y = applyOffset(y);
            int somethingsomethingoffset = 1;//TODO:handle edge cases like -1
            switch (direction)
            {
                case Direction.North:
                    y = y - somethingsomethingoffset;
                    break;
                case Direction.NorthEast:
                    x = x + somethingsomethingoffset;
                    y = y - somethingsomethingoffset;
                    break;
                case Direction.East:
                    x = x + somethingsomethingoffset;
                    break;
                case Direction.SouthEast:
                    x = x + somethingsomethingoffset;
                    y = y + somethingsomethingoffset;
                    break;
                case Direction.South:
                    y = y + somethingsomethingoffset;
                    break;
                case Direction.SouthWest:
                    x = x - somethingsomethingoffset;
                    y = y + somethingsomethingoffset;
                    break;
                case Direction.West:
                    x = x - somethingsomethingoffset;
                    break;
                case Direction.NorthWest:
                    x = x - somethingsomethingoffset;
                    y = y - somethingsomethingoffset;
                    break;
                default:
                    break;
            }
            return _nodeList[x][y];
        }

        private int applyOffset(int num)
        {
            return num / _offset;
        }
    }
}