using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Grid.PathFinding;

// ReSharper disable All

namespace Grid
{
    public class GridController
    {
        private MainWindow mainWindow;
        public GridController(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        private const int _offSet = 25;
        private const string Separator = "\n";
        private const int _sleepTimeInMiliseconds = 50;
        private DrawingVisual _gridLinesVisual = new DrawingVisual();
        private DrawingGroup _drawingGroup = new DrawingGroup();
        private readonly Color _red = Color.FromRgb(255, 0, 0);
        private readonly Color _black = Color.FromRgb(0, 0, 0);
        private readonly Color _brown = Color.FromRgb(165, 75, 0);
        private readonly Color _greenStart = Color.FromRgb(50,200,50);
        private readonly Color _greenEnd = Color.FromRgb(10, 100, 10);
        private readonly Color _white = Color.FromRgb(255, 255, 255);
        private readonly Color _blue = Color.FromRgb(25, 25, 125);
        private readonly Color _yellow = Color.FromRgb(255,255,0);
        private readonly Color _orange = Color.FromRgb(255,165,0);

        private Nullable<Point> _lastPoint;
        private int _baseGridHash;
        private NodeHandler _nodeHandler;
        private int _width;
        private int _height;
        private int _offset;
        private int _rows;
        private int _columns;
        private int visited = 0;
        public void InnitGrid()
        {

            _width = (int)mainWindow.GridImage.Width;
            _height = (int)mainWindow.GridImage.Height;
            _offset = 25;
            _rows = _height / _offset;
            _columns = _width / _offset;
            _nodeHandler= new NodeHandler(_rows,_columns, _offset);
            DrawGrid(_rows, _columns);
            PopulateFields(_rows,_columns);
        }

        public void WallClick(Point position)
        {
            Drawing result = _drawingGroup.Children.FirstOrDefault(x => x.Bounds.Contains(position) && !x.Bounds.GetHashCode().Equals(_baseGridHash));
            if (result != null && _lastPoint.HasValue && result.Bounds.Contains(_lastPoint.Value))
            {
                _lastPoint = position;
                return;
            }
            else if (result != null && _lastPoint.HasValue)
            {
                _lastPoint = null;
            }
            else 
            {
                _lastPoint = position;
            }

            if (result == null)
            {
                var node = _nodeHandler.GetNode(position);
                node.SetWall();
                ColourSquare(node.TopLeft, _brown, _brown);
            }
            else
            {
                var node = _nodeHandler.GetNode(position);
                node.SetTile();
                _drawingGroup.Children.Remove(result);
                mainWindow.GridImage.Source = new DrawingImage(_drawingGroup);
            }
        }
        public void Serialize(String name)
        {
            List<string> result =  _nodeHandler.Serialize();
            File.WriteAllLines(name + ".json", result);
        }

        public void LeftClick(Point position)
        {
            Node node = _nodeHandler.GetNode(position);
            Drawing result = _drawingGroup.Children.FirstOrDefault(x => x.Bounds.Contains(position) && !x.Bounds.GetHashCode().Equals(_baseGridHash));
            if (result == null)
            {
                node.SetWall();
                ColourSquare(node.TopLeft, _brown, _brown); 
            }
            else
            {
                node.SetTile();
                _drawingGroup.Children.Remove(result);
                mainWindow.GridImage.Source = new DrawingImage(_drawingGroup);
            }

        }

        public void SetStart(Point position)
        {
            Node node = _nodeHandler.GetNode(position);
            _nodeHandler.Start =node;
            ColourSquare(node.TopLeft, _greenStart, _greenStart);
        }

        public void SetEnd(Point position)
        {
            Node node = _nodeHandler.GetNode(position);
            _nodeHandler.End = _nodeHandler.GetNode(position);
            ColourSquare(node.TopLeft, _greenEnd, _greenEnd);
        }

        private void DrawGrid(int rows, int columns)
        {
            DrawingContext dct = _gridLinesVisual.RenderOpen();
            SolidColorBrush scb = new SolidColorBrush(_black);
            Pen lightPen = new Pen(scb, 0.5), darkPen = new Pen(scb, 1);
            int width = (int)mainWindow.GridImage.Width;
            int height = (int)mainWindow.GridImage.Height;
            int yOffset = 25,
                xOffset = 25,
                rowssHeight = height / yOffset,
                columnss = width / xOffset;
            Point point0 = new Point(0, 0.5);
            Point point1 = new Point(width, 0.5);

            for (int i = 0; i <= rows; i++)
            {
                dct.DrawLine(darkPen, point0, point1);
                point0.Offset(0, yOffset);
                point1.Offset(0, yOffset);
            }
            point0 = new Point(0, 0.5);
            point1 = new Point(0, height);

            for (int i = 0; i <= columns; i++)
            {
                dct.DrawLine(darkPen, point0, point1);
                point0.Offset(xOffset, 0);
                point1.Offset(xOffset, 0);
            }

            dct.Close();
            _baseGridHash = _gridLinesVisual.Drawing.Bounds.GetHashCode();
            AddAndRender(_gridLinesVisual.Drawing);
        }
        private void ColourSquare(Point clickPoint, Color border, Color filling)
        {
            int startX = Normalise(clickPoint.X);
            int startY = Normalise(clickPoint.Y);
            DrawingContext dct = _gridLinesVisual.RenderOpen();
            Rect rect = new Rect(new Point(startX, startY), new Point(startX + _offSet, startY + _offSet));
            SolidColorBrush brush = new SolidColorBrush(filling);
            SolidColorBrush penBrush = new SolidColorBrush(border);
            dct.DrawRectangle(brush, new Pen(penBrush, 1), rect);
            dct.Close();
            AddAndRender(_gridLinesVisual.Drawing);
        }
        private void AddAndRender(Drawing drawing)
        {
            _drawingGroup.Children.Add(drawing);
            mainWindow.GridImage.Source = new DrawingImage(_drawingGroup);
        }

        private int Normalise(double number)
        {
            return (int)number;
        }

        private void PopulateFields(int rows, int columns)
        {
            for (int i = 0; i < rows; i++) 
            {
                for (int j = 0; j < columns; j++)
                {
                    _nodeHandler.CreateNode(j,i,new Point(j*_offset, i*_offset));
                }
            }
        }

        public void Deserialize(string content)
        {
            string[] lines = content.Split(Separator);
            for (int i = 0; i < _columns; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    Node n = JsonSerializer.Deserialize<Node>(lines[j*_rows+ i]);
                    _nodeHandler.CreateNode(j, i, n);
                    if (n.FieldType == FieldTypeValue.Wall)
                    {
                        LeftClick(new Point(j * _offset + 1, i * _offset + 1));
                    }
                }
            }
        }

        public void PathFind(int algorithmIndex)
        {
            var start = _nodeHandler.Start;
            var end = _nodeHandler.End;
            if ( start!= null && end != null)
            {
                visited = 0;
                BasePathFinding a;
                if (algorithmIndex == 0 )
                {
                    a = new AStar(start, end, null, _nodeHandler);
                } else //if(algorithmIndex == 1)
                {
                    a = new Dijkstra(start, end, _nodeHandler);
                }

                a.CurrentMark = ColorCurrent;
                a.VisitedMark = ColorVisited;
                Thread thread = new Thread(e =>
                {
                    List<Node> result = a.FindPath();
                    if (result == null)
                    {
                        mainWindow.Dispatcher.Invoke(() =>
                        {
                            mainWindow.lShortestPath.Content =  0.ToString();
                        });
                        return;
                    }

                    List<Node> drawingResult = result.GetRange(1, result.Count - 2);
                    drawingResult.ForEach(e =>
                    {
                        mainWindow.Dispatcher.Invoke(() =>
                        {
                            ColourSquare(e.TopLeft, _black, _blue);
                        });
                    });
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.lShortestPath.Content = (result.Count-1).ToString();
                    });
                });
                thread.IsBackground = true;
                thread.Start();
            }

        }

        public void ColorVisited(Node node)
        {
            if(DontColor(node))
                return;
            mainWindow.Dispatcher.Invoke(() =>
            {
                    ColourSquare(node.TopLeft, _black, _yellow);
            });
            Thread.Sleep(_sleepTimeInMiliseconds);
        }

        public void ColorCurrent(Node node)
        {
            if (DontColor(node))
                return;
            mainWindow.Dispatcher.Invoke(() =>
            {
                ColourSquare(node.TopLeft, _black, _orange);
                mainWindow.lVisitedNodes.Content = (visited++).ToString();

            });
            Thread.Sleep(_sleepTimeInMiliseconds);
        }

        private bool DontColor(Node node)
        {
            return _nodeHandler.Start.Equals(node) || _nodeHandler.End.Equals(node);
        }

        public void Clear(bool clearWalls)
        {
            List<List<Node>> nodeList = _nodeHandler.GetNodes();
            nodeList.ForEach(a =>
            {
                a.ForEach(b =>
                {
                    if (clearWalls)
                    {
                        b.SetTile();
                        ColourSquare(b.TopLeft,_black,_white);
                    } else if (b.IsPassable())
                    {
                        ColourSquare(b.TopLeft, _black, _white);
                    }
                });
            });
            _nodeHandler.Start = null;
            _nodeHandler.End = null;
            visited = 0;
            mainWindow.lVisitedNodes.Content = "";
            mainWindow.lShortestPath.Content = "";
        }
    }
}