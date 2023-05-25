using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace Grid
{
    public class Node
    {
        private Size _size = new Size(1.0, 1.0);//TODO:use a provider maybe
        private double a = 0.5;
        private double c = 0.7071;
        private Point _topLeft;
        private FieldTypeValue _fieldType;

        public Node()
        {
        }

        public Node(Point topLeft, FieldTypeValue ftv = FieldTypeValue.Tile)
        {
            _topLeft = topLeft;
            _fieldType = ftv;
        }

        public bool Contains(Point point)
        {
            return (new Rect(_topLeft, _size)).Contains(point);
        }

        public override string ToString()
        {
            return "Node_ "+_topLeft.ToString();
        }

        public Point TopLeft
        {
            get
            {
                return _topLeft;
            }
            set
            {
                _topLeft = value;
            }
        }

        public FieldTypeValue FieldType
        {
            get
            {
                return _fieldType;
            }
            set
            {
                _fieldType = value;
            }
        }

        public void SetWall()
        {
            _fieldType = FieldTypeValue.Wall;
        }

        public void SetTile()
        {
            _fieldType = FieldTypeValue.Tile;
        }

        public bool IsPassable()
        {
            return _fieldType != FieldTypeValue.Wall;
        }

    }

    public enum FieldTypeValue
    {
        Tile,Wall
    }

    public enum Direction
    {
        North,East,South,West,NorthEast,SouthEast,SouthWest,NorthWest
    }

    public class NodeHandler
    {
        private List<List<Node>> _nodeList;
        private int _offset;
        
        public Node Start
        {
            get;
            set;
        }

        public Node End
        {
            get;
            set;
        }

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

        public void CreateNode(int x, int y, Node n)
        {
            _nodeList[x].Insert(y, n);
        }

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

        public List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();
            Point topLeft = node.TopLeft;
            int x = (int)topLeft.X, y = (int)topLeft.Y;
            x = applyOffset(x);
            y = applyOffset(y);

            if (x > 0 && y > 0 )
            {
                if (x < _nodeList.Count -1 && y < _nodeList[0].Count -1)
                {
                    AddIfNotWall(neighbors, x - 1, y - 1);
                    AddIfNotWall(neighbors, x - 1, y);
                    AddIfNotWall(neighbors, x - 1, y + 1);
                    AddIfNotWall(neighbors, x + 1, y - 1);
                    AddIfNotWall(neighbors, x + 1, y);
                    AddIfNotWall(neighbors, x + 1, y+1);
                    AddIfNotWall(neighbors, x, y-1);
                    AddIfNotWall(neighbors, x, y+1);
                }
                else if (x < _nodeList.Count -1)
                {
                    AddIfNotWall(neighbors,x - 1,y - 1);
                    AddIfNotWall(neighbors,x - 1,y);
                    AddIfNotWall(neighbors,x + 1,y - 1);
                    AddIfNotWall(neighbors,x + 1,y);
                    AddIfNotWall(neighbors,x,y - 1);
                } else if (y < _nodeList[0].Count -1)
                {
                    AddIfNotWall(neighbors,x - 1,y - 1);
                    AddIfNotWall(neighbors,x - 1,y);
                    AddIfNotWall(neighbors,x - 1,y + 1);
                    AddIfNotWall(neighbors,x,y - 1);
                    AddIfNotWall(neighbors,x,y + 1);
                }
                else
                {
                    AddIfNotWall(neighbors,x - 1,y - 1);
                    AddIfNotWall(neighbors,x - 1,y);
                    AddIfNotWall(neighbors,x,y - 1);
                }
            }
            else if (x == 0 && y == 0)
            {
                AddIfNotWall(neighbors, x + 1, y);
                AddIfNotWall(neighbors, x + 1, y + 1);
                AddIfNotWall(neighbors, x, y + 1);
            }
            else
            {
                if (y == 0)
                {
                    if (x < _nodeList.Count - 1)
                    {
                        AddIfNotWall(neighbors,x - 1,y);
                        AddIfNotWall(neighbors,x - 1,y + 1);
                        AddIfNotWall(neighbors,x + 1,y);
                        AddIfNotWall(neighbors,x + 1,y + 1);
                        AddIfNotWall(neighbors,x,y + 1);
                    }
                    else
                    {
                        AddIfNotWall(neighbors,x - 1,y);
                        AddIfNotWall(neighbors,x - 1,y + 1);
                        AddIfNotWall(neighbors,x,y + 1);
                    }
                }
                else if(x == 0)
                {
                    if (y < _nodeList[0].Count - 1)
                    {
                        AddIfNotWall(neighbors,x + 1,y - 1);
                        AddIfNotWall(neighbors,x + 1,y);
                        AddIfNotWall(neighbors,x + 1,y + 1);
                        AddIfNotWall(neighbors,x,y - 1);
                        AddIfNotWall(neighbors,x,y + 1);
                    }
                    else
                    {
                        AddIfNotWall(neighbors,x + 1,y - 1);
                        AddIfNotWall(neighbors,x + 1,y);
                        AddIfNotWall(neighbors,x,y - 1);
                    }
                }
            }

            return neighbors;
        }

        public Node GetAdjacent(Node referenceNode,Direction direction)
        {
            Point topLeft = referenceNode.TopLeft;
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

        public List<String> Serialize()
        {
            List<String> result = new List<string>();
            _nodeList.ForEach(a =>
            {
                a.ForEach(b =>
                {
                    result.Add(JsonSerializer.Serialize(b));
                });
            });
            return result;
        }

        public void Deserialize()
        {
            
        }

        public void Flush()
        {
            _nodeList.ForEach(x=>x.Clear());
        }

        private int applyOffset(int num)
        {
            return num / _offset;
        }

        private void AddIfNotWall(List<Node> result,int x, int y)
        {
            Node node = _nodeList[x][y];
            if (node.IsPassable())
            {
                result.Add(node);
            }
        }
    }
}